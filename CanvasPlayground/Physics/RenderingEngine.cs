using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
//using System.Drawing;
using System.Linq;
using System.Threading;
using System.Timers;
using CanvasPlayground.Physics.Figures;
//using System.Threading;
using FarseerPhysics;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Timer = System.Timers.Timer;

namespace CanvasPlayground.Physics
{
    public class RenderingEngine
    {

        //2 lists of all the objects
        List<IFigure> _figures = new List<IFigure>();
        List<IComplexFigure> _cfigures = new List<IComplexFigure>();

        //Frame counter
        public long FrameNo { get; internal set; }

        //Misc
        Random _random = new Random();
        private int _width = 800;
        private int _height = 800;
        private World _world = new World(new Vector2(0f, 2.82f));
        private DateTime _lastRun = DateTime.MinValue;



        #region Initialize Thread
        private bool _runEngine = true;
        private Thread _thread;
        private static RenderingEngine _instance;

        private static object _locker = new object();
        private static object _syncClearItem = new object();

        private Thread _stepThread = null;
        private Action _doStepWork = null;
        private bool _stepWorkDone = false;

        public static RenderingEngine Instance
        {
            get
            {
                if (_instance == null) _instance = new RenderingEngine();
                return _instance;
            }

        }
        public void Start()
        {
            Stop();
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
            lock (_figures) figures = _figures.ToList();
            foreach (var figure in figures)
            {
                lock (_syncClearItem) figure.Clear();
            }
            _figures.Clear();

            IEnumerable<IComplexFigure> cfigures;
            lock (_figures) cfigures = _cfigures.ToList();
            foreach (var figure in cfigures)
            {
                lock (_syncClearItem) figure.Clear();
            }
            _cfigures.Clear();

            _thread?.Abort();
            _stepThread?.Abort();
            _stepThread = null;
        }


        #endregion

        public void SetWorldBox(int width, int height)
        {
            this._width = width;
            this._height = height;
        }


        public void InitializeEngineLoop()
        {
            Debug.WriteLine("Engine Started");
            try
            {
                _world = new World(new Vector2(0f, 2.82f));
                CreateObstacles();

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
            lock (_figures) objects = _figures.ToList();
            foreach (var figure in objects)
            {
                if (figure.IsOutOfBounds)
                {
                    lock (_syncClearItem) figure.Clear();
                    lock (_figures) _figures.Remove(figure);
                    Debug.WriteLine($"REMOVED OUT OF PLACE BALL");
                }
            }

            List<IComplexFigure> cobjects;
            lock (_cfigures) cobjects = _cfigures.ToList();
            foreach (var figure in cobjects)
            {
                if (figure.IsOutOfBounds)
                {
                    lock (_syncClearItem) figure.Clear();
                    lock (_cfigures) _cfigures.Remove(figure);
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
                _world.Step(time);

                IEnumerable<IComplexFigure> cfigures;
                lock (_figures) cfigures = _cfigures.ToList();
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
                    lock (_figures) removes = _figures.AsEnumerable().Reverse().Take(_figures.Count / 10);
                    Debug.WriteLine($"Removing items: {removes.Count()}/{_figures.Count}");
                    lock (_figures)
                    {
                        lock (_syncClearItem)
                        {
                            foreach (var remove in removes)
                            {
                                remove.Clear();
                                _figures.Remove(remove);
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

        public IEnumerable<ObjectInfo> GetObjects()
        {
            List<IFigure> objects;
            lock (_figures) objects = _figures.ToList();
            foreach (var figure in objects)
            {
                if (figure is Circle)
                {
                    yield return new ObjectInfo() { P = figure.Position, C = figure.HtmlColor, I = figure.Id, D = figure.LinearVelocity };
                }
                else
                {
                    yield return new ObjectInfo() { P = figure.Position, C = figure.HtmlColor, I = figure.Id, D = figure.LinearVelocity, V = figure.Vertices };
                }
            }

            List<IComplexFigure> cobjects;
            lock (_cfigures) cobjects = _cfigures.ToList();
            foreach (var cfigure in cobjects)
            {
                foreach (var figure in cfigure.Figures)
                {
                    yield return new ObjectInfo() { P = figure.Position, C = figure.HtmlColor, I = figure.Id, D = figure.LinearVelocity, V = figure.Vertices };
                }

            }

        }

        public void ReverseGravity()
        {
            _world.Gravity = new Vector2(-_world.Gravity.X, -_world.Gravity.Y);
        }

        public void AddRandomBall()
        {
            lock (_figures) _figures.Add(new Circle(_world, 15, _random.Next(_width), _random.Next(_height)) { Restitution = 0.95f, SleepingAllowed = false });
        }
        public void AddBall(int x, int y)
        {
            lock (_figures) _figures.Add(new Circle(_world, 15, x, y) { Restitution = 0.95f, SleepingAllowed = false });
        }


        public void AddRect()
        {
            lock (_figures) _figures.Add(new Rectangle(_world, 40, 40, 0, _random.Next(_width), _random.Next(_height)) { Restitution = 0.95f });
        }



        public void AddTriangle()
        {
            lock (_figures) _figures.Add(new Triangle(_world, 100, 100, 100, _random.Next(_width), _random.Next(_height)) { Restitution = 0.95f, Mass = 1f });
        }

        public void CreateObstacles()
        {
            lock (_cfigures) _cfigures.Add(new HollowRectangle(_world, _width, _height, 25, _width / 2, _height / 2) { Restitution = 1f, Static = true, Density = 1f, Friction = 0, RotationPerSecond = 0.0f });

            //lock (_cfigures) _cfigures.Add(
            //    new HollowCircleWithInnerSpikes(_world, _height / 2, 25, _width / 2, _height / 2)
            //    { Restitution = 0.25f, Static = true, RotationPerSecond = 0.7f });
        }


        public void CreateCross()
        {
            lock (_cfigures) _cfigures.Add(new Cross(_world, 40, 40, 5, _random.Next(_width), _random.Next(_height)) { Restitution = 0.95f });
        }

        public void CreateStuff()
        {
            lock (_cfigures) _cfigures.Add(new HollowCircle(_world, 140, 25, _random.Next(_width), _random.Next(_height)) { Restitution = 0.95f, Static = true, RotationPerSecond = 0.1f });

        }

        public void RemoveLastItem()
        {

            IEnumerable<IFigure> removes = null;
            lock (_figures) removes = _figures.AsEnumerable().Reverse().Take(10);

            Debug.WriteLine($"Removing items: {removes.Count()}/{_figures.Count}");
            lock (_figures)
            {
                lock (_syncClearItem)
                {
                    foreach (var remove in removes)
                    {
                        remove.Clear();
                        _figures.Remove(remove);
                    }

                }
            }
        }
    }


}

