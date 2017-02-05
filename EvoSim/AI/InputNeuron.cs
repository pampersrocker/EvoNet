using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EvoNet.AI
{
    [Serializable]
    public class InputNeuron : Neuron
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

        public override Neuron NameCopy()
        {
            InputNeuron clone = new InputNeuron();
            clone.SetName(GetName());
            return clone;
        }

        public InputNeuron() :
            base(0)
        {

        }

        public InputNeuron(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
        
        }
    }
}
