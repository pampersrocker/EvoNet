using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvoNet.AI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using EvoNet.Map;
using System.Diagnostics;
using System.IO;
using EvoNet.Rendering;

namespace EvoNet.Objects
{



    public class Creature
    {
        private int collisionGridX = 0;
        private int collisionGridY = 0;

        private const int CREATURESIZE = 54;
        private const int FEELERTIPSIZE = 10;

        private float MAXIMUMFEELERDISTANCE = 100;
        private float feelerOcclusion = 0;

        private static int _maximumGeneration = 1;
        public static int maximumGeneration
        {
            get
            {
                return _maximumGeneration;
            }
            set { _maximumGeneration = value; }
        }

        private static Creature _oldestCreatureEver = null; //dummy creature
        public static Creature oldestCreatureEver
        {
            get
            {
                return _oldestCreatureEver;
            }
        }

        private const float COST_EAT = 10f;
        private const float GAIN_EAT = 100f;
        private const float COST_PERMANENT = 1f;
        private const float COST_WALK = 5f;
        private const float COST_ROTATE = 5f;

        private const float FOODDROPPERCENTAGE = 0;

        private const float ROTATE_FACTOR = 10f;
        private const float MOVESPEED = 1000f;

        private const float STARTENERGY = 150;
        private const float MINIMUMSURVIVALENERGY = 100;

        private static SpriteBatch spriteBatch = null;
        private static Texture2D bodyTex = null;
        private static Texture2D feelerTex = null;

        public static long currentId;

        public CreatureManager Manager { get; set; }

        // Id for serialization
        private long id;

        private Vector2 pos;
        public Vector2 Pos
        {
            get
            {
                return pos;
            }
        }
        private float viewAngle;

        private float feelerAngle;
        private Vector2 feelerPos;

        private float energy = 150;
        public float Energy
        {
            get
            {
                return energy;
            }
        }
        private float age = 0;
        public float Age
        {
            get
            {
                return age;
            }
        }

        private NeuronalNetwork brain;
        public NeuronalNetwork Brain
        {
            get
            {
                return brain;
            }
        }

        private const String NAME_IN_BIAS              = "bias";
        private const String NAME_IN_FOODVALUEPOSITION = "Food Value Position";
        private const String NAME_IN_FOODVALUEFEELER   = "Food Value Feeler";
        private const String NAME_IN_OCCLUSIONFEELER   = "Occlusion Feeler";
        private const String NAME_IN_ENERGY            = "Energy";
        private const String NAME_IN_AGE               = "Age";
        private const String NAME_IN_GENETICDIFFERENCE = "Genetic Difference";
        private const String NAME_IN_WASATTACKED       = "Was Attacked";
        private const String NAME_IN_WATERONFEELER     = "Water On Feeler";
        private const String NAME_IN_WATERONCREATURE   = "Water On Creature";
        private const String NAME_IN_MEMORY1           = "Input Memory #1";

        private const String NAME_OUT_BIRTH       = "Birth";
        private const String NAME_OUT_ROTATE      = "Rotate";
        private const String NAME_OUT_FORWARD     = "Forward";
        private const String NAME_OUT_FEELERANGLE = "Feeler Angle";
        private const String NAME_OUT_ATTACK      = "Attack";
        private const String NAME_OUT_EAT         = "Eat";
        private const String NAME_OUT_MEMORY1     = "Output Memory #1";

        private InputNeuron inBias              = new InputNeuron();
        private InputNeuron inFoodValuePosition = new InputNeuron();
        private InputNeuron inFoodValueFeeler   = new InputNeuron();
        private InputNeuron inOcclusionFeeler   = new InputNeuron();
        private InputNeuron inEnergy            = new InputNeuron();
        private InputNeuron inAge               = new InputNeuron();
        private InputNeuron inGeneticDifference = new InputNeuron();
        private InputNeuron inWasAttacked       = new InputNeuron();
        private InputNeuron inWaterOnFeeler     = new InputNeuron();
        private InputNeuron inWaterOnCreature   = new InputNeuron();
        private InputNeuron inMemory1           = new InputNeuron();

        private WorkingNeuron outBirth       = new WorkingNeuron();
        private WorkingNeuron outRotate      = new WorkingNeuron();
        private WorkingNeuron outForward     = new WorkingNeuron();
        private WorkingNeuron outFeelerAngle = new WorkingNeuron();
        private WorkingNeuron outAttack      = new WorkingNeuron();
        private WorkingNeuron outEat         = new WorkingNeuron();
        private WorkingNeuron outMemory1     = new WorkingNeuron();

        private Color color;
        private Color color_inv;

        private Creature mother;


        private List<Creature> children = new List<Creature>();
        public List<Creature> Children
        {
            get
            {
                return children;
            }
        }

        // Temps for deserialization
        private long motherId;
        private List<long> childIds = new List<long>();

        private int generation = 1;
        public int Generation
        {
            get
            {
                return generation;
            }
        }

        public static void Initialize()
        {
            if(spriteBatch == null)
            {
                spriteBatch = EvoGame.spriteBatch;
                bodyTex = EvoGame.WhiteCircleTexture;
                feelerTex = EvoGame.WhiteCircleTexture;
            }
        }

        public Creature(Vector2 pos, float viewAngle)
        {
            id = currentId++;

            this.pos = pos;
            this.viewAngle = viewAngle;
            inBias             .SetName(NAME_IN_BIAS);
            inFoodValuePosition.SetName(NAME_IN_FOODVALUEPOSITION);
            inFoodValueFeeler  .SetName(NAME_IN_FOODVALUEFEELER);
            inOcclusionFeeler  .SetName(NAME_IN_OCCLUSIONFEELER);
            inEnergy           .SetName(NAME_IN_ENERGY);
            inAge              .SetName(NAME_IN_AGE);
            inGeneticDifference.SetName(NAME_IN_GENETICDIFFERENCE);
            inWasAttacked      .SetName(NAME_IN_WASATTACKED);
            inWaterOnFeeler    .SetName(NAME_IN_WATERONFEELER);
            inWaterOnCreature  .SetName(NAME_IN_WATERONCREATURE);
            inMemory1          .SetName(NAME_IN_MEMORY1);

            outBirth      .SetName(NAME_OUT_BIRTH);
            outRotate     .SetName(NAME_OUT_ROTATE);
            outForward    .SetName(NAME_OUT_FORWARD);
            outFeelerAngle.SetName(NAME_OUT_FEELERANGLE);
            outAttack     .SetName(NAME_OUT_ATTACK);
            outEat        .SetName(NAME_OUT_EAT);
            outMemory1    .SetName(NAME_OUT_MEMORY1);

            brain = new NeuronalNetwork();

            brain.AddInputNeuron(inBias);
            brain.AddInputNeuron(inFoodValuePosition);
            brain.AddInputNeuron(inFoodValueFeeler);
            brain.AddInputNeuron(inOcclusionFeeler);
            brain.AddInputNeuron(inEnergy);
            brain.AddInputNeuron(inAge);
            brain.AddInputNeuron(inGeneticDifference);
            brain.AddInputNeuron(inWasAttacked);
            brain.AddInputNeuron(inWaterOnFeeler);
            brain.AddInputNeuron(inWaterOnCreature);
            brain.AddInputNeuron(inMemory1);

            brain.GenerateHiddenNeurons(10);

            brain.AddOutputNeuron(outBirth);
            brain.AddOutputNeuron(outRotate);
            brain.AddOutputNeuron(outForward);
            brain.AddOutputNeuron(outFeelerAngle);
            brain.AddOutputNeuron(outAttack);
            brain.AddOutputNeuron(outEat);
            brain.AddOutputNeuron(outMemory1);

            brain.GenerateFullMesh();

            brain.RandomizeAllWeights();
            CalculateFeelerPos(MAXIMUMFEELERDISTANCE);

            color = new Color((float)EvoGame.GlobalRandom.NextDouble(), (float)EvoGame.GlobalRandom.NextDouble(), (float)EvoGame.GlobalRandom.NextDouble());
            GenerateColorInv();
            CalculateCollisionGridPos();
        }

        public Creature(Creature mother)
        {
            id = currentId++;
            this.mother = mother;
            generation = mother.generation + 1;
            if(generation > _maximumGeneration)
            {
                _maximumGeneration = generation;
            }
            this.pos = mother.pos;
            this.viewAngle = (float)EvoGame.GlobalRandom.NextDouble() * Mathf.PI * 2;
            this.brain = mother.brain.CloneFullMesh();

            SetupVariablesFromBrain();


            CalculateFeelerPos(MAXIMUMFEELERDISTANCE);
            for (int i = 0; i < 10; i++)
            {
                brain.RandomMutation(0.1f);
            }

            float r = mother.color.R / 255f;
            float g = mother.color.G / 255f;
            float b = mother.color.B / 255f;

            r += (float)EvoGame.GlobalRandom.NextDouble() * 0.1f - 0.05f;
            g += (float)EvoGame.GlobalRandom.NextDouble() * 0.1f - 0.05f;
            b += (float)EvoGame.GlobalRandom.NextDouble() * 0.1f - 0.05f;

            r = Mathf.Clamp01(r);
            g = Mathf.Clamp01(g);
            b = Mathf.Clamp01(b);

            color = new Color(r, g, b);
            GenerateColorInv();
        }


        // For deserialization
        public Creature()
        {
            CalculateCollisionGridPos();
        }

        private void SetupVariablesFromBrain()
        {
            inBias = brain.GetInputNeuronFromName(NAME_IN_BIAS);
            inFoodValuePosition = brain.GetInputNeuronFromName(NAME_IN_FOODVALUEPOSITION);
            inFoodValueFeeler = brain.GetInputNeuronFromName(NAME_IN_FOODVALUEFEELER);
            inOcclusionFeeler = brain.GetInputNeuronFromName(NAME_IN_OCCLUSIONFEELER);
            inEnergy = brain.GetInputNeuronFromName(NAME_IN_ENERGY);
            inAge = brain.GetInputNeuronFromName(NAME_IN_AGE);
            inGeneticDifference = brain.GetInputNeuronFromName(NAME_IN_GENETICDIFFERENCE);
            inWasAttacked = brain.GetInputNeuronFromName(NAME_IN_WASATTACKED);
            inWaterOnFeeler = brain.GetInputNeuronFromName(NAME_IN_WATERONFEELER);
            inWaterOnCreature = brain.GetInputNeuronFromName(NAME_IN_WATERONCREATURE);
            inMemory1 = brain.GetInputNeuronFromName(NAME_IN_MEMORY1);

            outBirth = brain.GetOutputNeuronFromName(NAME_OUT_BIRTH);
            outRotate = brain.GetOutputNeuronFromName(NAME_OUT_ROTATE);
            outForward = brain.GetOutputNeuronFromName(NAME_OUT_FORWARD);
            outFeelerAngle = brain.GetOutputNeuronFromName(NAME_OUT_FEELERANGLE);
            outAttack = brain.GetOutputNeuronFromName(NAME_OUT_ATTACK);
            outEat = brain.GetOutputNeuronFromName(NAME_OUT_EAT);
            outMemory1 = brain.GetOutputNeuronFromName(NAME_OUT_MEMORY1);
            CalculateCollisionGridPos();
        }

        private void CalculateCollisionGridPos()
        {
            collisionGridX = (int)((pos.X / EvoGame.Instance.tileMap.GetWorldWidth()) * CreatureManager.COLLISIONGRIDSIZE / 3 + CreatureManager.COLLISIONGRIDSIZE / 3);
            collisionGridY = (int)((pos.Y / EvoGame.Instance.tileMap.GetWorldHeight()) *CreatureManager.COLLISIONGRIDSIZE / 3 + CreatureManager.COLLISIONGRIDSIZE / 3);
            CreatureManager.CollisionGrid[collisionGridX, collisionGridY].Add(this);
        }

        public void GenerateColorInv()
        {
            color_inv = new Color(255 - color.R, 255 - color.G, 255 - color.B);
        }

        public void ReadSensors()
        {
            //lock (this)
            {
                inMemory1.SetValue(outMemory1.GetValue());

                brain.Invalidate();

                Tile creatureTile = EvoGame.Instance.tileMap.GetTileAtWorldPosition(pos);
                Tile feelerTile = EvoGame.Instance.tileMap.GetTileAtWorldPosition(feelerPos);

                inBias.SetValue(1);
                inFoodValuePosition.SetValue(creatureTile.food / TileMap.MAXIMUMFOODPERTILE);
                inFoodValueFeeler.SetValue(feelerTile.food / TileMap.MAXIMUMFOODPERTILE);
                inOcclusionFeeler.SetValue(feelerOcclusion);
                inEnergy.SetValue((energy - MINIMUMSURVIVALENERGY) / (STARTENERGY - MINIMUMSURVIVALENERGY));
                inAge.SetValue(age / 10f);
                inGeneticDifference.SetValue(0); //TODO find real value
                inWasAttacked.SetValue(0); //TODO find real value
                inWaterOnFeeler.SetValue(feelerTile.IsLand() ? 0 : 1);
                inWaterOnCreature.SetValue(creatureTile.IsLand() ? 0 : 1);
            }
            
        }

        public void Act(GameTime deltaTime)
        {
            float fixedDeltaTime = (float)deltaTime.ElapsedGameTime.TotalSeconds;
            Tile t = EvoGame.Instance.tileMap.GetTileAtWorldPosition(pos);
            float costMult = CalculateCostMultiplier(t);
            ActRotate(costMult, fixedDeltaTime);
            ActMove(costMult, fixedDeltaTime);
            ActBirth();
            ActFeelerRotate();
            ActEat(costMult, t, fixedDeltaTime);

            age += fixedDeltaTime;

            if (oldestCreatureEver == null)
            {
                _oldestCreatureEver = this;
            }
            if (age > _oldestCreatureEver.age)
            {
                _oldestCreatureEver = this;
            }

            //TODO implement Attack

            if (energy < 100 || float.IsNaN(energy))
            {
                Kill(t);
            }
            
            CalculateCollisionGridPos();
        }

        private void Kill(Tile t)
        {
            if (t.IsLand())
            {
                EvoGame.Instance.tileMap.FoodValues[t.position.X, t.position.Y] += energy * FOODDROPPERCENTAGE;
            }
            Manager.CreaturesToKill.Add(this);
        }

        private void ActRotate(float costMult, float fixedDeltaTime)
        {
            float rotateForce = Mathf.ClampNegPos(outRotate.GetValue());
            this.viewAngle += rotateForce * fixedDeltaTime * ROTATE_FACTOR;
            energy -= Mathf.Abs(rotateForce * COST_ROTATE * fixedDeltaTime * costMult);
        }

        private void ActMove(float costMult, float fixedDeltaTime)
        {
            Vector2 forwardVector = new Vector2(Mathf.Sin(viewAngle), Mathf.Cos(viewAngle)) * MOVESPEED * fixedDeltaTime;
            float forwardForce = Mathf.ClampNegPos(outForward.GetValue());
            forwardVector *= forwardForce;
            this.pos += forwardVector;
            energy -= Mathf.Abs(forwardForce * COST_WALK * fixedDeltaTime * costMult);
        }

        private void ActBirth()
        {
            float birthWish = outBirth.GetValue();
            if (birthWish > 0) TryToGiveBirth();
        }

        private void ActFeelerRotate()
        {
            feelerAngle = Mathf.ClampNegPos(outFeelerAngle.GetValue()) * Mathf.PI;
        }

        private void ActEat(float costMult, Tile creatureTile, float fixedDeltaTime)
        {
            float eatWish = Mathf.Clamp01(outEat.GetValue());
            if (eatWish > 0)
            {
                Eat(eatWish, creatureTile, fixedDeltaTime);
                energy -= eatWish * COST_EAT * fixedDeltaTime * costMult;
            }
        }

        private void Eat(float eatWish, Tile t, float fixedDeltaTime)
        {
            if(t.type != TileType.None)
            {
                float foodVal = EvoGame.Instance.tileMap.FoodValues[t.position.X, t.position.Y];
                if (foodVal > 0)
                {
                    float eatAmount = GAIN_EAT * eatWish * fixedDeltaTime;
                    if (foodVal > eatAmount)
                    {
                        energy += eatAmount;
                        EvoGame.Instance.tileMap.FoodValues[t.position.X, t.position.Y] -= eatAmount;
                    }
                    else
                    {
                        energy += foodVal;
                        EvoGame.Instance.tileMap.FoodValues[t.position.X, t.position.Y] = 0;
                    }
                }
            }

        }

        private float CalculateCostMultiplier(Tile CreatureTile)
        {
            return age * (CreatureTile.IsLand() ? 1 : 2);
        }

        public void TryToGiveBirth()
        {
            if (IsAbleToGiveBirth())
            {
                GiveBirth();
            }
        }

        public void GiveBirth()
        {
            Creature child = new Creature(this);
            child.Manager = Manager;
            children.Add(child);
            Manager.CreaturesToSpawn.Add(child);
            energy -= STARTENERGY;
        }

        public bool IsAbleToGiveBirth()
        {
            return energy > STARTENERGY + MINIMUMSURVIVALENERGY * 1.1f;
        }

        public void CalculateFeelerPos(float feelerDistance)
        {
            float angle = feelerAngle + viewAngle;
            Vector2 localFeelerPos = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle)) * feelerDistance;
            feelerPos = pos + localFeelerPos;
        }

        public void Draw()
        {
            //lock (this)
            {

                spriteBatch.Begin(transformMatrix: Camera.instanceGameWorld.Matrix);
                DrawCreature(spriteBatch, Vector2.Zero);

                spriteBatch.End();
            }
        }

        public void DrawCreature(SpriteBatch spriteBatch, Vector2 offset)
        {
            RenderHelper.DrawLine(spriteBatch, pos.X + offset.X, pos.Y + offset.Y, feelerPos.X + offset.X, feelerPos.Y + offset.Y, Color.White);
            spriteBatch.Draw(bodyTex, new Rectangle((int)(pos.X + offset.X - CREATURESIZE / 2), (int)(pos.Y + offset.Y - CREATURESIZE / 2), CREATURESIZE, CREATURESIZE), color_inv);
            spriteBatch.Draw(bodyTex, new Rectangle((int)(pos.X + offset.X - (CREATURESIZE - 4) / 2), (int)(pos.Y + offset.Y - (CREATURESIZE - 4) / 2), CREATURESIZE - 4, CREATURESIZE - 4), color);
            spriteBatch.Draw(feelerTex, new Rectangle((int)(feelerPos.X + offset.X - 5), (int)(feelerPos.Y + offset.Y - 5), 10, 10), Color.Blue);
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write("CreatureBegin");
            writer.Write(id);
            writer.Write(pos);
            writer.Write(viewAngle);
            writer.Write(feelerAngle);
            writer.Write(energy);
            writer.Write(age);
            writer.Write(generation);
            writer.Write(color);
            writer.Write(mother != null ? mother.id : -1);
            writer.Write(children.Count);
            foreach (Creature child in children)
            {
                writer.Write(child.id);
            }
            brain.Serialize(writer);
        }

        public void ConnectAncestry(List<Creature> allThemCreatures)
        {
            if (mother == null)
            {
                mother = allThemCreatures.Find((p) => p.id == motherId);
            }
            if (childIds != null && children.Count != childIds.Count)
            {
                foreach (long childId in childIds)
                {
                    Creature child = allThemCreatures.Find((p) => p.id == childId);
                    if (child != null)
                    {
                        children.Add(child);
                    }

                }
            }
        }

        public void Deserialize(BinaryReader reader)
        {
            string magicString = reader.ReadString();
            Debug.Assert(magicString == "CreatureBegin");
            id = reader.ReadInt64();
            pos = reader.ReadVector2();
            viewAngle = reader.ReadSingle();
            feelerAngle = reader.ReadSingle();
            energy = reader.ReadSingle();
            age = reader.ReadSingle();
            generation = reader.ReadInt32();
            color = reader.ReadColor();
            motherId = reader.ReadInt64();
            int childrenCount = reader.ReadInt32();
            childIds = new List<long>();
            for (int childIndex = 0; childIndex< childrenCount; childIndex++)
            {
                childIds.Add(reader.ReadInt64());
            }
            brain = new NeuronalNetwork();
            brain.Deserialize(reader);

            SetupVariablesFromBrain();
        }

        public void HandleCollisions()
        {
            //lock (this)
            {
                feelerOcclusion = 0;
                CalculateFeelerPos(MAXIMUMFEELERDISTANCE);
                for (int i = collisionGridX - 1; i <= collisionGridX + 1; i++)
                {
                    for (int k = collisionGridY - 1; k <= collisionGridY + 1; k++)
                    {
                        if (i >= 0 && k >= 0 && i < CreatureManager.COLLISIONGRIDSIZE && k < CreatureManager.COLLISIONGRIDSIZE)
                        {
                            List<Creature> collisionList = CreatureManager.CollisionGrid[i, k];
                            HandleCollisionsWithList(collisionList);
                        }
                    }
                }
            }
        }

        private void HandleCollisionsWithList(List<Creature> creatures)
        {
            foreach(Creature c in creatures)
            {
                HandleCollisionWithCreature(c);
            }
        }

        private void HandleCollisionWithCreature(Creature c)
        {
            if (this == c) return;
            if(feelerOcclusion != 1) HandleCollisionWithCreatureEye(c);
        }

        private void HandleCollisionWithCreatureEye(Creature c)
        {
            for(float t = 0; t<= 1 - feelerOcclusion; t += 0.1f)
            {
                CalculateFeelerPos(MAXIMUMFEELERDISTANCE * t);
                if (IsMyFeelerCollidingWithCreature(c))
                {
                    long idd = this.id;
                    feelerOcclusion = 1 - t;
                    return;
                }
            }
        }

        private bool IsMyFeelerCollidingWithCreature(Creature c)
        {
            float dist = (this.feelerPos - c.pos).LengthSquared();
            float minDist = (FEELERTIPSIZE + CREATURESIZE) / 2;
            minDist *= minDist;
            return dist < minDist;
        }
    }
}
