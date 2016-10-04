using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanvasPlayground.Physics.Figures
{
    public class Base
    {
        internal static Random r = new Random();

        private static int _id;
        public int Id { get; private set; }

        public Base()
        {
            Id = _id++;
        }
    }
}
