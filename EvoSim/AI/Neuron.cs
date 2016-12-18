using EvoSim;
using EvoSim.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EvoNet.AI
{
    [Serializable]
    public abstract class Neuron : ISerializable
    {
        private string name = "NO NAME";
        public abstract float GetValue();
        public abstract Neuron NameCopy();
        private Vector2 drawPosition;
        public Vector2 DrawPosition { get { return drawPosition; } set { drawPosition = value; } }

        public bool IsMouseOverDrawPosition(float NeuronSize, Vector2 mousePos)
        {
            Vector2 to = DrawPosition - mousePos;
            return to.LengthSquared() < NeuronSize * NeuronSize;
        }

        public string GetName()
        {
            return name;
        }

        public void SetName(string name)
        {
            this.name = name;
        }

        public Neuron()
        {

        }

        public Neuron(SerializationInfo info, StreamingContext context)
        {
            name = info.GetString("name");
            drawPosition = info.GetVector2("drawPosition");
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddVector2("drawPosition", drawPosition);
            info.AddValue("name", name);
        }
    }
}
