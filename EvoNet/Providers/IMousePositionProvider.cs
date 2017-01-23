using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvoNet.Providers
{
    public interface IMousePositionProvider
    {
        Vector2 GetMousePosition();
    }
}
