using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvoNet.AI
{
    class NeuronalNetwork
    {
        private bool fullMeshGenerated = false;
        private List<InputNeuron> inputNeurons = new List<InputNeuron>();
        private List<WorkingNeuron> hiddenNeurons = new List<WorkingNeuron>();
        private List<WorkingNeuron> outputNeurons = new List<WorkingNeuron>();


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
            return inputNeurons[index];
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
            return hiddenNeurons[index];
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
            return outputNeurons[index];
        }

        public WorkingNeuron GetOutputNeuronFromName(String name)
        {
            foreach (WorkingNeuron wn in hiddenNeurons)
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
                List<Connection> connectionsOrginal = hiddenNeurons[i].GetConnections();
                List<Connection> connectionsCopy = copy.hiddenNeurons[i].GetConnections();
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
                List<Connection> connectionsOrginal = outputNeurons[i].GetConnections();
                List<Connection> connectionsCopy = copy.outputNeurons[i].GetConnections();
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
