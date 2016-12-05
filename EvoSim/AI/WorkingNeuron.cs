using EvoSim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace EvoNet.AI
{
    [Serializable]
    public class WorkingNeuron : Neuron
    {
        private float? value = null;
        private List<Connection> connections = new List<Connection>();

        public void RandomMutation(float MutationRate)
        {
            Connection c = connections[Simulation.RandomInt(connections.Count)];
            c.weight += (float)Simulation.RandomFloat() * 2 * MutationRate - MutationRate;
        }

        public void AddNeuronConnection(Neuron n, float weight)
        {
            AddNeuronConnection(new Connection(n, weight));
        }

        public void AddNeuronConnection(Connection connection)
        {
            connections.Add(connection);
        }

        public void Invalidate()
        {
            this.value = null;
        }

        public List<Connection> GetConnections()
        {
            return connections;
        }

        public void RandomizeWeights()
        {
            foreach(Connection c in connections)
            {
                c.weight = (float)Simulation.RandomFloat() * 2 - 1;
            }
        }

        private void Calculate()
        {
            float value = 0;
            foreach(Connection c in connections)
            {
                value += c.GetValue();
            }
            value = Mathf.Sigmoid(value);
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

        public override Neuron NameCopy()
        {
            WorkingNeuron clone = new WorkingNeuron();
            clone.SetName(GetName());
            return clone;
        }

        public float GetStrongestConnection()
        {
            float strongest = 0;
            foreach(Connection c in connections)
            {
                float val = Mathf.Abs(c.weight);
                if (val > strongest) strongest = val;
            }
            return strongest;
        }

        public WorkingNeuron() : 
            base()
        {

        }

        public WorkingNeuron(SerializationInfo info, StreamingContext context):
            base(info, context)
        {
            connections = info.GetValue(nameof(connections), typeof(List<Connection>)) as List<Connection>;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(connections), connections);
        }
    }
}
