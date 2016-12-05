using EvoSim;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvoNet.AI
{
    [Serializable]
    public class NeuronalNetwork
    {
        private bool fullMeshGenerated = false;
        private List<Neuron> inputNeurons = new List<Neuron>();
        public List<Neuron> InputNeurons
        {
            get { return inputNeurons; }
        }
        private List<Neuron> hiddenNeurons = new List<Neuron>();
        public List<Neuron> HiddenNeurons
        {
            get { return hiddenNeurons; }
        }
        private List<Neuron> outputNeurons = new List<Neuron>();
        public List<Neuron> OutputNeurons
        {
            get { return outputNeurons; }
        }

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

        public void AddInputNeuronAndMesh(InputNeuron neuron)
        {
            inputNeurons.Add(neuron);
            foreach(WorkingNeuron wn in hiddenNeurons)
            {
                wn.AddNeuronConnection(new Connection(neuron, 0));
            }
        }

        public void AddOutputNeuronAndMesh(WorkingNeuron neuron)
        {
            outputNeurons.Add(neuron);
            foreach(WorkingNeuron wn in hiddenNeurons)
            {
                neuron.AddNeuronConnection(new Connection(wn, 0));
            }
        }

        public void RemoveInputNeuron(InputNeuron neuron)
        {
            inputNeurons.Remove(neuron);
            foreach(WorkingNeuron wn in hiddenNeurons)
            {
                List<Connection> connectionsToRemove = new List<Connection>();
                foreach(Connection c in wn.GetConnections())
                {
                    if (c.entryNeuron == neuron)
                    {
                        connectionsToRemove.Add(c);
                    }
                }
                foreach(Connection c in connectionsToRemove)
                {
                    wn.GetConnections().Remove(c);
                }
            }
        }

        public void RemoveOutputNeuron(WorkingNeuron neuron)
        {
            outputNeurons.Remove(neuron);
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
            int index = Simulation.RandomInt(hiddenNeurons.Count + outputNeurons.Count);
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

        
    }
}
