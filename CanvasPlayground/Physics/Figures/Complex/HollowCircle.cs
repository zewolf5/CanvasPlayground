using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public class HollowCircle : BaseComplexFigure
    {

        public HollowCircle(World world, int radius, int borderSize, int x, int y, string color = null, bool? isStatic = null) : base(world, x, y, color)
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
                var figure1 = new Rectangle(world, borderSize, length, angle, x + (int)circleVertex.X, y + (int)circleVertex.Y, color, isStatic);

                figure1.OnCollision += Figure1_OnCollision;


                Figures.Add(figure1);

                if (lastFigure != null)
                {
                    //JointFactory.CreateWeldJoint(world, figure1.Body, lastFigure.Body, Vector2.Zero, Vector2.Zero);
                    lastVector = circleVertex;
                }
                else
                {
                    firstFigure = figure1;
                    firstVector = circleVertex;
                }

                lastFigure = figure1;
            }
            //JointFactory.CreateWeldJoint(world, lastFigure.Body, firstFigure.Body, Vector2.Zero, Vector2.Zero);


            Density = 0.3f;
            Friction = 0;
            Restitution = 1;
        }

        private bool Figure1_OnCollision(IFigure arg1, IFigure arg2, FarseerPhysics.Dynamics.Contacts.Contact arg3)
        {
            Debug.WriteLine($"Collision between: {arg1.Id} and : {arg2.Id}");
            return true;
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
