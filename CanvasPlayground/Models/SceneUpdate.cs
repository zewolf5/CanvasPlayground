using System.Collections.Generic;

namespace CanvasPlayground.Models
{
    public class SceneUpdate
    {
        public long FrameNo { get; set; }
        public List<ObjectInfo> Objects { get; set; }
    }
}
