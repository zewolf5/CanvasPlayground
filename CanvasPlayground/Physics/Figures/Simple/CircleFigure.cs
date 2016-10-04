using System;

namespace CanvasPlayground.Physics.Figures.Simple
{
    public class CircleFigure : BaseFigure, IFigure
    {
        private int _radius;

        public CircleFigure(PWorld world, int radiusPixels, int x, int y) : base(world,x,y)
        {
            _radius = radiusPixels;

            var circleVertices = CreateCircle(radiusPixels, 5);
            Create(x, y, circleVertices);

            Mass = 0.3f;

            Friction = 0;
            Restitution = 1;
        }

        public Vertices CreateCircle(float radius, float pieces = 5)
        {
            var simRadius = (radius);
            double angleStep = Math.PI * 2 / pieces;

            Vertices vertices = new Vertices();

            for (double angle = 0; angle < Math.PI * 2; angle += angleStep)
            {
                double x = simRadius * Math.Cos(angle);
                double y = simRadius * Math.Sin(angle);

                vertices.Add(new Vector2((float)x, (float)y));
            }
            return vertices;
        }
    }

}
