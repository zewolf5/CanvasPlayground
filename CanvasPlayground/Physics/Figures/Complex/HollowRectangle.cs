﻿using CanvasPlayground.Physics.Figures.Simple;

namespace CanvasPlayground.Physics.Figures.Complex
{
    public class HollowRectangle : BaseComplexFigure
    {
        private int _originalWidth;
        private int _originalHeight;

        public HollowRectangle(PWorld world, int width, int height, int borderSize, int x, int y) : base(world, x, y)
        {
            _originalWidth = width;
            _originalHeight = height;

            int halfX = width / 2;
            int halfY = height / 2;

            var figure1 = new Rectangle(world, width, borderSize, 0, x, y - halfY);
            var figure2 = new Rectangle(world, borderSize, height, 0, x - halfX, y);
            var figure3 = new Rectangle(world, width, borderSize, 0, x, y + halfY);
            var figure4 = new Rectangle(world, borderSize, height, 0, x + halfX, y);

            Figures.Add(figure1);
            Figures.Add(figure2);
            Figures.Add(figure3);
            Figures.Add(figure4);

            //JointFactory.CreateWeldJoint(world, figure1.Body, figure2.Body, Vector2.Zero, Vector2.Zero);
            //JointFactory.CreateWeldJoint(world, figure2.Body, figure3.Body, Vector2.Zero, Vector2.Zero);
            //JointFactory.CreateWeldJoint(world, figure3.Body, figure4.Body, Vector2.Zero, Vector2.Zero);
            //JointFactory.CreateWeldJoint(world, figure4.Body, figure1.Body, Vector2.Zero, Vector2.Zero);

            Mass = 1f;
            Friction = 0;
            Restitution = 1;
        }


    }

}
