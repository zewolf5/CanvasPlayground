using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FarseerPhysics;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;

namespace CanvasPlayground.Physics.Figures
{
    public class Cross : BaseComplexFigure
    {
        private int _originalWidth;
        private int _originalHeight;

        public Cross(World world, int width, int height, int borderSize, int x, int y) : base(world, x, y)
        {
            _originalWidth = width;
            _originalHeight = height;

            var rectVertices1 = new Rectangle(world, width, borderSize, 0, x, y);
            var rectVertices2 = new Rectangle(world, borderSize, height, 0, x, y);

            Figures.Add(rectVertices1);
            Figures.Add(rectVertices2);

            JointFactory.CreateWeldJoint(world, rectVertices1.Body, rectVertices2.Body, Vector2.Zero, Vector2.Zero);

            Density = 1f;
            Friction = 0;
            Restitution = 1;
        }


    }

}
