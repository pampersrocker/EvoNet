using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvoNet.AI;
using EvoNet.Map;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using EvoSim;
using System.Runtime.Serialization;
using EvoSim.Serialization;
using System.Security.Permissions;
using EvoSim.Tasks;

namespace EvoNet.Objects
{


    [Serializable]
    public class Creature : ISerializable
    {
        private int collisionGridX = 0;
        private int collisionGridY = 0;

        public const int CREATURESIZE = 54;
        public const int FEELERTIPSIZE = 10;

        private const float MAXIMUMFEELERDISTANCE = 100;
        private float feelerOcclusion = 0;

        private static int _maximumGeneration = 1;
        public static int maximumGeneration
        {

            [MethodImpl(MethodImplOptions.Synchronized)]
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

        private const float MINAGETOGIVEBIRTH = 4f;
        private const float COST_EAT = 1f;
        private const float COST_ATTACK = 10f;
        private const float GAIN_EAT = 100f;
        private const float DESTROYED_ATTACK = 110f;
        private const float GAIN_ATTACK = 50f;
        private const float COST_PERMANENT = 1f;
        private const float COST_WALK = 5f;
        private const float COST_ROTATE = 5f;
        private const float COST_PER_MEMORY_NEURON = 1.5f;

        private const float FOODDROPPERCENTAGE = 0;

        private const float ROTATE_FACTOR = 10f;
        private const float MOVESPEED = 1000f;

        private const float STARTENERGY = 150;
        private const float MINIMUMSURVIVALENERGY = 100;



        public static long currentId;
        [NonSerialized]
        private CreatureManager manager;
        public CreatureManager Manager { get { return manager; } set { manager = value; } }

        // Id for serialization
        private long id;
        public long Id
        {
            get { return id; }
        }
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
        public Vector2 FeelerPos
        {
            get { return feelerPos; }
        }
        private float energy_ = 150;
        private object energyLock = new object();
        public float Energy
        {
            get
            {
                lock (energyLock)
                {
                    return energy_;
                }
            }
            set
            {
                lock (energyLock)
                {
                    energy_ = value;
                }
            }
        }

        private float timeSinceThisWasAttacked_ = 1;
        private object AttackLock = new object();
        public float TimeSinceThisWasAttacked
        {
            get
            {
                lock (AttackLock)
                {
                    return timeSinceThisWasAttacked_;
                }
            }
            set
            {
                lock (AttackLock)
                {
                    timeSinceThisWasAttacked_ = value;
                }
            }
        }

        private Vector2 forward = new Vector2();
        public Vector2 Forward
        {
            get
            {
                return forward;
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

        public EvoSim.Simulation Simulation
        {
            get { return Manager.simulation; }
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
        //private const String NAME_IN_OSCILATION        = "Oscilation input";
        private const String NAME_IN_MEMORY            = "Input Memory #";

        private const String NAME_OUT_BIRTH       = "Birth";
        private const String NAME_OUT_ROTATE      = "Rotate";
        private const String NAME_OUT_FORWARD     = "Forward";
        private const String NAME_OUT_STRAFE      = "Strafe";
        private const String NAME_OUT_FEELERANGLE = "Feeler Angle";
        private const String NAME_OUT_ATTACK      = "Attack";
        private const String NAME_OUT_EAT         = "Eat";
        //private const String NAME_OUT_OSCILATION  = "Oscilation output";
        private const String NAME_OUT_MEMORY      = "Output Memory #";

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
        //private InputNeuron inOscilation        = new InputNeuron();
        private InputNeuron[] inMemory          = null;

        private WorkingNeuron outBirth       = new WorkingNeuron();
        private WorkingNeuron outRotate      = new WorkingNeuron();
        private WorkingNeuron outForward     = new WorkingNeuron();
        private WorkingNeuron outStrafe      = new WorkingNeuron();
        private WorkingNeuron outFeelerAngle = new WorkingNeuron();
        private WorkingNeuron outAttack      = new WorkingNeuron();
        private WorkingNeuron outEat         = new WorkingNeuron();
        //private WorkingNeuron outOscilation  = new WorkingNeuron();
        private WorkingNeuron[] outMemory    = null;

        private object amountOfMemoryLock = new object();
        private int amountOfMemory_ = 1;
        public int AmountOfMemory
        {
            get
            {
                lock (amountOfMemoryLock)
                {
                    return amountOfMemory_;
                }
            }
            set
            {
                lock (amountOfMemoryLock)
                {
                    amountOfMemory_ = value;
                }
            }
        }

        private object oscilationLock = new object();
        private float oscilationValue_;
        private float OscilationValue
        {
            get
            {
                lock (oscilationLock)
                {
                    return oscilationValue_;
                }
            }
            set
            {
                lock (oscilationLock)
                {
                    oscilationValue_ = value;
                }
            }
        }

        private object colorLock = new object();
        private Color color_;
        private Color color_inv_;
        public Color Color
        {
            get
            {
                lock (colorLock)
                {
                    return color_;
                }
            }
            set
            {
                lock (colorLock)
                {
                    color_ = value;
                }
            }
        }
        public Color Color_inv
        {
            get
            {
                lock (colorLock)
                {
                    return color_inv_;
                }
            }
            set
            {
                lock (colorLock)
                {
                    color_inv_ = value;
                }
            }
        }

        private float timeSinceLastAttack = 0;
        public float TimeSinceLastAttack
        {
            get { return timeSinceLastAttack; }
        }
        public const float TIMEBETWEENATTACKS = 0.1f;

        private Creature feelerCreature = null;

        // Temps for deserialization
        private long motherId;
        private List<long> childIds = new List<long>();
        public List<long> Children
        {
            get { return childIds; }
        }
        private int generation = 1;
        public int Generation
        {
            get
            {
                return generation;
            }
        }


        public Creature(Vector2 pos, float viewAngle, CreatureManager manager) :
            this(manager)
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
            //inOscilation       .SetName(NAME_IN_OSCILATION);
            inMemory = new InputNeuron[AmountOfMemory];
            for(int i = 0; i<AmountOfMemory; i++)
            {
                inMemory[i] = new InputNeuron();
                inMemory[i].SetName(NAME_IN_MEMORY + (i + 1));
            }

            outBirth      .SetName(NAME_OUT_BIRTH);
            outRotate     .SetName(NAME_OUT_ROTATE);
            outForward    .SetName(NAME_OUT_FORWARD);
            outStrafe     .SetName(NAME_OUT_STRAFE);
            outFeelerAngle.SetName(NAME_OUT_FEELERANGLE);
            outAttack     .SetName(NAME_OUT_ATTACK);
            outEat        .SetName(NAME_OUT_EAT);
            //outOscilation .SetName(NAME_OUT_OSCILATION);
            outMemory = new WorkingNeuron[AmountOfMemory];
            for(int i = 0; i<AmountOfMemory; i++)
            {
                outMemory[i] = new WorkingNeuron();
                outMemory[i].SetName(NAME_OUT_MEMORY + (i + 1));
            }

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
            //brain.AddInputNeuron(inOscilation);
            for(int i = 0; i<AmountOfMemory; i++)
            {
                brain.AddInputNeuron(inMemory[i]);
            }

            brain.GenerateHiddenNeurons(10);

            brain.AddOutputNeuron(outBirth);
            brain.AddOutputNeuron(outRotate);
            brain.AddOutputNeuron(outForward);
            brain.AddOutputNeuron(outStrafe);
            brain.AddOutputNeuron(outFeelerAngle);
            brain.AddOutputNeuron(outAttack);
            brain.AddOutputNeuron(outEat);
            //brain.AddOutputNeuron(outOscilation);
            for(int i = 0; i < AmountOfMemory; i++)
            {
                brain.AddOutputNeuron(outMemory[i]);
            }

            brain.GenerateFullMesh();

            brain.RandomizeAllWeights();
            CalculateFeelerPos(MAXIMUMFEELERDISTANCE);

            Color = Color.FromFloat(Simulation.RandomFloat(), Simulation.RandomFloat(), Simulation.RandomFloat());
            GenerateColorInv();
            CalculateCollisionGridPos();
        }

        public Creature(Creature mother, CreatureManager manager) :
            this(manager)
        {
            id = currentId++;
            motherId = mother.id;
            generation = mother.generation + 1;
            if(generation > _maximumGeneration)
            {
                _maximumGeneration = generation;
            }
            this.pos = mother.pos;
            this.viewAngle = Simulation.RandomFloat() * Mathf.PI * 2;
            this.brain = mother.brain.CloneFullMesh();

            AmountOfMemory = mother.AmountOfMemory;
            inMemory = new InputNeuron[AmountOfMemory];
            outMemory = new WorkingNeuron[AmountOfMemory];

            SetupVariablesFromBrain();


            CalculateFeelerPos(MAXIMUMFEELERDISTANCE);
            if(Simulation.RandomFloat() > 0.01f)
            {
                MutateConnections();
            }
            else
            {
                MutateMemory();
            }

            MutateColor(mother);
            GenerateColorInv();

            if(manager.SelectedCreature == null || manager.SelectedCreature.Energy < 100)
            {
                manager.SelectedCreature = this;
            }
        }

        private void MutateMemory()
        {
            if(Simulation.RandomFloat() > 0.5f && AmountOfMemory > 0)
            {
                //Remove a memory neuron
                InputNeuron inRemove = inMemory[AmountOfMemory - 1];
                WorkingNeuron outRemove = outMemory[AmountOfMemory - 1];
                brain.RemoveInputNeuron(inRemove);
                brain.RemoveOutputNeuron(outRemove);
                InputNeuron[] newInputNeurons = new InputNeuron[AmountOfMemory - 1];
                WorkingNeuron[] newOutputNeurons = new WorkingNeuron[AmountOfMemory - 1];
                for(int i = 0; i<AmountOfMemory - 1; i++)
                {
                    newInputNeurons[i] = inMemory[i];
                    newOutputNeurons[i] = outMemory[i];
                }
                inMemory = newInputNeurons;
                outMemory = newOutputNeurons;
                AmountOfMemory--;
            }
            else
            {
                //Add a memory neuron
                InputNeuron newIn = new InputNeuron();
                WorkingNeuron newOut = new WorkingNeuron();
                newIn.SetName(NAME_IN_MEMORY + (AmountOfMemory + 1));
                newOut.SetName(NAME_OUT_MEMORY + (AmountOfMemory + 1));
                brain.AddInputNeuronAndMesh(newIn);
                brain.AddOutputNeuronAndMesh(newOut);
                InputNeuron[] newInputNeurons = new InputNeuron[AmountOfMemory + 1];
                WorkingNeuron[] newOutputNeurons = new WorkingNeuron[AmountOfMemory + 1];
                for (int i = 0; i < AmountOfMemory; i++)
                {
                    newInputNeurons[i] = inMemory[i];
                    newOutputNeurons[i] = outMemory[i];
                }
                newInputNeurons[AmountOfMemory] = newIn;
                newOutputNeurons[AmountOfMemory] = newOut;
                inMemory = newInputNeurons;
                outMemory = newOutputNeurons;
                AmountOfMemory++;
            }
        }

        private void MutateConnections()
        {
            for (int i = 0; i < 10; i++)
            {
                brain.RandomMutation(0.1f);
            }
        }

        private void MutateColor(Creature mother)
        {
            int r = mother.Color.R;
            int g = mother.Color.G;
            int b = mother.Color.B;

            r += Simulation.RandomInt(-5, 6);
            g += Simulation.RandomInt(-5, 6);
            b += Simulation.RandomInt(-5, 6);

            r = Mathf.ClampColorValue(r);
            g = Mathf.ClampColorValue(g);
            b = Mathf.ClampColorValue(b);

            Color = new Color(r, g, b, 255);
        }


        // For deserialization
        public Creature(CreatureManager manager)
        {
            Manager = manager;
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
            //inOscilation = brain.GetInputNeuronFromName(NAME_IN_OSCILATION);
            for(int i = 0; i<AmountOfMemory; i++)
            {
                inMemory[i] = brain.GetInputNeuronFromName(NAME_IN_MEMORY + (i + 1));
            }

            outBirth = brain.GetOutputNeuronFromName(NAME_OUT_BIRTH);
            outRotate = brain.GetOutputNeuronFromName(NAME_OUT_ROTATE);
            outForward = brain.GetOutputNeuronFromName(NAME_OUT_FORWARD);
            outStrafe = brain.GetOutputNeuronFromName(NAME_OUT_STRAFE);
            outFeelerAngle = brain.GetOutputNeuronFromName(NAME_OUT_FEELERANGLE);
            outAttack = brain.GetOutputNeuronFromName(NAME_OUT_ATTACK);
            outEat = brain.GetOutputNeuronFromName(NAME_OUT_EAT);
            //outOscilation = brain.GetOutputNeuronFromName(NAME_OUT_OSCILATION);
            for(int i = 0; i<AmountOfMemory; i++)
            {
                outMemory[i] = brain.GetOutputNeuronFromName(NAME_OUT_MEMORY + (i + 1));
            }
            CalculateCollisionGridPos();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void CalculateCollisionGridPos()
        {
            lock (this)
            {
                collisionGridX = (int)((pos.X / Simulation.TileMap.GetWorldWidth()) * CreatureManager.COLLISIONGRIDSIZE / 3 + CreatureManager.COLLISIONGRIDSIZE / 3);
                collisionGridY = (int)((pos.Y / Simulation.TileMap.GetWorldHeight()) * CreatureManager.COLLISIONGRIDSIZE / 3 + CreatureManager.COLLISIONGRIDSIZE / 3);
                collisionGridX = Mathf.Clamp(collisionGridX, 0, CreatureManager.COLLISIONGRIDSIZE - 1);
                collisionGridY = Mathf.Clamp(collisionGridY, 0, CreatureManager.COLLISIONGRIDSIZE - 1);
                CreatureManager.AddToCollisionGrid(collisionGridX, collisionGridY, this);
            }
        }

        private float CalculateGeneticDifferencToFeelerCreature()
        {
            if(feelerCreature == null)
            {
                return 0;
            }
            Vector3 vec = new Vector3(
                Color.R - feelerCreature.Color.R,
                Color.G - feelerCreature.Color.G,
                Color.B - feelerCreature.Color.B
                );

            vec /= 255f;
            return vec.Length();
        }

        public void GenerateColorInv()
        {
            Color_inv = new Color(255 - Color.R, 255 - Color.G, 255 - Color.B, 255);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void ReadSensors()
        {
            for(int i = 0; i<AmountOfMemory; i++)
            {
                inMemory[i].SetValue(outMemory[i].GetValue());
            }

            brain.Invalidate();

            Tile creatureTile = Simulation.TileMap.GetTileAtWorldPosition(pos);
            Tile feelerTile = Simulation.TileMap.GetTileAtWorldPosition(feelerPos);

            inBias.SetValue(1);
            inFoodValuePosition.SetValue(creatureTile.food / TileMap.MAXIMUMFOODPERTILE);
            inFoodValueFeeler.SetValue(feelerTile.food / TileMap.MAXIMUMFOODPERTILE);
            inOcclusionFeeler.SetValue(feelerOcclusion);
            inEnergy.SetValue((Energy - MINIMUMSURVIVALENERGY) / (STARTENERGY - MINIMUMSURVIVALENERGY));
            inAge.SetValue(age / 10f);
            inGeneticDifference.SetValue(CalculateGeneticDifferencToFeelerCreature());
            inWasAttacked.SetValue(Mathf.Clamp01(1 - TimeSinceThisWasAttacked));
            inWaterOnFeeler.SetValue(feelerTile.IsLand() ? 0 : 1);
            inWaterOnCreature.SetValue(creatureTile.IsLand() ? 0 : 1);
            //inOscilation.SetValue(Mathf.Sin(OscilationValue));
        }

        CreatureTask currentTask;
        internal CreatureTask CurrentTask
        {
            get { return currentTask; }
            set { currentTask = value; }
        }
        public void Act(float deltaTime)
        {
            float fixedDeltaTime = deltaTime;
            Tile t = Simulation.TileMap.GetTileAtWorldPosition(pos);
            float costMult = CalculateCostMultiplier(t);
            ActRotate(costMult, fixedDeltaTime);
            forward = new Vector2(Mathf.Sin(viewAngle), Mathf.Cos(viewAngle));
            if(Mathf.Abs(outForward.GetValue()) > Mathf.Abs(outStrafe.GetValue()))
            {
                ActMove(costMult, fixedDeltaTime, forward);
            }
            else
            {
                ActStrafe(costMult, fixedDeltaTime, forward);
            }
            ActBirth();
            ActFeelerRotate();
            ActEat(costMult, t, fixedDeltaTime);
            ActAttack(costMult);

            //OscilationValue += outOscilation.GetValue();

            Energy -= COST_PERMANENT * fixedDeltaTime * costMult;
            Energy -= COST_PER_MEMORY_NEURON * fixedDeltaTime * costMult * AmountOfMemory;

            timeSinceLastAttack += fixedDeltaTime;
            TimeSinceThisWasAttacked += fixedDeltaTime;
            age += fixedDeltaTime;

            if (oldestCreatureEver == null)
            {
                _oldestCreatureEver = this;
            }
            if (age > _oldestCreatureEver.age)
            {
                _oldestCreatureEver = this;
            }

            if (Energy < 100 || float.IsNaN(Energy))
            {
                Kill(t);
            }
            
            CalculateCollisionGridPos();
        }

        private void Kill(Tile t)
        {
            if (t.IsLand())
            {
                Simulation.TileMap.FoodValues[t.position.X, t.position.Y] += Energy * FOODDROPPERCENTAGE;
            }
            currentTask.RemoveCreature(this);
        }

        private void ActAttack(float costMult)
        {
            if(outAttack.GetValue() > 0 && timeSinceLastAttack > TIMEBETWEENATTACKS)
            {
                timeSinceLastAttack = 0;
                Energy -= COST_ATTACK;
                if(feelerCreature != null)
                {
                    feelerCreature.Energy -= DESTROYED_ATTACK;
                    feelerCreature.TimeSinceThisWasAttacked = 0;
                    this.Energy += GAIN_ATTACK;
                }
            }
        }

        private void ActRotate(float costMult, float fixedDeltaTime)
        {
            float rotateForce = Mathf.ClampNegPos(outRotate.GetValue());
            this.viewAngle += rotateForce * fixedDeltaTime * ROTATE_FACTOR;
            Energy -= Mathf.Abs(rotateForce * COST_ROTATE * fixedDeltaTime * costMult);
        }

        private void ActMove(float costMult, float fixedDeltaTime, Vector2 forwardVector)
        {
            Vector2 moveVector = forwardVector * MOVESPEED * fixedDeltaTime;
            float forwardForce = Mathf.ClampNegPos(outForward.GetValue());
            forwardVector *= forwardForce;
            this.pos += forwardVector;
            Energy -= Mathf.Abs(forwardForce * COST_WALK * fixedDeltaTime * costMult);
        }

        private void ActStrafe(float costMult, float fixedDeltaTime, Vector2 forwardVector)
        {
            Vector2 strafeVector = new Vector2(forwardVector.Y, -forwardVector.X);
            strafeVector *= MOVESPEED * fixedDeltaTime;
            float strafeForce = Mathf.ClampNegPos(outStrafe.GetValue());
            strafeVector *= strafeForce;
            this.pos += strafeVector;
            Energy -= Mathf.Abs(strafeForce * COST_WALK * fixedDeltaTime * costMult);
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
                Energy -= eatWish * COST_EAT * fixedDeltaTime * costMult;
            }
        }

        private void Eat(float eatWish, Tile t, float fixedDeltaTime)
        {
            if(t.type != TileType.None)
            {
                float eatAmount = GAIN_EAT * eatWish * fixedDeltaTime;
                Energy += Simulation.TileMap.EatOfTile(t.position.X, t.position.Y, eatAmount);
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
            Creature child = new Creature(this, Manager);
            childIds.Add(child.id);
            currentTask.AddCreature(child);
            Energy -= STARTENERGY;
        }

        public bool IsAbleToGiveBirth()
        {
            return Energy > STARTENERGY + MINIMUMSURVIVALENERGY * 1.1f && Age > MINAGETOGIVEBIRTH;
        }

        public void CalculateFeelerPos(float feelerDistance)
        {
            float angle = feelerAngle + viewAngle;
            Vector2 localFeelerPos = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle)) * feelerDistance;
            feelerPos = pos + localFeelerPos;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void HandleCollisions()
        {
            feelerOcclusion = 0;
            feelerCreature = null;
            CalculateFeelerPos(MAXIMUMFEELERDISTANCE);
            for (int i = collisionGridX - 1; i <= collisionGridX + 1; i++)
            {
                for (int k = collisionGridY - 1; k <= collisionGridY + 1; k++)
                {
                    if (i >= 0 && k >= 0 && i < CreatureManager.COLLISIONGRIDSIZE && k < CreatureManager.COLLISIONGRIDSIZE)
                    {
                        List<Creature> collisionList = CreatureManager.GetCollisionGridList(i, k);
                        HandleCollisionsWithList(collisionList);
                    }
                }
            }
        }

        private void HandleCollisionsWithList(List<Creature> creatures)
        {
            for(int i = 0; i<creatures.Count; i++)
            {
                Creature c = creatures[i];
                if(c != null) HandleCollisionWithCreature(c);
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
                    feelerOcclusion = 1 - t;
                    feelerCreature = c;
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

        public Creature(SerializationInfo info, StreamingContext context)
        {
            id = info.GetInt64(nameof(id));
            pos = info.GetVector2(nameof(pos));
            viewAngle = info.GetSingle(nameof(viewAngle));
            feelerAngle = info.GetSingle(nameof(feelerAngle));
            Energy = info.GetSingle(nameof(Energy));
            age = info.GetSingle(nameof(age));
            generation = info.GetInt32(nameof(generation));
            Color = info.GetColor(nameof(Color));
            motherId = info.GetInt64(nameof(motherId));
            AmountOfMemory = info.GetInt32(nameof(AmountOfMemory));
            inMemory = new InputNeuron[AmountOfMemory];
            outMemory = new WorkingNeuron[AmountOfMemory];
            childIds = info.GetValue(nameof(childIds), typeof(List<long>)) as List<long>;
            brain = info.GetValue("brain", typeof(NeuronalNetwork)) as NeuronalNetwork;

        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(id), id);
            info.AddVector2(nameof(pos), pos);
            info.AddValue(nameof(viewAngle), viewAngle);
            info.AddValue(nameof(feelerAngle), feelerAngle);
            info.AddValue(nameof(Energy), Energy);
            info.AddValue(nameof(age), age);
            info.AddValue(nameof(generation), generation);
            info.AddColor(nameof(Color), Color);
            info.AddValue(nameof(motherId), motherId);
            info.AddValue(nameof(AmountOfMemory), AmountOfMemory);
            info.AddValue(nameof(childIds), childIds);
            info.AddValue(nameof(brain), brain);
        }

        public void SetupManager(CreatureManager manager)
        {
            Manager = manager;
            CalculateCollisionGridPos();
            SetupVariablesFromBrain();

        }

    }
}
