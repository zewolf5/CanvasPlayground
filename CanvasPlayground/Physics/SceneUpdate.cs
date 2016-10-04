using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanvasPlayground.Physics
{
    public class SceneUpdate
    {
        public long FrameNo { get; set; }
        public List<ObjectInfo> Objects { get; set; }
    }
}
