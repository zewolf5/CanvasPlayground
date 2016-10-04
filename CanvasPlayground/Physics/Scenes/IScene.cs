using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanvasPlayground.Physics.Scenes
{
    public interface IScene
    {
        void Start();
        void Stop();

        void SendEvent(string eventString);
    }
}
