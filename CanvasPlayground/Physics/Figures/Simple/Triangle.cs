namespace CanvasPlayground.Physics.Figures.Simple
{
    public class Triangle : BaseFigure
    {

        public Triangle(PWorld world, int side, int side2, int side3, int x, int y) : base(world,x,y)
        {

            var rectVertices = CreateTriangle(side, side2, side3);
            Create(x, y, rectVertices);

            Mass = 0.3f;

            Friction = 0;
            Restitution = 1;
        }

        public static Vertices CreateTriangle(int side, int side2, int side3)
        {
            var simSide1 = (side);
            var simSide2 = (side2);
            var simSide3 = (side3);
            Vertices vertices = new Vertices(3);
            vertices.Add(new Vector2(0, 0.5f));
            vertices.Add(new Vector2(0.5f, -0.30f));
            vertices.Add(new Vector2(-0.5f, -0.30f));

            return vertices;
        }

    }

}
