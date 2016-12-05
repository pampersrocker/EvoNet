using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvoNet.AI
{
    [Serializable]
    public class Connection
    {
        public float weight = 1;
        public Neuron entryNeuron;

        public Connection(Neuron n, float weight)
        {
            this.weight = weight;
            this.entryNeuron = n;
        }

        public float GetValue()
        {
            return weight * entryNeuron.GetValue();
        }
    }
}
