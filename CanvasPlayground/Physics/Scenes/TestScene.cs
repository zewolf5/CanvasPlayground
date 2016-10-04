using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using CanvasPlayground.Physics.Figures;
using Timer = System.Timers.Timer;

namespace CanvasPlayground.Physics.Scenes
{
    public class TestScene : IScene
    {
        private WorldLoop _worldLoop;
        public TestScene(WorldLoop worldLoop)
        {
            _worldLoop = worldLoop;
        }

        public void Start()
        {
            Timer t = new Timer();
            t.Elapsed += T_Elapsed1;
            t.Interval = 5000;
            t.Start();

            _worldLoop.AddFigure(new CircleFigure(_worldLoop.World, 75, 400, 400,40));
        }

        private void T_Elapsed1(object sender, ElapsedEventArgs e)
        {
            ((Timer)sender).Stop();
            _worldLoop.AddFigure(new CircleFigure(_worldLoop.World, 250, 400, 400, 40));
        }

        public void Stop()
        {

        }

        public void SendEvent(string eventString)
        {
            if (eventString == "SomeEvent")
            {
                for (int i = 0; i < 25; i++)
                {
                    _worldLoop.AddFigure(new Circle(_worldLoop.World, 15, 400, 400));
                    Thread.Sleep(100);   
                }
            }
        }
    }
}
