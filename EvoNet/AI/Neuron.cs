using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvoNet.AI
{
    abstract class Neuron
    {
        private String name = "NO NAME";
        public abstract float GetValue();
        public abstract Neuron NameCopy();
        public virtual void Serialize(BinaryWriter writer)
        {
            writer.Write(name);
        }
        public virtual void Deserialize(BinaryReader reader)
        {
            name = reader.ReadString();
        }



        public string GetName()
        {
            return name;
        }

        public void SetName(string name)
        {
            this.name = name;
        }
    }
}
