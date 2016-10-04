using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChipmunkSharp;

namespace CanvasPlayground.Physics
{
    public class Vertices : List<Vector2>
    {
        public Vertices()
        {
        }

        public Vertices(int capacity) : base(capacity)
        {
        }

        public Vertices(IEnumerable<Vector2> vertices)
        {
            AddRange(vertices);
        }
    }
}