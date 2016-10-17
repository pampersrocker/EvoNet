using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvoNet.NeuronalNetwork
{
    class InputNeuron : Neuron
    {
        private float value = 0;
        public void SetValue(float x)
        {
            this.value = x;
        }
        public override float GetValue()
        {
            return this.value;
        }
    }
}
