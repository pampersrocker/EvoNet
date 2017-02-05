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
using EvoSim.Objects;

namespace EvoNet.Objects
{


    

    [Serializable]
    public class Creature
    {
        public int collisionGridX = 0;
        public int collisionGridY = 0;

        public const int CREATURESIZE = 54;

        private List<Feeler> feelers = new List<Feeler>();

        public int AmountOfFeelers
        {
            get
            {
                return feelers.Count;
            }
        }
        public List<Feeler> Feelers
        {
            get
            {
                return feelers;
            }
        }

        private static Creature _oldestCreatureEver = null; //dummy creature
        public static Creature oldestCreatureEver
        {
            get
            {
                return _oldestCreatureEver;
            }
        }

        public const float MINAGETOGIVEBIRTH = 4f;
        public const float COST_EAT = 1f;
        public const float COST_ATTACK = 10f;
        public const float GAIN_EAT = 100f;
        public const float DESTROYED_ATTACK = 110f;
        public const float GAIN_ATTACK = 50f;
        public const float COST_PERMANENT = 1f;
        public const float COST_WALK = 5f;
        public const float COST_ROTATE = 5f;
        public const float COST_PER_MEMORY_NEURON = 1.5f;

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
        public float ViewAngle
        {
            get
            {
                return viewAngle;
            }
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
        public const string NAME_IN_BIAS              = "bias";
        public const string NAME_IN_FOODVALUEPOSITION = "Food Value Position";
        public const string NAME_IN_FOODVALUEFEELER   = "Food Value Feeler";
        public const string NAME_IN_OCCLUSIONFEELER   = "Occlusion Feeler";
        public const string NAME_IN_ENERGY            = "Energy";
        public const string NAME_IN_AGE               = "Age";
        public const string NAME_IN_GENETICDIFFERENCE = "Genetic Difference";
        public const string NAME_IN_WASATTACKED       = "Was Attacked";
        public const string NAME_IN_WATERONFEELER     = "Water On Feeler";
        public const string NAME_IN_WATERONCREATURE   = "Water On Creature";
        //private const String NAME_IN_OSCILATION        = "Oscilation input";
        public const string NAME_IN_MEMORY            = "Input Memory #";

        public const string NAME_OUT_BIRTH       	        = "Birth";
        public const string NAME_OUT_ROTATE      	        = "Rotate";
        public const string NAME_OUT_FORWARD     	        = "Forward";
        public const string NAME_OUT_STRAFE      	        = "Strafe";
        public const string NAME_OUT_FEELERANGLE 	        = "Feeler Angle";
        public const string NAME_OUT_ATTACK      	        = "Attack";
        public const string NAME_OUT_EAT         	        = "Eat";
        public const string NAME_OUT_MATE_AGE 	 	        = "Mate Age";
        public const string NAME_OUT_MATE_ENERGY 	        = "Mate Energy";
        public const string NAME_OUT_MATE_GENERATION 		= "Mate Generation";
        public const string NAME_OUT_MATE_GENETICDIFFERENCE = "Mate Genetic Difference";
        public const string NAME_OUT_MATE_AGE_WEIGHT 	 	        = "Mate Age Weight";
        public const string NAME_OUT_MATE_ENERGY_WEIGHT 	        = "Mate Energy Weight";
        public const string NAME_OUT_MATE_GENERATION_WEIGHT 		= "Mate Generation Weight";
        public const string NAME_OUT_MATE_GENETICDIFFERENCE_WEIGHT = "Mate Genetic Difference Weight";

        //private const String NAME_OUT_OSCILATION  = "Oscilation output";
        public const string NAME_OUT_MEMORY      = "Output Memory #";

        private InputNeuron inBias              = new InputNeuron();
        private InputNeuron inFoodValuePosition = new InputNeuron();
        private InputNeuron inEnergy            = new InputNeuron();
        private InputNeuron inAge               = new InputNeuron();
        private InputNeuron inWasAttacked       = new InputNeuron();
        private InputNeuron inWaterOnCreature   = new InputNeuron();
        //private InputNeuron inOscilation        = new InputNeuron();
        private InputNeuron[] inMemory          = null;

        private WorkingNeuron outBirth       = new WorkingNeuron(-1);
        private WorkingNeuron outRotate      = new WorkingNeuron(-1);
        private WorkingNeuron outForward     = new WorkingNeuron(-1);
        private WorkingNeuron outStrafe      = new WorkingNeuron(-1);
        private WorkingNeuron outEat         = new WorkingNeuron(-1);
        private WorkingNeuron outMate_Age         = new WorkingNeuron(-1);
        private WorkingNeuron outMate_Energy         = new WorkingNeuron(-1);
        private WorkingNeuron outMate_Generation         = new WorkingNeuron(-1);
        private WorkingNeuron outMate_GeneticDifference = new WorkingNeuron(-1);
        private WorkingNeuron outMate_Age_Weight         = new WorkingNeuron(-1);
        private WorkingNeuron outMate_Energy_Weight         = new WorkingNeuron(-1);
        private WorkingNeuron outMate_Generation_Weight         = new WorkingNeuron(-1);
        private WorkingNeuron outMate_GeneticDifference_Weight = new WorkingNeuron(-1);
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

        private object amountOfHiddenLock = new object();
        private int amountOfHidden_ = 10;
        public int AmountOfHidden
        {
            get
            {
                lock (amountOfHiddenLock)
                {
                    return amountOfHidden_;
                }
            }
            set
            {
                lock (amountOfHiddenLock)
                {
                    amountOfHidden_ = value;
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
            inEnergy           .SetName(NAME_IN_ENERGY);
            inAge              .SetName(NAME_IN_AGE);
            inWasAttacked      .SetName(NAME_IN_WASATTACKED);
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
            outEat        .SetName(NAME_OUT_EAT);
            outMate_Age.SetName(NAME_OUT_MATE_AGE);
            outMate_Energy.SetName(NAME_OUT_MATE_ENERGY);
            outMate_Generation.SetName(NAME_OUT_MATE_GENERATION);
            outMate_GeneticDifference.SetName(NAME_OUT_MATE_GENETICDIFFERENCE);
            outMate_Age_Weight.SetName(NAME_OUT_MATE_AGE_WEIGHT);
            outMate_Energy_Weight.SetName(NAME_OUT_MATE_ENERGY_WEIGHT);
            outMate_Generation_Weight.SetName(NAME_OUT_MATE_GENERATION_WEIGHT);
            outMate_GeneticDifference_Weight.SetName(NAME_OUT_MATE_GENETICDIFFERENCE_WEIGHT);
            //outOscilation .SetName(NAME_OUT_OSCILATION);
            outMemory = new WorkingNeuron[AmountOfMemory];
            for(int i = 0; i<AmountOfMemory; i++)
            {
                outMemory[i] = new WorkingNeuron(-1);
                outMemory[i].SetName(NAME_OUT_MEMORY + (i + 1));
            }

            brain = new NeuronalNetwork();

            brain.AddInputNeuron(inBias);
            brain.AddInputNeuron(inFoodValuePosition);
            brain.AddInputNeuron(inEnergy);
            brain.AddInputNeuron(inAge);
            brain.AddInputNeuron(inWasAttacked);
            brain.AddInputNeuron(inWaterOnCreature);
            //brain.AddInputNeuron(inOscilation);
            for(int i = 0; i<AmountOfMemory; i++)
            {
                brain.AddInputNeuron(inMemory[i]);
            }

            brain.GenerateHiddenNeurons(AmountOfHidden, manager.simulation.SimulationConfiguration.NumberOfStartNeuronLayers);

            brain.AddOutputNeuron(outBirth);
            brain.AddOutputNeuron(outRotate);
            brain.AddOutputNeuron(outForward);
            brain.AddOutputNeuron(outStrafe);
            brain.AddOutputNeuron(outEat);
            brain.AddOutputNeuron(outMate_Age);
            brain.AddOutputNeuron(outMate_Energy);
            brain.AddOutputNeuron(outMate_Generation);
            brain.AddOutputNeuron(outMate_GeneticDifference);
            brain.AddOutputNeuron(outMate_Age_Weight);
            brain.AddOutputNeuron(outMate_Energy_Weight);
            brain.AddOutputNeuron(outMate_Generation_Weight);
            brain.AddOutputNeuron(outMate_GeneticDifference_Weight);
            //brain.AddOutputNeuron(outOscilation);
            for (int i = 0; i < AmountOfMemory; i++)
            {
                brain.AddOutputNeuron(outMemory[i]);
            }


            SetupFeelers(false, 1);

            brain.GenerateFullMesh();

            brain.RandomizeAllWeights();

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
            if(generation > manager.MaxGeneration)
            {
                manager.MaxGeneration = generation;
            }
            this.pos = mother.pos;
            this.viewAngle = Simulation.RandomFloat() * Mathf.PI * 2;
            this.brain = mother.brain.CloneFullMesh();

            AmountOfMemory = mother.AmountOfMemory;
            AmountOfHidden = mother.AmountOfHidden;
            inMemory = new InputNeuron[AmountOfMemory];
            outMemory = new WorkingNeuron[AmountOfMemory];

            SetupFeelers(true, mother.AmountOfFeelers);
            SetupVariablesFromBrain();

            DoMutations(mother);

            if(manager.SelectedCreature == null || manager.SelectedCreature.Energy < 100)
            {
                manager.SelectedCreature = this;
            }
        }

        public Creature(Creature mother, Creature father, CreatureManager manager) :
            this(manager)
        {
            id = currentId++;
            motherId = mother.id;
            generation = mother.generation + 1;
            if (generation > manager.MaxGeneration)
            {
                manager.MaxGeneration = generation;
            }
            pos = mother.pos;
            viewAngle = Simulation.RandomFloat() * Mathf.PI * 2;
            if(Simulation.SimulationConfiguration.MateBrainPercentage == 1.0f)
            {
                brain = father.brain.CloneFullMesh();
            }
            else
            {
                brain = mother.brain.CloneFullMesh();
                brain.MixNetwork(father.brain, Simulation.SimulationConfiguration.MateBrainPercentage);
            }

            AmountOfMemory = mother.AmountOfMemory;
            AmountOfHidden = mother.AmountOfHidden;
            inMemory = new InputNeuron[AmountOfMemory];
            outMemory = new WorkingNeuron[AmountOfMemory];

            SetupFeelers(true, mother.AmountOfFeelers);
            SetupVariablesFromBrain();


            DoMutations(mother);


            if (manager.SelectedCreature == null || manager.SelectedCreature.Energy <= 100)
            {
                manager.SelectedCreature = this;
            }
        }

        private void DoMutations(Creature mother)
        {
            if (Simulation.RandomFloat() < 0.01f)
            {
                MutateMemory();
            }

            if (Simulation.RandomFloat() < 0.01f && AmountOfFeelers < 5)
            {
                AddFeeler();
            }

            if (Simulation.RandomFloat() < Simulation.SimulationConfiguration.AddRemoveLayerPercentage)
            {
                MutateHiddenLayer();
            }


            if (Simulation.RandomFloat() < Simulation.SimulationConfiguration.AddHiddenNeuronPercentage && 
                AmountOfHidden < 40 * brain.HiddenLayerCount)
            {
                AddHiddenNeuron();
            }

            MutateConnections();

            MutateColor(mother);
            GenerateColorInv();
        }

        private void SetupFeelers(bool isChild, int count)
        {
            for(int i = 0; i<count; i++)
            {
                feelers.Add(new Feeler(this, i, isChild));
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
                WorkingNeuron newOut = new WorkingNeuron(-1);
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

        private void AddHiddenNeuron()
        {
            int index = 0;
            int count = brain.Neurons[1].Count;
            for (int hiddenIndex =1; hiddenIndex < brain.HiddenLayerCount; hiddenIndex++)
            {
                if (brain.Neurons[hiddenIndex+1].Count < count)
                {
                    index = hiddenIndex;
                    count = brain.Neurons[hiddenIndex + 1].Count;
                }
            }
            brain.AddHiddenNeuronToLayerAndMesh(index);
            AmountOfHidden++;
        }

        private void AddFeeler()
        {
            Feeler newFeeler = new Feeler(this, AmountOfFeelers, true);
            newFeeler.AddAndMesh();
            Feelers.Add(newFeeler);
        }

        private void MutateConnections()
        {
            for (int i = 0; i < 10 * brain.HiddenLayerCount; i++)
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
            for(int i = 0; i<AmountOfFeelers; i++)
            {
                feelers[i].SetupVariablesFromBrain(i);
            }
            inBias = brain.GetInputNeuronFromName(NAME_IN_BIAS);
            inFoodValuePosition = brain.GetInputNeuronFromName(NAME_IN_FOODVALUEPOSITION);
            inEnergy = brain.GetInputNeuronFromName(NAME_IN_ENERGY);
            inAge = brain.GetInputNeuronFromName(NAME_IN_AGE);
            inWasAttacked = brain.GetInputNeuronFromName(NAME_IN_WASATTACKED);
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
            outEat = brain.GetOutputNeuronFromName(NAME_OUT_EAT);

            outMate_Age = brain.GetOutputNeuronFromName(NAME_OUT_MATE_AGE);
            outMate_Energy = brain.GetOutputNeuronFromName(NAME_OUT_MATE_ENERGY);
            outMate_Generation = brain.GetOutputNeuronFromName(NAME_OUT_MATE_GENERATION);
            outMate_GeneticDifference = brain.GetOutputNeuronFromName(NAME_OUT_MATE_GENETICDIFFERENCE);

            outMate_Age_Weight = brain.GetOutputNeuronFromName(NAME_OUT_MATE_AGE_WEIGHT);
            outMate_Energy_Weight = brain.GetOutputNeuronFromName(NAME_OUT_MATE_ENERGY_WEIGHT);
            outMate_Generation_Weight = brain.GetOutputNeuronFromName(NAME_OUT_MATE_GENERATION_WEIGHT);
            outMate_GeneticDifference_Weight = brain.GetOutputNeuronFromName(NAME_OUT_MATE_GENETICDIFFERENCE_WEIGHT);
            //outOscilation = brain.GetOutputNeuronFromName(NAME_OUT_OSCILATION);
            for (int i = 0; i<AmountOfMemory; i++)
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

            inBias.SetValue(1);
            inFoodValuePosition.SetValue(creatureTile.food / TileMap.MAXIMUMFOODPERTILE);
            for(int i = 0; i<AmountOfFeelers; i++)
            {
                feelers[i].ReadSensors(Simulation);
            }
            inEnergy.SetValue((Energy - MINIMUMSURVIVALENERGY) / (STARTENERGY - MINIMUMSURVIVALENERGY));
            inAge.SetValue(age / 10f);
            inWasAttacked.SetValue(Mathf.Clamp01(1 - TimeSinceThisWasAttacked));
            inWaterOnCreature.SetValue(creatureTile.IsLand() ? 0 : 1);
            //inOscilation.SetValue(Mathf.Sin(OscilationValue));

        }

        [NonSerialized]
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
            for(int i = 0; i<AmountOfFeelers; i++)
            {
                feelers[i].ActAttack(costMult, fixedDeltaTime);
            }

            //OscilationValue += outOscilation.GetValue();

            Energy -= COST_PERMANENT * fixedDeltaTime * costMult;
            Energy -= COST_PER_MEMORY_NEURON * fixedDeltaTime * costMult * AmountOfMemory;

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
            for(int i = 0; i<AmountOfFeelers; i++)
            {
                feelers[i].ActFeelerRotate();
            }
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

        private Creature FindMate()
        {
            Creature father = this;
            float fatherDif = float.MaxValue;
            float mateAgeWeight = (outMate_Age_Weight.GetValue() + 1) / 2;
            float mateEnergyWeight = (outMate_Energy_Weight.GetValue() + 1) / 2;
            float mateGenerationWeight = (outMate_Generation_Weight.GetValue() + 1) / 2;
            float mateDifferenceWeight = (outMate_GeneticDifference_Weight.GetValue() + 1) / 2;
            float mateAge = (outMate_Age.GetValue() + 1) / 2;
            float mateEnergy = (outMate_Energy.GetValue() + 1) / 2;
            float mateGeneration = (outMate_Generation.GetValue() + 1) / 2;
            float mateDifference = (outMate_GeneticDifference.GetValue() + 1) / 2;



            float maxAge = 0;
            float minAge = 0;
            float maxEnergy = 0;
            float minEnergy = 0;
            float maxGeneration = 0;
            float minGeneration = 0;
            float maxDifference = 0;
            float minDifference = 0;

            manager.Creatures.ForEach((creature) =>
            {
                var difference = CalculateGeneticDifferencToCreature(creature);
                maxAge = Mathf.Max(maxAge, creature.Age);
                maxEnergy = Mathf.Max(maxEnergy, creature.Energy);
                maxGeneration = Mathf.Max(maxGeneration, creature.Generation);
                maxDifference = Mathf.Max(maxDifference, difference);
                minAge = Mathf.Min(minAge, creature.Age);
                minEnergy = Mathf.Min(minEnergy, creature.Energy);
                minGeneration = Mathf.Min(minGeneration, creature.Generation);
                minDifference = Mathf.Min(minDifference, difference);
            });



            manager.Creatures.ForEach((creature) =>
            {
                if (creature == this)
                {
                    return;
                }
        //Calculate the closest difference to the desired weighted attributes
        float difAge = Mathf.Abs((creature.Age - minAge) / (maxAge - minAge) - mateAge) * mateAgeWeight;
                float difEnergy = Mathf.Abs((creature.Energy - minEnergy) / (maxEnergy - minEnergy) - mateEnergy) * mateEnergyWeight;
                float difGeneration = Mathf.Abs((creature.Generation - minGeneration) / (maxGeneration - minGeneration) - mateGeneration) * mateGenerationWeight;
                float difDifference = Mathf.Abs((CalculateGeneticDifferencToCreature(creature) - minDifference) / (maxDifference - minDifference) - mateDifference) * mateDifferenceWeight;
                float totalDif = difAge + difEnergy + difGeneration + difDifference;
                if (totalDif < fatherDif)
                {
                    fatherDif = totalDif;
                    father = creature;
                }
            });
            return father;
        }

        public void GiveBirth()
        {
            Creature child = null;
            if (Simulation.SimulationConfiguration.UseMate)
            {
                Creature mate = FindMate();
                child = new Creature(this, mate, Manager);
            }
            else
            {
                child = new Creature(this, Manager);
            }
            childIds.Add(child.id);
            currentTask.AddCreature(child);
            Energy -= STARTENERGY;
        }

        public bool IsAbleToGiveBirth()
        {
            return Energy > STARTENERGY + MINIMUMSURVIVALENERGY * 1.1f && Age > MINAGETOGIVEBIRTH;
        }

        private float CalculateGeneticDifferencToCreature(Creature other)
        {
            Vector3 vec = new Vector3(
                Color.R - other.Color.R,
                Color.G - other.Color.G,
                Color.B - other.Color.B
                );

            vec /= 255f;
            return vec.Length();
        }



        [MethodImpl(MethodImplOptions.Synchronized)]
        public void HandleCollisions()
        {
            for (int i = 0; i < AmountOfFeelers; i++)
            {
                feelers[i].HandleCollisions();
            }
        }

        public void SetupManager(CreatureManager manager)
        {
            Manager = manager;
            CalculateCollisionGridPos();
            SetupVariablesFromBrain();

        }

        private void MutateHiddenLayer()
        {
            bool add = Simulation.RandomFloat() < 0.5f;
            if (add)
            {
                brain.CreateHiddenLayer();
                manager.SelectedCreature = this;
            }
            else
            {
                brain.RemoveHiddenLayer();
            }
        }

    }
}
