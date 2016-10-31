using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvoNet.AI
{
    public abstract class Neuron
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

        public Vector2 DrawPosition { get; set; }

        public bool IsMouseOverDrawPosition(float NeuronSize, MouseState ms)
        {
            Vector2 mousePos = new Vector2(ms.Position.X , ms.Position.Y);
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
    }
}
