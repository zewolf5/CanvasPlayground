using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using CanvasPlayground.Models;
using CanvasPlayground.Physics.Figures;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace CanvasPlayground.Physics
{
    public class WorldLoop
    {
        //2 lists of all the objects
        public List<IFigure> Figures { get; private set; }
        public List<IComplexFigure> CFigures { get; private set; }
        public List<Info> TextInfos { get; private set; }

        //Frame counter
        public long FrameNo { get; internal set; }
        public DateTime FrameRenderTime { get; internal set; }


        //Misc
        private bool _runEngine = true;
        private Thread _thread;
        private DateTime _lastRun = DateTime.MinValue;


        //private Thread _stepThread = null;

        //private Action _doStepWork = null;
        //private bool _stepWorkDone = false;
        private Vector2 _gravity;

        private static object _locker = new object();
        //private static object _syncClearItem = new object();

        public float WorldFriction { get; set; } = 0f;

        public World World { get; private set; }

        private int _sleepDuration = 30;


        private System.Windows.Threading.Dispatcher _worldDispatcher;

        public WorldLoop(int sleepDuration)
        {
            ManualResetEvent dispatcherReadyEvent = new ManualResetEvent(false);
            _sleepDuration = sleepDuration;

            new Thread(new ThreadStart(() =>
            {
                _worldDispatcher = Dispatcher.CurrentDispatcher;
                dispatcherReadyEvent.Set();
                Dispatcher.Run();
            })).Start();

            dispatcherReadyEvent.WaitOne();
        }

        public void Start(Vector2 gravity)
        {
            _gravity = gravity;
            Figures = new List<IFigure>();
            CFigures = new List<IComplexFigure>();
            TextInfos = new List<Info>();
            World = new World(_gravity);
        }

        public void Start()
        {
            Stop();

            World = new World(_gravity);

            _runEngine = true;
            _thread = new Thread(InitializeEngineLoop);
            _thread.IsBackground = true;
            _thread.Priority = ThreadPriority.Highest;
            _thread.Start();
        }

        public void Stop()
        {
            _runEngine = false;
            _thread?.Abort();
            //_stepThread?.Abort();
            IEnumerable<IFigure> figures;
            lock (Figures) figures = Figures.ToList();
            foreach (var figure in figures)
            {
                RemoveFigure(figure);
            }
            Figures.Clear();

            IEnumerable<IComplexFigure> cfigures;
            lock (Figures) cfigures = CFigures.ToList();
            foreach (var figure in cfigures)
            {
                RemoveComplexFigure(figure);
            }
            CFigures.Clear();

            TextInfos.Clear();

            _thread?.Abort();
            //_stepThread?.Abort();
            //_stepThread = null;
        }




        public void InitializeEngineLoop()
        {
            Debug.WriteLine("Engine Started");
            try
            {
                while (_runEngine)
                {
                    try
                    {
                        MainEngineLoop();
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            catch (Exception)
            {
            }
            Debug.WriteLine("Engine Stopped");
        }

        public void MainEngineLoop()
        {
            //_timer.Stop();
            DateTime now = DateTime.Now;
            var stepSize = now.Subtract(_lastRun);
            _lastRun = now;

            var swa = Stopwatch.StartNew();
            DoTheStepTimeout((float)stepSize.TotalSeconds);
            FrameNo++;
            FrameRenderTime = now;
            if (swa.ElapsedMilliseconds > 70)
            {
                Debug.WriteLine($"stepTime: {(int)stepSize.TotalMilliseconds} - STEP: {(int)swa.ElapsedMilliseconds}");
            }
            swa.Stop();

            List<IFigure> objects;
            lock (Figures) objects = Figures.ToList();
            foreach (var figure in objects)
            {
                if (figure.IsOutOfBounds)
                {
                    RemoveFigure(figure);
                    Debug.WriteLine($"REMOVED OUT OF PLACE BALL");
                }
            }

            List<IComplexFigure> cobjects;
            lock (CFigures) cobjects = CFigures.ToList();
            foreach (var figure in cobjects)
            {
                if (figure.IsOutOfBounds)
                {
                    RemoveComplexFigure(figure);
                    Debug.WriteLine($"REMOVED OUT OF PLACE BALL");
                }
            }
            var sleep = _sleepDuration;
            var elapsed = (int)DateTime.Now.Subtract(now).TotalMilliseconds;
            if(sleep>elapsed) Thread.Sleep(sleep-elapsed);
            Thread.Sleep(1);
        }


        private void DoTheStepTimeout(float time)
        {

            var sw = Stopwatch.StartNew();

            _worldDispatcher.Invoke(() =>
            {
                World.Step(time);

                IEnumerable<IComplexFigure> cfigures;
                lock (Figures) cfigures = CFigures.ToList();
                foreach (var figure in cfigures)
                {
                    figure.Step(time);
                }
                if (WorldFriction != 1f)
                {
                    IEnumerable<IFigure> figures;
                    lock (Figures) figures = Figures.ToList();
                    foreach (var figure in figures)
                    {
                        figure.LinearVelocity *= (1f - WorldFriction);
                    }
                }

            });



            //while (!_stepWorkDone)
            //{
            //    Thread.Sleep(1);
            //    if (sw.ElapsedMilliseconds > 35000)
            //    {
            //        Debug.WriteLine("ABORT SLOW CALC!");

            //        IEnumerable<IFigure> removes;
            //        lock (Figures) removes = Figures.AsEnumerable().Reverse().Take(Figures.Count / 10);
            //        Debug.WriteLine($"Removing items: {removes.Count()}/{Figures.Count}");
            //        lock (Figures)
            //        {
            //            lock (_syncClearItem)
            //            {
            //                foreach (var remove in removes)
            //                {
            //                    remove.Clear();
            //                    Figures.Remove(remove);
            //                }
            //            }
            //        }
            //        _stepThread?.Abort();
            //        _stepThread = null;
            //        break;
            //    }
            //}

        }

        private void AddFigure(IFigure figure)
        {
            lock (Figures) Figures.Add(figure);
        }


        public void RemoveFigure(IFigure removeItem)
        {
            _worldDispatcher.Invoke(() =>
            {
                if (!removeItem.Body.IsDisposed) removeItem.Clear();
            });
            lock (Figures) Figures.Remove(removeItem);
        }


        private void AddComplexFigure(IComplexFigure figure)
        {
            lock (CFigures) CFigures.Add(figure);
        }


        public void RemoveComplexFigure(IComplexFigure removeItem)
        {
            _worldDispatcher.Invoke(() => { removeItem.Clear(); });
            lock (CFigures) CFigures.Remove(removeItem);
        }

        public void RemoveLast10()
        {
            IEnumerable<IFigure> removes = null;
            lock (Figures) removes = Figures.AsEnumerable().Reverse().Take(10);

            Debug.WriteLine($"Removing items: {removes.Count()}/{Figures.Count}");
            _worldDispatcher.Invoke(() =>
            {
                foreach (var remove in removes)
                {
                    RemoveFigure(remove);
                }
            });
        }

        public IFigure CreateFigure(Func<IFigure> figureCreation)
        {
            return _worldDispatcher.Invoke(() =>
            {
                var figure = figureCreation?.Invoke();
                if (figure != null) AddFigure(figure);
                return figure;
            });
        }

        public IComplexFigure CreateComplexFigure(Func<IComplexFigure> figureCreation)
        {
            return _worldDispatcher.Invoke(() =>
            {
                var figure = figureCreation?.Invoke();
                if (figure != null) AddComplexFigure(figure);
                return figure;
            });
        }

    }
}
