namespace CanvasPlayground.Physics.Figures.Simple
{
    public class Rectangle : BaseFigure
    {
        private int _originalWidth;
        private int _originalHeight;

        public Rectangle(PWorld world, int width, int height, float angle, int x, int y) : base(world,x,y)
        {
            _originalWidth = width;
            _originalHeight = height;

            var rectVertices = CreateRectangle(width, height);
            //rectVertices = RotateVertices(rectVertices, angle);

            Create(x, y, rectVertices);

            Mass = 0.3f;
            Friction = 0;
            Restitution = 1;
        }

        public static Vertices CreateRectangle(float hx, float hy)
        {
            var simHx = (hx/2);
            var simHy = (hy/2);
            Vertices vertices = new Vertices(4);
            vertices.Add(new Vector2(-simHx, -simHy));
            vertices.Add(new Vector2(simHx, -simHy));
            vertices.Add(new Vector2(simHx, simHy));
            vertices.Add(new Vector2(-simHx, simHy));

            return vertices;
        }

    }

}
