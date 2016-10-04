using ChipmunkSharp;

namespace CanvasPlayground.Physics.Figures
{
    public interface IFigure
    {
        int Id { get; }
        PWorld World { get; }
        cpBody Body { get; }
        cpShape Shape { get; }
        //Fixture Fixture { get; }
        Vertices Vertices { get; }
        Vertices OriginalVertices { get; }
        int X { get; }
        int Y { get; }
        //float Density { get; set; }
        float Mass { get; set; }
        float Restitution { get; set; }
        float Friction { get; set; }
        bool Static { get; set; }
        bool SleepingAllowed { get; set; }

        void Clear();

        bool IsOutOfBounds { get; }
        string HtmlColor { get; }

        Vector2 Position { get; }
        Vector2 LinearVelocity { get; }

    }
}
