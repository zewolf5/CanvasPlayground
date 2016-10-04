using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FarseerPhysics;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace CanvasPlayground.Physics.Figures
{
    public class Triangle : BaseFigure
    {

        public Triangle(World world, int side, int side2, int side3, int x, int y) : base(world,x,y)
        {

            var rectVertices = CreateTriangle(side, side2, side3);
            Create(x, y, rectVertices);

            Density = 0.3f;
            Friction = 0;
            Restitution = 1;
        }

        public static Vertices CreateTriangle(int side, int side2, int side3)
        {
            var simSide1 = ConvertUnits.ToSimUnits(side);
            var simSide2 = ConvertUnits.ToSimUnits(side2);
            var simSide3 = ConvertUnits.ToSimUnits(side3);
            Vertices vertices = new Vertices(3);
            vertices.Add(new Vector2(0, 0.5f));
            vertices.Add(new Vector2(0.5f, -0.30f));
            vertices.Add(new Vector2(-0.5f, -0.30f));

            return vertices;
        }

    }

}
