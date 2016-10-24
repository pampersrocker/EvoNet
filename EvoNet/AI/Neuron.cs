using System;
using System.Collections.Generic;
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
