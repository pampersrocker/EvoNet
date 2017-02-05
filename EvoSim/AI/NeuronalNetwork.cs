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
            if (Layer + 1 > 0 && Layer + 1 < neurons.Count)
            {
                neurons[Layer + 1].Add(neuron);
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
            layer += 1;
            WorkingNeuron neuron = new WorkingNeuron(layer);
            neurons[layer].Add(neuron);
            int newIndex = neurons[layer].Count - 1;
            if (newIndex < neurons[layer - 1].Count)
            {
                Neuron oldNeuron = neurons[layer - 1][newIndex];
                foreach (WorkingNeuron n in neurons[layer + 1])
                {
                    foreach (Connection conn in n.GetConnections())
                    {
                        if (conn.entryNeuron == oldNeuron)
                        {
                            conn.entryNeuron = neuron;
                        }
                    }
                }
            }
            else
            {
                foreach (WorkingNeuron n in neurons[layer + 1])
                {
                    n.AddNeuronConnection(new Connection(neuron, Simulation.RandomFloat()*2-1));
                }
            }
            foreach (Neuron n in neurons[layer - 1])
            {
                neuron.AddNeuronConnection(new Connection(n, Simulation.RandomFloat() * 2 - 1));
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
                    newNeurons.Add(new WorkingNeuron(layerIndex + 1));
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
                    int prevLayerIndex = layerIndex - 1;
                    int connectionCount = 0;
                    while (prevLayerIndex >= 0)
                    {
                        if (layerIndex > 1 && prevLayerIndex == 0)
                        {
                            break;
                        }
                        for (int prevNeuronIndex = connectionCount; prevNeuronIndex < neurons[prevLayerIndex].Count &&
                            ((!(prevLayerIndex == 0) || connectionCount < FirstHiddenLayer.Count) || layerIndex == 1); ++prevNeuronIndex)
                        {
                            wn.AddNeuronConnection(neurons[prevLayerIndex][prevNeuronIndex], 1);
                            connectionCount++;
                        }
                        --prevLayerIndex;
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
                    // Allow slowly more weight as layer count rises
                    wn.RandomizeWeights(HiddenLayerCount / (2 * Mathf.Sqrt(HiddenLayerCount)) + 0.5f);
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
        /// <param name="mixRate">Percentage of the otherNetwork to use</param>
        public void MixNetwork(NeuronalNetwork otherNetwork, float mixRate)
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
            while (mixIndices.Count < totalCount * mixRate)
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
            Dictionary<Neuron, Neuron> oldToNewMap = new Dictionary<Neuron, Neuron>();
            foreach (InputNeuron input in InputNeurons)
            {
                Neuron newNeuron = input.NameCopy();
                oldToNewMap.Add(input, newNeuron);

                copy.AddInputNeuron((InputNeuron)newNeuron);
            }
            for (int layerIndex = 0; layerIndex < HiddenLayerCount; layerIndex++)
            {
                List<Neuron> newLayer = new List<Neuron>();
                foreach (WorkingNeuron wn in neurons[layerIndex + 1])
                {
                    WorkingNeuron newNeuron = wn.NameCopy() as WorkingNeuron;
                    oldToNewMap.Add(wn, newNeuron);
                    newLayer.Add(newNeuron);
                    foreach(Connection con in wn.GetConnections())
                    {
                        newNeuron.AddNeuronConnection(oldToNewMap[con.entryNeuron], con.weight);
                    }
                }
                copy.AddHiddenLayer(newLayer);
            }
            foreach (Neuron output in OutputNeurons)
            {
                WorkingNeuron newNeuron = output.NameCopy() as WorkingNeuron;
                copy.AddOutputNeuron(newNeuron);
                foreach (Connection con in (output as WorkingNeuron).GetConnections())
                {
                    newNeuron.AddNeuronConnection(oldToNewMap[con.entryNeuron], con.weight);
                }
            }

            copy.fullMeshGenerated = true;

            //copy.GenerateFullMesh();

            //for (int layerIndex = 1; layerIndex < neurons.Count; layerIndex++)
            //{
            //    for (int neuronIndex = 0; neuronIndex < neurons[layerIndex].Count; neuronIndex++)
            //    {
            //        List<Connection> connectionsOriginal = ((WorkingNeuron)neurons[layerIndex][neuronIndex]).GetConnections();
            //        List<Connection> connectionsCopy = ((WorkingNeuron)copy.neurons[layerIndex][neuronIndex]).GetConnections();
            //        if (connectionsOriginal.Count != connectionsCopy.Count)
            //        {
            //            throw new NotSameAmountOfNeuronsException();
            //        }
            //        for (int k = 0; k < connectionsOriginal.Count; k++)
            //        {
            //            connectionsCopy[k].weight = connectionsOriginal[k].weight;
            //        }
            //    }
            //}

            return copy;
        }

        public void CreateHiddenLayer()
        {
            List<Neuron> newLayer = new List<Neuron>();
            WorkingNeuron newNeuron = new WorkingNeuron(HiddenLayerCount + 2);
            int layerIndex = neurons.Count - 1;

            int prevLayerIndex = layerIndex-1;
            int connectionCount = 0;
            while (prevLayerIndex >= 0)
            {
                if (layerIndex > 1 && prevLayerIndex == 0)
                {
                    break;
                }
                float weight = 0.0f;
                if (connectionCount < LastHiddenLayer.Count && connectionCount < ((WorkingNeuron)LastHiddenLayer[connectionCount]).GetConnections().Count)
                {
                    weight = ((WorkingNeuron)LastHiddenLayer[0]).GetConnections()[connectionCount].weight;
                }
                for (int prevNeuronIndex = connectionCount; prevNeuronIndex < neurons[prevLayerIndex].Count &&
                    ((!(prevLayerIndex == 0) || connectionCount < FirstHiddenLayer.Count) || layerIndex == 1); ++prevNeuronIndex)
                {
                    newNeuron.AddNeuronConnection(neurons[prevLayerIndex][prevNeuronIndex], weight);
                    connectionCount++;
                }
                --prevLayerIndex;
            }

            for (int outputIndex = 0; outputIndex < OutputNeurons.Count; outputIndex++)
            {
                WorkingNeuron currentOutputNeuron = OutputNeurons[outputIndex] as WorkingNeuron;
                for (int connectionIndex = 0; connectionIndex < currentOutputNeuron.GetConnections().Count; connectionIndex++)
                {
                    if (currentOutputNeuron.GetConnections()[connectionIndex].entryNeuron == LastHiddenLayer[0])
                    {
                        currentOutputNeuron.GetConnections()[connectionIndex].entryNeuron = newNeuron;
                    }
                }
            }
            newLayer.Add(newNeuron);
            AddHiddenLayer(newLayer);
        }

        public void RemoveHiddenLayer()
        {
            if (HiddenLayerCount > 1)
            {
                neurons.RemoveAt(neurons.Count - 2);
                for (int outNeuronIndex = 0; outNeuronIndex < OutputNeurons.Count; outNeuronIndex++)
                {
                    WorkingNeuron outputNeuron = OutputNeurons[outNeuronIndex] as WorkingNeuron;
                    if (outputNeuron.GetConnections().Count > LastHiddenLayer.Count)
                    {
                        outputNeuron.GetConnections().RemoveRange(LastHiddenLayer.Count, outputNeuron.GetConnections().Count - LastHiddenLayer.Count);
                    }
                    for (int connectionIndex = 0; connectionIndex < LastHiddenLayer.Count && connectionIndex < outputNeuron.GetConnections().Count; connectionIndex++)
                    {
                        outputNeuron.GetConnections()[connectionIndex].entryNeuron = LastHiddenLayer[connectionIndex];
                    }
                    for (int connectionIndex = outputNeuron.GetConnections().Count; connectionIndex < LastHiddenLayer.Count; connectionIndex++)
                    {
                        outputNeuron.AddNeuronConnection(LastHiddenLayer[connectionIndex], (Simulation.RandomFloat() * 2 * -1));
                    }
                }
            }
        }
    }
}
