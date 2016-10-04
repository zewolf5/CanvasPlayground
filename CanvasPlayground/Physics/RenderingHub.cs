using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
//using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using CanvasPlayground.Models;
using CanvasPlayground.Physics.Figures;
using CanvasPlayground.Physics.Scenes;
using CanvasPlayground.Utils;
//using System.Threading;
using FarseerPhysics;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Timer = System.Timers.Timer;

namespace CanvasPlayground.Physics
{
    public class RenderingHub
    {

        //2 lists of all the objects

        public long FrameNo => _theWorldLoop.FrameNo;

        private IScene _currentScene;

        //Misc
        Random _random = new Random();
        private int _width = 800;
        private int _height = 800;


        private static RenderingHub _instance;
        private WorldLoop _theWorldLoop;
        public static RenderingHub Instance
        {
            get
            {
                if (_instance == null) _instance = new RenderingHub();
                return _instance;
            }

        }

        public RenderingHub()
        {

            _theWorldLoop = new WorldLoop();
            _theWorldLoop.Start(new Vector2(0, 10), 20);
        }

        public void SceneStart(string name)
        {
            var type = Misc.GetTypesByName(name);
            _currentScene?.Stop();
            _currentScene = (IScene)Activator.CreateInstance(type, _theWorldLoop);
            Task.Run(() =>
            {
                _currentScene?.Start();
            });
        }

        public void SceneEvent(string eventName)
        {
            Task.Run(() =>
            {
                _currentScene?.SendEvent(eventName);
            });

        }

        public void SetWorldBox(int width, int height)
        {
            this._width = width;
            this._height = height;
        }

        public void Stop()
        {
            _theWorldLoop.Stop();
            _currentScene?.Stop();
        }

        public void Start()
        {
            _theWorldLoop.Start();
            CreateWorldBox();
        }


        public IEnumerable<ObjectInfo> GetObjects()
        {
            List<IFigure> objects;
            lock (_theWorldLoop.Figures) objects = _theWorldLoop.Figures.ToList();
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
            lock (_theWorldLoop.CFigures) cobjects = _theWorldLoop.CFigures.ToList();
            foreach (var cfigure in cobjects)
            {
                foreach (var figure in cfigure.Figures)
                {
                    yield return new ObjectInfo() { P = figure.Position, C = figure.HtmlColor, I = figure.Id, D = figure.LinearVelocity, V = figure.Vertices };
                }

            }

        }

        public void CreateWorldBox()
        {
            _theWorldLoop.AddComplexFigure(new HollowRectangle(_theWorldLoop.World, _width, _height, 25, _width / 2, _height / 2) { Restitution = 0.9f, Static = true, Density = 1f, Friction = 0, RotationPerSecond = 0.0f });
        }


        public void ReverseGravity()
        {
            _theWorldLoop.World.Gravity = new Vector2(-_theWorldLoop.World.Gravity.X, -_theWorldLoop.World.Gravity.Y);
        }

        public void AddRandomBall()
        {
            _theWorldLoop.AddFigure(new Circle(_theWorldLoop.World, 15, _random.Next(_width), _random.Next(_height)) { Restitution = 0.95f, SleepingAllowed = false });
        }
        public void AddBall(int x, int y)
        {
            _theWorldLoop.AddFigure(new Circle(_theWorldLoop.World, 15, x, y) { Restitution = 0.95f, SleepingAllowed = false });
        }


        public void AddRect()
        {
            _theWorldLoop.AddFigure(new Rectangle(_theWorldLoop.World, 40, 40, 0, _random.Next(_width), _random.Next(_height)) { Restitution = 0.95f });
        }



        public void AddTriangle()
        {
            _theWorldLoop.AddFigure(new Triangle(_theWorldLoop.World, 100, 100, 100, _random.Next(_width), _random.Next(_height)) { Restitution = 0.95f, Mass = 1f });
        }


        public void BigWheel()
        {

            _theWorldLoop.AddComplexFigure(
                new HollowCircleWithInnerSpikes(_theWorldLoop.World, _height / 2, 25, _width / 2, _height / 2)
                { Restitution = 0.25f, Static = true, RotationPerSecond = 0.7f });
        }

        public void CreateCross()
        {
            _theWorldLoop.AddComplexFigure(new Cross(_theWorldLoop.World, 40, 40, 5, _random.Next(_width), _random.Next(_height)) { Restitution = 0.95f });
        }

        public void CreateStuff()
        {
            _theWorldLoop.AddComplexFigure(new HollowCircle(_theWorldLoop.World, 140, 25, _random.Next(_width), _random.Next(_height)) { Restitution = 0.95f, Static = true, RotationPerSecond = 0.1f });

        }

        public void RemoveLastItem()
        {
            _theWorldLoop.RemoveLast10();
        }
    }


}

