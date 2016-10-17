using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvoNet.NeuronalNetwork
{
    class WorkingNeuron : Neuron
    {
        private float? value = null;
        private List<Connection> connections = new List<Connection>();

        public void AddNeuronConnection(Neuron n, float weight)
        {
            AddNeuronConnection(new Connection(n, weight));
        }

        private void AddNeuronConnection(Connection connection)
        {
            connections.Add(connection);
        }

        public void Invalidate()
        {
            this.value = null;
        }

        private void Calculate()
        {
            float value = 0;
            foreach(Connection c in connections)
            {
                value += c.GetValue();
            }
            value = Neuron.Sigmoid(value);
            this.value = value;
        }

        public override float GetValue()
        {
            if(value == null)
            {
                Calculate();
            }
            return (float)value;
        }
    }
}
