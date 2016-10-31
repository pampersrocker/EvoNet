using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvoNet.AI
{
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

        public static Connection Deserialize(BinaryReader reader, IEnumerable<Neuron> inputNeurons)
        {
            int index = reader.ReadInt32();
            Neuron entry = inputNeurons.ElementAt(index);
            float weight = reader.ReadSingle();
            return new Connection(entry, weight);
        }

        internal void Serialize(BinaryWriter writer, IEnumerable<Neuron> inputNeurons)
        {
            IEnumerator<Neuron> enumerator = inputNeurons.GetEnumerator();
            int index = 0;
            bool found = false;
            while (enumerator.MoveNext())
            {
                if (entryNeuron == enumerator.Current)
                {
                    found = true;
                    break;
                }
                index++;
            }
            writer.Write((int)(found ? index : -1));
            writer.Write(weight);
        }
    }
}
