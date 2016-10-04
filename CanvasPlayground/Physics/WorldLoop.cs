using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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

        //Frame counter
        public long FrameNo { get; internal set; }


        //Misc
        private bool _runEngine = true;
        private Thread _thread;
        private DateTime _lastRun = DateTime.MinValue;

        private Thread _stepThread = null;
        private Action _doStepWork = null;
        private bool _stepWorkDone = false;
        private Vector2 _gravity;

        private static object _locker = new object();
        private static object _syncClearItem = new object();


        public World World { get; private set; }


        public void Start(Vector2 gravity, int sleepDuration)
        {
            _gravity = gravity;
            Figures = new List<IFigure>();
            CFigures = new List<IComplexFigure>();

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
            _stepThread?.Abort();
            IEnumerable<IFigure> figures;
            lock (Figures) figures = Figures.ToList();
            foreach (var figure in figures)
            {
                lock (_syncClearItem) figure.Clear();
            }
            Figures.Clear();

            IEnumerable<IComplexFigure> cfigures;
            lock (Figures) cfigures = CFigures.ToList();
            foreach (var figure in cfigures)
            {
                lock (_syncClearItem) figure.Clear();
            }
            CFigures.Clear();

            _thread?.Abort();
            _stepThread?.Abort();
            _stepThread = null;
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
                    lock (_syncClearItem) figure.Clear();
                    lock (Figures) Figures.Remove(figure);
                    Debug.WriteLine($"REMOVED OUT OF PLACE BALL");
                }
            }

            List<IComplexFigure> cobjects;
            lock (CFigures) cobjects = CFigures.ToList();
            foreach (var figure in cobjects)
            {
                if (figure.IsOutOfBounds)
                {
                    lock (_syncClearItem) figure.Clear();
                    lock (CFigures) CFigures.Remove(figure);
                    Debug.WriteLine($"REMOVED OUT OF PLACE BALL");
                }
            }
            Thread.Sleep(30);
        }


        private void DoTheStepTimeout(float time)
        {
            lock (_locker)
            {
                if (_stepThread == null)
                {
                    _stepThread = new Thread(StepLoop);
                    _stepThread.IsBackground = true;
                    _stepThread.Start();
                }
            }

            _stepWorkDone = false;
            var sw = Stopwatch.StartNew();

            _doStepWork = () =>
            {
                World.Step(time);

                IEnumerable<IComplexFigure> cfigures;
                lock (Figures) cfigures = CFigures.ToList();
                foreach (var figure in cfigures)
                {
                    figure.Step(time);
                }
            };


            while (!_stepWorkDone)
            {
                Thread.Sleep(1);
                if (sw.ElapsedMilliseconds > 5000)
                {
                    Debug.WriteLine("ABORT SLOW CALC!");

                    IEnumerable<IFigure> removes;
                    lock (Figures) removes = Figures.AsEnumerable().Reverse().Take(Figures.Count / 10);
                    Debug.WriteLine($"Removing items: {removes.Count()}/{Figures.Count}");
                    lock (Figures)
                    {
                        lock (_syncClearItem)
                        {
                            foreach (var remove in removes)
                            {
                                remove.Clear();
                                Figures.Remove(remove);
                            }
                        }
                    }
                    _stepThread?.Abort();
                    _stepThread = null;
                    break;
                }
            }

        }


        private void StepLoop()
        {
            try
            {
                while (_runEngine)
                {
                    Thread.Sleep(5);
                    var action = _doStepWork;
                    if (action != null)
                    {
                        _doStepWork = null;

                        lock (_syncClearItem) action();

                        _stepWorkDone = true;
                    }
                }
            }
            catch (ThreadAbortException ex)
            {
                _stepThread = null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error Step: {ex.Message}");
                _stepThread = null;
            }
        }

        public void AddFigure(IFigure figure)
        {
            lock (_syncClearItem) Figures.Add(figure);
        }


        public void RemoveFigure(IFigure removeItem)
        {
            lock (_syncClearItem) removeItem.Clear();
        }


        public void AddComplexFigure(IComplexFigure figure)
        {
            lock (_syncClearItem) CFigures.Add(figure);
        }


        public void RemoveComplexFigure(IComplexFigure removeItem)
        {
            lock (_syncClearItem) removeItem.Clear();
        }

        public void RemoveLast10()
        {
            IEnumerable<IFigure> removes = null;
            lock (Figures) removes = Figures.AsEnumerable().Reverse().Take(10);

            Debug.WriteLine($"Removing items: {removes.Count()}/{Figures.Count}");
            lock (Figures)
            {
                foreach (var remove in removes)
                {
                    RemoveFigure(remove);
                    Figures.Remove(remove);
                }

            }
        }
    }
}
