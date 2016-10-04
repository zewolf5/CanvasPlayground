
using System.Collections.Generic;
using ChipmunkSharp;

namespace CanvasPlayground.Physics
{
    public class ObjectInfo
    {
        public int I { get; set; }
        public string C { get; set; }

        public Vector2 P { get; set; }
        public Vector2 D { get; set; }

        public List<Vector2> V { get; set; }
    }
}
