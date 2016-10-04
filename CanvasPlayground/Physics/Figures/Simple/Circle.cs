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
    public class Circle : BaseFigure, IFigure
    {
        private int _radius;

        public Circle(World world, int radiusPixels, int x, int y) : base(world,x,y)
        {
            Create(x, y, new Vertices(), radiusPixels);

            Density = 0.3f;
            Friction = 0;
            Restitution = 1;
        }

     
    }

}
