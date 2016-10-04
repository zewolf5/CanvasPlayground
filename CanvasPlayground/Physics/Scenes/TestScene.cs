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
        List<IFigure> _sceneFigures = new List<IFigure>();

        private WorldLoop _worldLoop;
        public TestScene(WorldLoop worldLoop)
        {
            _worldLoop = worldLoop;
        }

        public void Start()
        {
            SceneTimer.RunSetup(new List<Tuple<int, Action>> {
                new Tuple<int, Action>(500, () =>{
                    AddFigure(new CircleFigure(_worldLoop.World, 75, 600, 400,40));
                }),
                new Tuple<int, Action>(1000, () =>{
                    AddFigure(new CircleFigure(_worldLoop.World, 75, 600, 400,40));
                }),
                new Tuple<int, Action>(2000, () =>{
                    AddFigure(new CircleFigure(_worldLoop.World, 75, 600, 600,40));
                }),
                new Tuple<int, Action>(3000, () =>{
                    AddFigure(new CircleFigure(_worldLoop.World, 75, 400, 600,40));
                }),
                new Tuple<int, Action>(4000, () =>{
                    AddFigure(new Rectangle(_worldLoop.World, 600,20, 0.2f, 500,500));
                }),

                new Tuple<int, Action>(10000, () =>{
                    AddFigure(new Rectangle(_worldLoop.World, 600,20, 0.2f, 500,500) {Static = true});
                }),
                new Tuple<int, Action>(20000, () =>{
                    AddFigure(new Rectangle(_worldLoop.World, 600,20, -0.2f, 1500,500) {Static = true});
                }),
            });

            while (_worldLoop != null)
            {
                //Do something/wait for something/ react to stuff as long as the scene is active

                Thread.Sleep(1000);
            }
        }

        private void AddFigure(IFigure figure)
        {
            _sceneFigures.Add(figure);
            _worldLoop?.AddFigure(figure);
        }

        public void Stop()
        {
            //Clean up!
            foreach (var sceneFigure in _sceneFigures)
            {
                _worldLoop.RemoveFigure(sceneFigure);
            }
            _worldLoop = null;
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
