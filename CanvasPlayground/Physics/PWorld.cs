using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChipmunkSharp;

namespace CanvasPlayground.Physics
{
    public class PWorld
    {
        const float DefaultGravity = 981;
        private static PWorld _activePWorld;

        public cpSpace Space { get; private set; }

        public static PWorld ActivePWorld
        {
            get
            {
                if (_activePWorld == null)
                {
                    _activePWorld = new PWorld();
                }
                return _activePWorld;
            }
        }

        public PWorld()
        {
            Space = new cpSpace();
            cpVect gravity = new cpVect(0, DefaultGravity);
            Space = new cpSpace();
            Space.SetGravity(gravity);
        }

        public void ReverseGravity()
        {
            cpVect gravity = new cpVect(-Space.GetGravity().x, -Space.GetGravity().y);
            Space.SetGravity(gravity);
        }
    }
}
