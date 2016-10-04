using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace CanvasPlayground.Physics.Figures
{
    public interface IComplexFigure
    {
        int Id { get; }

        World World { get; }

        List<IFigure> Figures { get; }

        int X { get; }
        int Y { get; }
        float Mass { get; set; }
        float Density { get; set; }
        float Restitution { get; set; }
        float Friction { get; set; }
        bool Static { get; set; }

        void Clear();

        bool IsOutOfBounds { get; }
        string HtmlColor { get; }

        Vector2 Position { get; }
        Vector2 LinearVelocity { get; }
        float RotationPerSecond { get; set; }

        void Step(float seconds);

    }
}
