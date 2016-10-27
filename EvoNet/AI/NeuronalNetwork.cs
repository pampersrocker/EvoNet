using EvoNet.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvoNet.AI
{
    public class NeuronalNetwork
    {
        private bool fullMeshGenerated = false;
        private List<Neuron> inputNeurons = new List<Neuron>();
        private List<Neuron> hiddenNeurons = new List<Neuron>();
        private List<Neuron> outputNeurons = new List<Neuron>();


        public void AddInputNeuron(InputNeuron neuron)
        {
            inputNeurons.Add(neuron);
        }
        public void AddHiddenNeuron(WorkingNeuron neuron)
        {
            hiddenNeurons.Add(neuron);
        }
        public void AddOutputNeuron(WorkingNeuron neuron)
        {
            outputNeurons.Add(neuron);
        }

        public InputNeuron GetInputNeuronFromIndex(int index)
        {
            return (InputNeuron)inputNeurons[index];
        }

        public InputNeuron GetInputNeuronFromName(String name)
        {
            foreach(InputNeuron neuron in inputNeurons)
            {
                if(name == neuron.GetName())
                {
                    return neuron;
                }
            }
            return null;
        }

        public WorkingNeuron GetHiddenNeuronFromIndex(int index)
        {
            return (WorkingNeuron)hiddenNeurons[index];
        }

        public WorkingNeuron GetHiddenNeuronFromName(String name)
        {
            foreach(WorkingNeuron wn in hiddenNeurons)
            {
                if(name == wn.GetName())
                {
                    return wn;
                }
            }
            return null;
        }

        public WorkingNeuron GetOutputNeuronFromIndex(int index)
        {
            return (WorkingNeuron)outputNeurons[index];
        }

        public WorkingNeuron GetOutputNeuronFromName(String name)
        {
            foreach (WorkingNeuron wn in outputNeurons)
            {
                if (name == wn.GetName())
                {
                    return wn;
                }
            }
            return null;
        }

        public void GenerateHiddenNeurons(int amount)
        {
            for(int i = 0; i < amount; i++)
            {
                hiddenNeurons.Add(new WorkingNeuron());
            }
        }

        public void GenerateFullMesh()
        {
            fullMeshGenerated = true;
            foreach (WorkingNeuron wn in hiddenNeurons)
            {
                foreach (InputNeuron input in inputNeurons)
                {
                    wn.AddNeuronConnection(input, 1);
                }
            }

            foreach (WorkingNeuron wn in outputNeurons)
            {
                foreach (WorkingNeuron wn2 in hiddenNeurons)
                {
                    wn.AddNeuronConnection(wn2, 1);
                }
            }
        }

        public void Invalidate()
        {
            foreach(WorkingNeuron wn in hiddenNeurons)
            {
                wn.Invalidate();
            }
            foreach(WorkingNeuron wn in outputNeurons)
            {
                wn.Invalidate();
            }
        }

        public void RandomizeAllWeights()
        {
            foreach (WorkingNeuron wn in hiddenNeurons)
            {
                wn.RandomizeWeights();
            }
            foreach (WorkingNeuron wn in outputNeurons)
            {
                wn.RandomizeWeights();
            }
        }

        public void RandomMutation(float MutationRate)
        {
            int index = EvoGame.GlobalRandom.Next(hiddenNeurons.Count + outputNeurons.Count);
            if(index < hiddenNeurons.Count)
            {
                ((WorkingNeuron)hiddenNeurons[index]).RandomMutation(MutationRate);
            }
            else
            {
                ((WorkingNeuron)outputNeurons[index - hiddenNeurons.Count]).RandomMutation(MutationRate);
            }
        }

        public NeuronalNetwork CloneFullMesh()
        {
            //TODO make this mess pretty
            if (!this.fullMeshGenerated)
            {
                throw new NeuronalNetworkNotFullmeshedException();
            }
            NeuronalNetwork copy = new NeuronalNetwork();
            foreach (InputNeuron input in inputNeurons)
            {
                copy.AddInputNeuron((InputNeuron)input.NameCopy());
            }
            foreach (WorkingNeuron wn in hiddenNeurons)
            {
                copy.AddHiddenNeuron((WorkingNeuron)wn.NameCopy());
            }
            foreach (WorkingNeuron wn in outputNeurons)
            {
                copy.AddOutputNeuron((WorkingNeuron)wn.NameCopy());
            }

            copy.GenerateFullMesh();

            for (int i = 0; i < hiddenNeurons.Count; i++)
            {
                List<Connection> connectionsOrginal = ((WorkingNeuron)hiddenNeurons[i]).GetConnections();
                List<Connection> connectionsCopy = ((WorkingNeuron)copy.hiddenNeurons[i]).GetConnections();
                if (connectionsOrginal.Count != connectionsCopy.Count)
                {
                    throw new NotSameAmountOfNeuronsException();
                }
                for (int k = 0; k < connectionsOrginal.Count; k++)
                {
                    connectionsCopy[k].weight = connectionsOrginal[k].weight;
                }
            }
            for (int i = 0; i < outputNeurons.Count; i++)
            {
                List<Connection> connectionsOrginal = ((WorkingNeuron)outputNeurons[i]).GetConnections();
                List<Connection> connectionsCopy = ((WorkingNeuron)copy.outputNeurons[i]).GetConnections();
                if (connectionsOrginal.Count != connectionsCopy.Count)
                {
                    throw new NotSameAmountOfNeuronsException();
                }
                for (int k = 0; k < connectionsOrginal.Count; k++)
                {
                    connectionsCopy[k].weight = connectionsOrginal[k].weight;
                }
            }

            return copy;
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(fullMeshGenerated);
            writer.Write(inputNeurons.Count);
            foreach (InputNeuron inputNeuron in inputNeurons)
            {
                inputNeuron.Serialize(writer);
            }
            writer.Write(hiddenNeurons.Count);
            foreach (WorkingNeuron hiddenNeuron in hiddenNeurons)
            {
                hiddenNeuron.Serialize(writer);
                writer.Write(hiddenNeuron.GetConnections().Count);
                foreach (Connection connection in hiddenNeuron.GetConnections())
                {
                    // Todo get rid of this covariance cast
                    connection.Serialize(writer, inputNeurons.Cast<Neuron>());
                }
            }
            writer.Write(outputNeurons.Count);
            foreach (WorkingNeuron outNeuron in outputNeurons)
            {
                outNeuron.Serialize(writer);
                writer.Write(outNeuron.GetConnections().Count);
                foreach (Connection connection in outNeuron.GetConnections())
                {
                    // Todo get rid of this covariance cast
                    connection.Serialize(writer, hiddenNeurons.Cast<Neuron>());
                }
            }
        }

        public void Deserialize(BinaryReader reader)
        {
            fullMeshGenerated = reader.ReadBoolean();
            int inputCount = reader.ReadInt32();
            for (int currentIndex = 0; currentIndex < inputCount; currentIndex++)
            {
                InputNeuron newNeuron = new InputNeuron();
                newNeuron.Deserialize(reader);
                inputNeurons.Add(newNeuron);
            }
            int hiddenCount = reader.ReadInt32();
            for (int currentIndex = 0; currentIndex < hiddenCount; currentIndex++)
            {
                WorkingNeuron newNeuron = new WorkingNeuron();
                newNeuron.Deserialize(reader);
                hiddenNeurons.Add(newNeuron);

                int connectionCount = reader.ReadInt32();
                for (int connectionIndex = 0; connectionIndex < connectionCount; connectionIndex++)
                {
                    // Todo get rid of this covariance cast
                    Connection connection = Connection.Deserialize(reader, inputNeurons.Cast<Neuron>());
                    newNeuron.AddNeuronConnection(connection);
                }
            }
            int outputCount = reader.ReadInt32();
            for (int currentIndex = 0; currentIndex < outputCount; currentIndex++)
            {
                WorkingNeuron newNeuron = new WorkingNeuron();
                newNeuron.Deserialize(reader);
                outputNeurons.Add(newNeuron);

                int connectionCount = reader.ReadInt32();
                for (int connectionIndex = 0; connectionIndex < connectionCount; connectionIndex++)
                {
                    // Todo get rid of this covariance cast
                    Connection connection = Connection.Deserialize(reader, hiddenNeurons.Cast<Neuron>());
                    newNeuron.AddNeuronConnection(connection);
                }
            }
            Invalidate();
        }


        private const float NEURONSIZE = 15;

        public void Draw(SpriteBatch spriteBatch, Rectangle rect)
        {
            float yMin = rect.Y + NEURONSIZE / 2;
            float yMax = rect.Y + rect.Height - NEURONSIZE / 2;
            DrawLayer(spriteBatch, rect.X + NEURONSIZE / 2, yMin, yMax, inputNeurons, new Vector2(-10, -10), true);
            DrawLayer(spriteBatch, rect.X + rect.Width / 2, yMin, yMax, hiddenNeurons);
            DrawLayer(spriteBatch, rect.X + rect.Width - NEURONSIZE / 2, yMin, yMax, outputNeurons, new Vector2(10, -10));
        }

        private void DrawLayer(SpriteBatch spriteBatch, float x, float yMin, float yMax, List<Neuron> layer, Vector2? nameOffset = null, bool writeRight = false) {
            float yDiff = yMax - yMin;
            float distanceBetweenNeurons = yDiff / (layer.Count - 1);
            float currentY = yMin;

            for(int i = 0; i<layer.Count; i++)
            {
                DrawNeuron(spriteBatch, x, currentY, layer[i], nameOffset, writeRight);
                currentY += distanceBetweenNeurons;
            }
        }

        private void DrawNeuron(SpriteBatch spriteBatch, float x, float y, Neuron n, Vector2? nameOffset = null, bool writeRight = false)
        {
            Color c = Color.Black;
            float val = n.GetValue();
            if(val < 0)
            {
                c = Color.Red;
            }
            else
            {
                c = Color.Blue;
            }

            float valSize = val * NEURONSIZE;

            RenderHelper.DrawCircle(spriteBatch, x, y, NEURONSIZE / 2 + 1, Color.White);
            RenderHelper.DrawCircle(spriteBatch, x, y, valSize / 2, c);

            if (nameOffset != null)
            {
                Vector2 pos = new Vector2(x, y) + (Vector2)nameOffset;
                if (writeRight)
                {
                    pos.X -= Fonts.FontArial.MeasureString(n.GetName()).X;
                }
                spriteBatch.DrawString(Fonts.FontArial, n.GetName(), pos, Color.White);
            }
        }
    }
}
