using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace CanvasPlayground.Utils
{
    public static class SceneTimer
    {

        public static void RunSetup(List<Tuple<int,Action>> _timeAndScenes)
        {
            var list = _timeAndScenes.OrderBy(o => o.Item1);

            foreach (var tuple in list)
            {
                var time = tuple.Item1;
                var action = tuple.Item2;
                Timer timer = new Timer();
                timer.Interval = time;
                timer.Elapsed += (s, e) =>
                {
                    timer.Stop();
                    try
                    {
                        action();
                    }
                    catch (Exception)
                    {
                    }
                };
                timer.Start();
            }
        }
    }
}
