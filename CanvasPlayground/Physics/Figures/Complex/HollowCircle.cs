﻿using System;
using System.Collections.Generic;
using CanvasPlayground.Physics.Figures.Simple;

namespace CanvasPlayground.Physics.Figures.Complex
{
    public class HollowCircle : BaseComplexFigure
    {

        public HollowCircle(PWorld world, int radius, int borderSize, int x, int y) : base(world, x, y)
        {

            float[] angles;
            var circleVertices = CreateCircleVertices(radius, 20, out angles);

            var length = radius * 2 / 6;

            Rectangle lastFigure = null;
            Rectangle firstFigure = null;
            Vector2 lastVector = Vector2.Zero;
            Vector2 firstVector = Vector2.Zero;

            for (int i = 0; i < circleVertices.Count; i++)
            {
                var circleVertex = circleVertices[i];
                var angle = angles[i];
                var figure1 = new Rectangle(world, borderSize, length, angle, x + (int)circleVertex.X, y + (int)circleVertex.Y);


                Figures.Add(figure1);

                if (lastFigure != null)
                {
                    //JointFactory.CreateWeldJoint(world, figure1.Body, lastFigure.Body, Vector2.Zero, Vector2.Zero);
                    //lastVector = circleVertex;
                }
                else
                {
                    //firstFigure = figure1;
                    //firstVector = circleVertex;
                }

                lastFigure = figure1;
            }
            //JointFactory.CreateWeldJoint(world, lastFigure.Body, firstFigure.Body, Vector2.Zero, Vector2.Zero);


            Mass = 1f;
            Friction = 0;
            Restitution = 1;
        }

        public Vertices CreateCircleVertices(float radius, float pieces, out float[] angles)
        {
            double angleStep = Math.PI * 2 / pieces;

            Vertices vertices = new Vertices();
            var anglesList = new List<float>();
            for (double angle = 0; angle < Math.PI * 2; angle += angleStep)
            {
                anglesList.Add((float)angle);

                double x = radius * Math.Cos(angle);
                double y = radius * Math.Sin(angle);

                vertices.Add(new Vector2((float)x, (float)y));
            }
            angles = anglesList.ToArray();
            return vertices;
        }
    }

}
