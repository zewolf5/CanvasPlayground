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
    public interface IFigure
    {
        int Id { get; }
        World World { get; }
        Body Body { get; }
        Shape Shape { get; }
        Fixture Fixture { get; }

        event Func<IFigure, IFigure, FarseerPhysics.Dynamics.Contacts.Contact, bool> OnCollision;


        Vertices Vertices { get; }
        Vertices OriginalVertices { get; }
        int X { get; }
        int Y { get; }
        float Density { get; set; }
        float Mass { get; set; }
        float Restitution { get; set; }
        float Friction { get; set; }
        bool Static { get; set; }
        bool SleepingAllowed { get; set; }

        void Clear();

        bool IsOutOfBounds { get; }
        string HtmlColor { get; }

        Vector2 Position { get; }
        Vector2 LinearVelocity { get; }

    }
}
