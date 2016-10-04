using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using CanvasPlayground.Physics.Figures;
using CanvasPlayground.Utils;
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
            SceneTimer.RunSetup(new List<Tuple<int, Action>> {
                new Tuple<int, Action>(500, () =>{
                    _worldLoop.AddFigure(new CircleFigure(_worldLoop.World, 75, 400, 400,40));
                }),
                new Tuple<int, Action>(1000, () =>{
                    _worldLoop.AddFigure(new CircleFigure(_worldLoop.World, 75, 600, 400,40));
                }),
                new Tuple<int, Action>(1500, () =>{
                    _worldLoop.AddFigure(new CircleFigure(_worldLoop.World, 75, 600, 600,40));
                }),
                new Tuple<int, Action>(2500, () =>{
                    _worldLoop.AddFigure(new CircleFigure(_worldLoop.World, 75, 400, 600,40));
                }),
                new Tuple<int, Action>(3500, () =>{
                    _worldLoop.AddFigure(new Rectangle(_worldLoop.World, 600,20, 0.2f, 500,500));
                }),

                   new Tuple<int, Action>(10000, () =>{
                    _worldLoop.AddFigure(new Rectangle(_worldLoop.World, 600,20, 0.2f, 500,500) {Static = true});
                }),
            });

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
