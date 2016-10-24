using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvoNet.AI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EvoNet.Objects
{
    public class Creature
    {
        private const float STARTENERGY = 150;
        private const float MINIMUMSURVIVALENERGY = 100;

        private static SpriteBatch spriteBatch = null;
        private static Texture2D bodyTex = null;
        private static Texture2D feelerTex = null;

        private Vector2 pos;
        private float viewAngle;

        private float feelerAngle;
        private Vector2 feelerPos;

        private float energy = 150;
        private float age = 0;

        private NeuronalNetwork brain;

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

        private const String NAME_OUT_BIRTH       = "Birth";
        private const String NAME_OUT_ROTATE      = "Rotate";
        private const String NAME_OUT_FORWARD     = "Forward";
        private const String NAME_OUT_FEELERANGLE = "Feeler Angle";
        private const String NAME_OUT_ATTACK      = "Attack";
        private const String NAME_OUT_EAT         = "Eat";

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

        private WorkingNeuron outBirth       = new WorkingNeuron();
        private WorkingNeuron outRotate      = new WorkingNeuron();
        private WorkingNeuron outForward     = new WorkingNeuron();
        private WorkingNeuron outFeelerAngle = new WorkingNeuron();
        private WorkingNeuron outAttack      = new WorkingNeuron();
        private WorkingNeuron outEat         = new WorkingNeuron();

        public Creature(Vector2 pos, float viewAngle)
        {
            if(spriteBatch == null)
            {
                spriteBatch = new SpriteBatch(EvoGame.Instance.GraphicsDevice);
                bodyTex = EvoGame.Instance.Content.Load<Texture2D>("Map/SandTexture");
                feelerTex = EvoGame.Instance.Content.Load<Texture2D>("Map/SandTexture");
            }
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

            outBirth      .SetName(NAME_OUT_BIRTH);
            outRotate     .SetName(NAME_OUT_ROTATE);
            outForward    .SetName(NAME_OUT_FORWARD);
            outFeelerAngle.SetName(NAME_OUT_FEELERANGLE);
            outAttack     .SetName(NAME_OUT_ATTACK);
            outEat        .SetName(NAME_OUT_EAT);

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

            brain.GenerateHiddenNeurons(10);

            brain.AddOutputNeuron(outBirth);
            brain.AddOutputNeuron(outRotate);
            brain.AddOutputNeuron(outForward);
            brain.AddOutputNeuron(outFeelerAngle);
            brain.AddOutputNeuron(outAttack);
            brain.AddOutputNeuron(outEat);

            brain.GenerateFullMesh();

            brain.RandomizeAllWeights();
            CalculateFeelerPos();

        }

        public Creature(Creature mother)
        {
            this.pos = mother.pos;
            this.viewAngle = (float)EvoGame.GlobalRandom.NextDouble() * Mathf.PI * 2;
            this.brain = mother.brain.CloneFullMesh();

            inBias              = brain.GetInputNeuronFromName(NAME_IN_BIAS);
            inFoodValuePosition = brain.GetInputNeuronFromName(NAME_IN_FOODVALUEPOSITION);
            inFoodValueFeeler   = brain.GetInputNeuronFromName(NAME_IN_FOODVALUEFEELER);
            inOcclusionFeeler   = brain.GetInputNeuronFromName(NAME_IN_OCCLUSIONFEELER);
            inEnergy            = brain.GetInputNeuronFromName(NAME_IN_ENERGY);
            inAge               = brain.GetInputNeuronFromName(NAME_IN_AGE);
            inGeneticDifference = brain.GetInputNeuronFromName(NAME_IN_GENETICDIFFERENCE);
            inWasAttacked       = brain.GetInputNeuronFromName(NAME_IN_WASATTACKED);
            inWaterOnFeeler     = brain.GetInputNeuronFromName(NAME_IN_WATERONFEELER);
            inWaterOnCreature   = brain.GetInputNeuronFromName(NAME_IN_WATERONCREATURE);

            outBirth       = brain.GetOutputNeuronFromName(NAME_OUT_BIRTH);
            outRotate      = brain.GetOutputNeuronFromName(NAME_OUT_ROTATE);
            outForward     = brain.GetOutputNeuronFromName(NAME_OUT_FORWARD);
            outFeelerAngle = brain.GetOutputNeuronFromName(NAME_OUT_FEELERANGLE);
            outAttack      = brain.GetOutputNeuronFromName(NAME_OUT_ATTACK);
            outEat         = brain.GetOutputNeuronFromName(NAME_OUT_EAT);

            CalculateFeelerPos();
            //TODO mutate
        }

        public void ReadSensors()
        {
            brain.Invalidate();

            inBias.SetValue(1);
            inFoodValuePosition.SetValue(0); //TODO find real value
            inFoodValueFeeler.SetValue(0); //TODO find real value
            inOcclusionFeeler.SetValue(0); //TODO find real value
            inEnergy.SetValue((energy - MINIMUMSURVIVALENERGY) / (STARTENERGY - MINIMUMSURVIVALENERGY));
            inAge.SetValue(age);
            inGeneticDifference.SetValue(0); //TODO find real value
            inWasAttacked.SetValue(0); //TODO find real value
            inWaterOnFeeler.SetValue(0); //TODO find real value
            inWaterOnCreature.SetValue(0); //TODO find real value
        }

        public void Act()
        {
            float rotateForce = Mathf.ClampNegPos(outRotate.GetValue());
            this.viewAngle += rotateForce / 10;

            Vector2 forwardVector = new Vector2(Mathf.Sin(viewAngle), Mathf.Cos(viewAngle));
            float forwardForce = Mathf.ClampNegPos(outForward.GetValue());
            forwardVector *= forwardForce;
            this.pos += forwardVector;

            float birthWish = outBirth.GetValue();
            if (birthWish > 0) TryToGiveBirth();

            feelerAngle = Mathf.ClampNegPos(outFeelerAngle.GetValue()) * Mathf.PI;
            CalculateFeelerPos();
            //TODO implement Attack
            //TODO implement Eat
            //TODO implement Action Costs
            //TODO increase Age
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
            EvoGame.Creatures.Add(new Creature(this));
            energy -= STARTENERGY;
        }

        public bool IsAbleToGiveBirth()
        {
            return energy > STARTENERGY + MINIMUMSURVIVALENERGY * 1.1f;
        }

        public void CalculateFeelerPos()
        {
            float angle = feelerAngle + viewAngle;
            Vector2 localFeelerPos = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle)) * 100;
            feelerPos = pos + localFeelerPos;
        }

        public void Draw()
        {
            //TODO change that quick and dirty solution
            spriteBatch.Begin(transformMatrix: Camera.instanceGameWorld.Matrix);
            spriteBatch.Draw(bodyTex, new Rectangle((int)pos.X - 25, (int)pos.Y - 25, 50, 50), Color.Red);
            spriteBatch.Draw(feelerTex, new Rectangle((int)feelerPos.X - 5, (int)feelerPos.Y - 5, 10, 10), Color.Blue);
            //TODO draw line between body and feelerpos
            spriteBatch.End();
        }
    }
}
