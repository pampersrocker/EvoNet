using EvoNet;
using EvoNet.AI;
using EvoNet.Map;
using EvoNet.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EvoSim.Objects
{
    [Serializable]
    public class Feeler
    {
        public const int FEELERTIPSIZE = 10;
        private const float MAXIMUMFEELERDISTANCE = 100;


        private float feelerOcclusion = 0;
        private float feelerAngle;
        private Vector2 feelerPos;
        public Vector2 FeelerPos
        {
            get { return feelerPos; }
        }


        private InputNeuron inFoodValueFeeler = new InputNeuron();
        private InputNeuron inOcclusionFeeler = new InputNeuron();
        private InputNeuron inWaterOnFeeler = new InputNeuron();
        private InputNeuron inGeneticDifference = new InputNeuron();

        private WorkingNeuron outFeelerAngle = new WorkingNeuron(-1);
        private WorkingNeuron outAttack = new WorkingNeuron(-1);

        [NonSerialized]
        private Creature feelerCreature = null;
        private Creature owner = null;


        private float timeSinceLastAttack = 0;
        public float TimeSinceLastAttack
        {
            get { return timeSinceLastAttack; }
        }
        public const float TIMEBETWEENATTACKS = 0.1f;

        public Feeler(Creature owner, int index, bool isChild)
        {
            this.owner = owner;

            inFoodValueFeeler.SetName(Creature.NAME_IN_FOODVALUEFEELER + " #" + index);
            inOcclusionFeeler.SetName(Creature.NAME_IN_OCCLUSIONFEELER + " #" + index);
            inWaterOnFeeler.SetName(Creature.NAME_IN_WATERONFEELER + " #" + index);
            inGeneticDifference.SetName(Creature.NAME_IN_GENETICDIFFERENCE + " #" + index);

            outFeelerAngle.SetName(Creature.NAME_OUT_FEELERANGLE + " #" + index);
            outAttack.SetName(Creature.NAME_OUT_ATTACK + " #" + index);

            if (!isChild)
            {
                owner.Brain.AddInputNeuron(inFoodValueFeeler);
                owner.Brain.AddInputNeuron(inOcclusionFeeler);
                owner.Brain.AddInputNeuron(inWaterOnFeeler);
                owner.Brain.AddInputNeuron(inGeneticDifference);

                owner.Brain.AddOutputNeuron(outFeelerAngle);
                owner.Brain.AddOutputNeuron(outAttack);
            }


            CalculateFeelerPos(MAXIMUMFEELERDISTANCE);
        }

        public void CalculateFeelerPos(float feelerDistance)
        {
            float angle = feelerAngle + owner.ViewAngle;
            Vector2 localFeelerPos = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle)) * feelerDistance;
            feelerPos = owner.Pos + localFeelerPos;
        }

        public void SetupVariablesFromBrain(int index)
        {
            inFoodValueFeeler = owner.Brain.GetInputNeuronFromName(Creature.NAME_IN_FOODVALUEFEELER + " #" + index);
            inOcclusionFeeler = owner.Brain.GetInputNeuronFromName(Creature.NAME_IN_OCCLUSIONFEELER + " #" + index);
            inWaterOnFeeler = owner.Brain.GetInputNeuronFromName(Creature.NAME_IN_WATERONFEELER + " #" + index);
            inGeneticDifference = owner.Brain.GetInputNeuronFromName(Creature.NAME_IN_GENETICDIFFERENCE + " #" + index);

            outFeelerAngle = owner.Brain.GetOutputNeuronFromName(Creature.NAME_OUT_FEELERANGLE + " #" + index);
            outAttack = owner.Brain.GetOutputNeuronFromName(Creature.NAME_OUT_ATTACK + " #" + index);
        }

        private float CalculateGeneticDifferencToFeelerCreature(Color ownColor)
        {
            if (feelerCreature == null)
            {
                return 0;
            }
            Vector3 vec = new Vector3(
                ownColor.R - feelerCreature.Color.R,
                ownColor.G - feelerCreature.Color.G,
                ownColor.B - feelerCreature.Color.B
                );

            vec /= 255f;
            return vec.Length();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void ReadSensors(Simulation Simulation)
        {
            Tile feelerTile = Simulation.TileMap.GetTileAtWorldPosition(feelerPos);

            inFoodValueFeeler.SetValue(feelerTile.food / TileMap.MAXIMUMFOODPERTILE);
            inOcclusionFeeler.SetValue(feelerOcclusion);
            inWaterOnFeeler.SetValue(feelerTile.IsLand() ? 0 : 1);
            inGeneticDifference.SetValue(CalculateGeneticDifferencToFeelerCreature(owner.Color));
        }

        public void ActAttack(float costMult, float fixedDeltaTime)
        {
            if (outAttack.GetValue() > 0 && timeSinceLastAttack > TIMEBETWEENATTACKS)
            {
                timeSinceLastAttack = 0;
                owner.Energy -= Creature.COST_ATTACK;
                if (feelerCreature != null)
                {
                    feelerCreature.Energy -= Creature.DESTROYED_ATTACK;
                    feelerCreature.TimeSinceThisWasAttacked = 0;
                    owner.Energy += Creature.GAIN_ATTACK;
                }
            }


            timeSinceLastAttack += fixedDeltaTime;
        }

        public void ActFeelerRotate()
        {
            feelerAngle = Mathf.ClampNegPos(outFeelerAngle.GetValue()) * Mathf.PI;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void HandleCollisions()
        {
            feelerOcclusion = 0;
            feelerCreature = null;
            CalculateFeelerPos(MAXIMUMFEELERDISTANCE);
            for (int i = owner.collisionGridX - 1; i <= owner.collisionGridX + 1; i++)
            {
                for (int k = owner.collisionGridY - 1; k <= owner.collisionGridY + 1; k++)
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
            for (int i = 0; i < creatures.Count; i++)
            {
                Creature c = creatures[i];
                if (c != null) HandleCollisionWithCreature(c);
            }
        }

        private void HandleCollisionWithCreature(Creature c)
        {
            if (owner == c) return;
            if (feelerOcclusion != 1) HandleCollisionWithCreatureEye(c);
        }

        private void HandleCollisionWithCreatureEye(Creature c)
        {
            for (float t = 0; t <= 1 - feelerOcclusion; t += 0.1f)
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
            float dist = (this.feelerPos - c.Pos).LengthSquared();
            float minDist = (FEELERTIPSIZE + Creature.CREATURESIZE) / 2;
            minDist *= minDist;
            return dist < minDist;
        }

        public void AddAndMesh()
        {
            owner.Brain.AddInputNeuronAndMesh(inFoodValueFeeler);
            owner.Brain.AddInputNeuronAndMesh(inOcclusionFeeler);
            owner.Brain.AddInputNeuronAndMesh(inWaterOnFeeler);
            owner.Brain.AddInputNeuronAndMesh(inGeneticDifference);

            owner.Brain.AddOutputNeuronAndMesh(outFeelerAngle);
            owner.Brain.AddOutputNeuronAndMesh(outAttack);
        }
    }
}
