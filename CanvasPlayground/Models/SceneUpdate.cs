using System.Collections.Generic;

namespace CanvasPlayground.Models
{
    public class SceneUpdate
    {
        public long Ms { get; set; }
        public long FrameNo { get; set; }
        public List<ObjectInfo> Objects { get; set; }
        public List<Info> Info { get; set; }

    }
}
