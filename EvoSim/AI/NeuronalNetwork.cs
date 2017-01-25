using EvoSim;
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
        public struct NetworkIndex
        {
            public NetworkIndex(int layer, int neuron, int connection)
            {
                LayerIndex = layer;
                NeuronIndex = neuron;
                ConnectionIndex = connection;
            }

            public static bool operator ==(NetworkIndex a, NetworkIndex b)
            {
                return a.ConnectionIndex == b.ConnectionIndex &&
                    a.LayerIndex == b.LayerIndex &&
                    a.NeuronIndex == b.NeuronIndex;
            }

            public static bool operator !=(NetworkIndex a, NetworkIndex b)
            {
                return !(a == b);
            }

            public int LayerIndex { get; set; }
            public int NeuronIndex { get; set; }
            public int ConnectionIndex { get; set; }
        }
        private bool fullMeshGenerated = false;

        List<List<Neuron>> neurons = new List<List<Neuron>>();

        public List<List<Neuron>> Neurons
        {
            get
            {
                return neurons;
            }
        }

        public NeuronalNetwork()
        {
            // Add list for Input and output layer
            neurons.Add(new List<Neuron>());
            neurons.Add(new List<Neuron>());
        }

        public List<Neuron> InputNeurons
        {
            get
            {
                if (neurons.Count > 0)
                {
                    return neurons[0];
                }
                else
                {
                    return null;
                }
            }
        }

        public List<Neuron> FirstHiddenLayer
        {
            get
            {
                if (neurons.Count > 2)
                {
                    return neurons[1];
                }
                else

                {
                    return null;
                }
            }
        }

        public List<Neuron> LastHiddenLayer
        {
            get
            {
                if (neurons.Count > 2)
                {
                    return neurons[neurons.Count - 2];
                }
                else

                {
                    return null;
                }
            }
        }

        public List<Neuron> OutputNeurons
        {
            get
            {
                if (neurons.Count > 1)
                {
                    return neurons.Last();
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="layerIndex">Zero based index of hidden layers</param>
        public List<Neuron> GetHiddenLayer(int layerIndex)
        {
            if (layerIndex < HiddenLayerCount && layerIndex >= 0)
            {
                return neurons[layerIndex + 1];
            }
            else
            {
                return null;
            }
        }

        public int HiddenLayerCount { get { return neurons.Count - 2; } }

        

        public void AddInputNeuron(InputNeuron neuron)
        {
            InputNeurons.Add(neuron);
        }
        public void AddHiddenNeuron(WorkingNeuron neuron, int Layer)
        {
            if (Layer+1 > 0 && Layer+1 < neurons.Count)
            {
                neurons[Layer+1].Add(neuron);
            }
            else
            {
                throw new ArgumentException("Layer needs to be within the hidden layer", "Layer");
            }
        }
        public void AddOutputNeuron(Neuron neuron)
        {
            OutputNeurons.Add(neuron);
        }

        public void AddInputNeuronAndMesh(InputNeuron neuron)
        {
            InputNeurons.Add(neuron);
            foreach (WorkingNeuron wn in FirstHiddenLayer)
            {
                wn.AddNeuronConnection(new Connection(neuron, 0));
            }
        }

        public void AddOutputNeuronAndMesh(WorkingNeuron neuron)
        {
            OutputNeurons.Add(neuron);
            foreach (WorkingNeuron wn in LastHiddenLayer)
            {
                neuron.AddNeuronConnection(new Connection(wn, 0));
            }
        }

        public void AddHiddenNeuronToLayerAndMesh(int layer)
        {
            WorkingNeuron neuron = new WorkingNeuron();
            neurons[layer].Add(neuron);
            foreach(Neuron n in neurons[layer - 1])
            {
                neuron.AddNeuronConnection(new Connection(n, 0));
            }
            foreach (WorkingNeuron n in neurons[layer + 1])
            {
                n.AddNeuronConnection(new Connection(neuron, 0));
            }
        }

        public void AddHiddenLayer(List<Neuron> layer)
        {
            neurons.Insert(neurons.Count - 1, layer);
        }

        public void RemoveInputNeuron(InputNeuron neuron)
        {
            InputNeurons.Remove(neuron);
            foreach (WorkingNeuron wn in neurons[1])
            {
                List<Connection> connectionsToRemove = new List<Connection>();
                foreach (Connection c in wn.GetConnections())
                {
                    if (c.entryNeuron == neuron)
                    {
                        connectionsToRemove.Add(c);
                    }
                }
                foreach (Connection c in connectionsToRemove)
                {
                    wn.GetConnections().Remove(c);
                }
            }
        }

        public void RemoveOutputNeuron(WorkingNeuron neuron)
        {
            OutputNeurons.Remove(neuron);
        }

        public InputNeuron GetInputNeuronFromIndex(int index)
        {
            return (InputNeuron)InputNeurons[index];
        }

        public InputNeuron GetInputNeuronFromName(String name)
        {
            foreach (InputNeuron neuron in InputNeurons)
            {
                if (name == neuron.GetName())
                {
                    return neuron;
                }
            }
            throw new NullReferenceException();
        }

        public WorkingNeuron GetHiddenNeuronFromIndex(int index, int Layer)
        {
            return (WorkingNeuron)neurons[Layer][index];
        }

        public WorkingNeuron GetHiddenNeuronFromName(string name, int Layer)
        {
            foreach (WorkingNeuron wn in neurons[Layer])
            {
                if (name == wn.GetName())
                {
                    return wn;
                }
            }
            return null;
        }

        public WorkingNeuron GetOutputNeuronFromIndex(int index)
        {
            return (WorkingNeuron)OutputNeurons[index];
        }

        public WorkingNeuron GetOutputNeuronFromName(String name)
        {
            foreach (WorkingNeuron wn in OutputNeurons)
            {
                if (name == wn.GetName())
                {
                    return wn;
                }
            }
            throw new NullReferenceException();
        }

        public void GenerateHiddenNeurons(int amount, int numLayers)
        {
            for (int layerIndex = 0; layerIndex < numLayers; layerIndex++)
            {
                List<Neuron> newNeurons = new List<Neuron>(amount);
                for (int i = 0; i < amount; i++)
                {
                    newNeurons.Add(new WorkingNeuron());
                }
                neurons.Insert(neurons.Count - 1, newNeurons); 
            }
        }

        public void GenerateFullMesh()
        {
            fullMeshGenerated = true;
            for (int layerIndex = 1; layerIndex < neurons.Count; layerIndex++)
            {
                foreach (WorkingNeuron wn in neurons[layerIndex])
                {
                    foreach (Neuron previous in neurons[layerIndex-1])
                    {
                        wn.AddNeuronConnection(previous, 1);
                    }
                }
            }
        }

        public void Invalidate()
        {
            for (int layerIndex = 1; layerIndex < neurons.Count; layerIndex++)
            {
                foreach (WorkingNeuron wn in neurons[layerIndex])
                {
                    if (wn != null)
                    {
                        wn.Invalidate();
                    }
                }
            }
        }

        public void RandomizeAllWeights()
        {
            for (int layerIndex = 1; layerIndex < neurons.Count; layerIndex++)
            {
                foreach (WorkingNeuron wn in neurons[layerIndex])
                {
                    wn.RandomizeWeights();
                }
            }
        }

        public void RandomMutation(float MutationRate)
        {
            int layer = Simulation.RandomInt(1, neurons.Count);
            int index = Simulation.RandomInt(neurons[layer].Count);
            ((WorkingNeuron)neurons[layer][index]).RandomMutation(MutationRate);
        }

        public Connection GetConnection(NetworkIndex index)
        {
            WorkingNeuron neuron = neurons[index.LayerIndex][index.NeuronIndex] as WorkingNeuron;
            if (neuron != null)
            {
                return neuron.GetConnections()[index.ConnectionIndex];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Mixes two networks together, meaning taking 50% of the 
        /// otherNetwork connections and set the weights of those in this network
        /// </summary>
        /// <param name="otherNetwork">The network to be mixed in</param>
        public void MixNetwork(NeuronalNetwork otherNetwork)
        {
            List<NetworkIndex> mixIndices = new List<NetworkIndex>();
            int totalCount = 0;
            var maxLayerIndex = Math.Min(neurons.Count, otherNetwork.neurons.Count);
            for (int layerIndex = 1; layerIndex < maxLayerIndex; layerIndex++)
            {

                int maxNeuronIndex = Math.Min(neurons[layerIndex].Count, otherNetwork.neurons[layerIndex].Count);
                for (int neuronIndex = 0; neuronIndex < maxNeuronIndex; neuronIndex++)
                {
                    totalCount += Math.Min(
                        ((WorkingNeuron)neurons[layerIndex][neuronIndex]).GetConnections().Count,
                        ((WorkingNeuron)otherNetwork.neurons[layerIndex][neuronIndex]).GetConnections().Count);
                }
            }
            while (mixIndices.Count < totalCount / 2)
            {
                NetworkIndex index = new NetworkIndex();
                do
                {
                    index.LayerIndex = Simulation.RandomInt(1, maxLayerIndex);
                    index.NeuronIndex = Simulation.RandomInt(Math.Min(neurons[index.LayerIndex].Count, otherNetwork.neurons[index.LayerIndex].Count));
                    index.ConnectionIndex = Simulation.RandomInt(Math.Min(
                        ((WorkingNeuron)neurons[index.LayerIndex][index.NeuronIndex]).GetConnections().Count,
                        ((WorkingNeuron)otherNetwork.neurons[index.LayerIndex][index.NeuronIndex]).GetConnections().Count));
                } while (mixIndices.Contains(index));
                mixIndices.Add(index);
            }

            foreach (NetworkIndex index in mixIndices)
            {
                Connection toBeChanged = GetConnection(index);
                Connection toBeUsed = otherNetwork.GetConnection(index);
                toBeChanged.weight = toBeUsed.weight;
            }
        }

        public NeuronalNetwork CloneFullMesh()
        {
            if (!fullMeshGenerated)
            {
                throw new NeuronalNetworkNotFullmeshedException();
            }
            NeuronalNetwork copy = new NeuronalNetwork();
            foreach (InputNeuron input in InputNeurons)
            {
                copy.AddInputNeuron((InputNeuron)input.NameCopy());
            }
            for (int layerIndex=0; layerIndex<HiddenLayerCount; layerIndex++)
            {
                List<Neuron> newLayer = new List<Neuron>();
                foreach (WorkingNeuron wn in neurons[layerIndex+1])
                {
                    newLayer.Add(wn.NameCopy());
                }
                copy.AddHiddenLayer(newLayer);
            }
            foreach (Neuron output in OutputNeurons)
            {
                copy.AddOutputNeuron(output.NameCopy());
            }

            copy.GenerateFullMesh();

            for (int layerIndex = 1; layerIndex < neurons.Count; layerIndex++)
            {
                for (int neuronIndex = 0; neuronIndex < neurons[layerIndex].Count; neuronIndex++)
                {
                    List<Connection> connectionsOrginal = ((WorkingNeuron)neurons[layerIndex][neuronIndex]).GetConnections();
                    List<Connection> connectionsCopy = ((WorkingNeuron)copy.neurons[layerIndex][neuronIndex]).GetConnections();
                    if (connectionsOrginal.Count != connectionsCopy.Count)
                    {
                        throw new NotSameAmountOfNeuronsException();
                    }
                    for (int k = 0; k < connectionsOrginal.Count; k++)
                    {
                        connectionsCopy[k].weight = connectionsOrginal[k].weight;
                    }
                }
            }

            return copy;
        }


    }
}
