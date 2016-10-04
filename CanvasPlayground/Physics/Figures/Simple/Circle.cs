namespace CanvasPlayground.Physics.Figures.Simple
{
    public class Circle : BaseFigure, IFigure
    {
        private int _radius;

        public Circle(PWorld world, int radiusPixels, int x, int y) : base(world,x,y)
        {
            Create(x, y, new Vertices(), radiusPixels);

            //Density = 0.3f;
            //Friction = 0;
            //Restitution = 1;
        }

     
    }

}
