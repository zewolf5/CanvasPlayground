using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using CanvasPlayground.Models;
using CanvasPlayground.Physics.Figures;
using CanvasPlayground.Utils;
using FarseerPhysics;
using FarseerPhysics.Collision;
using Microsoft.Xna.Framework;
using Timer = System.Timers.Timer;

namespace CanvasPlayground.Physics.Scenes
{
    public class WoF3Scene : IScene
    {
        private List<IFigure> _sceneFigures = new List<IFigure>();
        private List<IComplexFigure> _sceneCFigures = new List<IComplexFigure>();

        private List<IFigure> _roundTwoBalls = new List<IFigure>();
        private List<IFigure> _roundOneBalls = new List<IFigure>();

        private IComplexFigure _wofObject;
        private IFigure _rollRectangle;

        private IFigure _waitRectangle;

        private Info _countDownInfo;

        static Random random = new Random();
        private WorldLoop _worldLoop;
        public WoF3Scene(WorldLoop worldLoop)
        {
            _worldLoop = worldLoop;
        }

        public void Start()
        {
            InitializeScene();


            int lastId = -1;
            int countId = 0;

            int countDown = 10;

            while (_worldLoop != null)
            {
                //Do something/wait for something/ react to stuff as long as the scene is active

                if (_rollRectangle != null)
                {

                    if (countDown > 0)
                    {
                        var testAABB = new AABB();
                        testAABB.UpperBound = new Vector2(ConvertUnits.ToSimUnits(325), ConvertUnits.ToSimUnits(415));
                        testAABB.LowerBound = new Vector2(ConvertUnits.ToSimUnits(325), ConvertUnits.ToSimUnits(415));


                        _worldLoop.World.QueryAABB((fixture) =>
                        {
                            var figure = fixture.Tag as IFigure;
                            if (!figure.Static && figure.Position.Y >= 430) //320
                            {
                                if (lastId != figure.Id)
                                {
                                    lastId = figure.Id;
                                    countId = 0;
                                }
                                countId++;
                                Debug.WriteLine($"Touching goal: {figure.Id} - {figure.HtmlColor} - #{countId} - {figure.Position.X},{figure.Position.Y}");

                                if (countId >= 5)
                                {
                                    lastId = -1;
                                    MoveBallToNextPart(figure);
                                    var str = (--countDown).ToString();
                                    if (_countDownInfo == null)
                                    {
                                        _countDownInfo = new Info() { F = "Arial", P = new Vector2(1125, 85), S = 60, T = str };
                                        _worldLoop.TextInfos.Add(_countDownInfo);
                                        _worldLoop.TextInfos.Add(new Info() { F = "Arial", P = new Vector2(775, 85), S = 60, T = "Baller igjen:" });
                                    }
                                    else
                                    {
                                        _countDownInfo.T = str;
                                        _countDownInfo.S += 5;
                                    }

                                }
                            }
                            return true;
                        },
                            ref testAABB);
                    }
                    else
                    {
                        if (_waitRectangle != null)
                        {
                            _worldLoop.TextInfos.Clear();
                            foreach (var roundOneBall in _roundOneBalls)
                            {
                                roundOneBall.Static = true;
                            }

                            _worldLoop.World.Gravity = new Vector2(0, -1);
                            var rect = _waitRectangle;
                            _waitRectangle = null;
                            _wofObject.RotationPerSecond = 0f;
                            int cdown = 5;

                            _countDownInfo = new Info() { F = "Arial", P = new Vector2(900, 75), S = 60, T = $"Countdown: {cdown}" };
                            _worldLoop.TextInfos.Add(_countDownInfo);

                            SceneTimer.RunSetup(new List<Tuple<int, Action>> {
                            new Tuple<int, Action>(1000, () => {_countDownInfo.T = $"Countdown: {--cdown}";}),
                            new Tuple<int, Action>(2000, () => {_countDownInfo.T = $"Countdown: {--cdown}";}),
                            new Tuple<int, Action>(2500, () => {StartZigZaggers();}),
                            new Tuple<int, Action>(3000, () => {_countDownInfo.T = $"Countdown: {--cdown}";}),
                            new Tuple<int, Action>(4000, () => {_countDownInfo.T = $"Countdown: {--cdown}";}),
                            new Tuple<int, Action>(5000, () => {_countDownInfo.T = $"Countdown: {--cdown}";}),
                            new Tuple<int, Action>(5500, () => { _worldLoop.World.Gravity= new Vector2(0,4);}),
                            new Tuple<int, Action>(6000, () => {
                                _countDownInfo.T = $"GO";
                                _worldLoop.RemoveFigure(rect);

                                foreach (var roundTwoBall in _roundTwoBalls)
                                {
                                    roundTwoBall.LinearVelocity = new Vector2(random.Next(25), random.Next(25));
                                }
                            }),
                            new Tuple<int, Action>(8000, () => {
                                _worldLoop.TextInfos.Clear();
                            }),
                        });
                        }
                    }

                }

                Thread.Sleep(200);
            }
        }

        private void MoveBallToNextPart(IFigure ball)
        {
            ball.Position = new Vector2(random.Next(600) + 900, 50);
            ball.LinearVelocity = new Vector2(random.Next(50), random.Next(50));
            _roundTwoBalls.Add(ball);
            _roundOneBalls.Remove(ball);
        }

        private void InitializeScene()
        {
            _wofObject = AddFigure(() => new HollowCircleWithInnerSpikes(_worldLoop.World, 350, 30, 80, 20, 350, 350, "#0f0", true));
        }


        private IFigure AddFigure(Func<IFigure> figureAction)
        {
            var figure = _worldLoop?.CreateFigure(figureAction);
            _sceneFigures.Add(figure);
            return figure;
        }

        private IComplexFigure AddFigure(Func<IComplexFigure> figureAction)
        {
            var figure = _worldLoop?.CreateComplexFigure(figureAction);
            _sceneCFigures.Add(figure);
            return figure;
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
                    AddFigure(() => new Circle(_worldLoop.World, 15, 400, 400));
                    Thread.Sleep(100);
                }
            }
            if (eventString == "DropTheBalls")
            {
                for (int i = 0; i < 100; i++)
                {
                    var ball = AddFigure(() => new Circle(_worldLoop.World, 15, 350 + random.Next(400) - 200, 350 + random.Next(400) - 200) { Restitution = 0.1f, Mass = 0.4f, Friction = 1f, SleepingAllowed = false });
                    _roundOneBalls.Add(ball);
                    Thread.Sleep(25);
                }


                var rect = AddFigure(()=>new HollowRectangle(_worldLoop.World, 800, 800, 20, 1150, 400, "blue", true) { Restitution = 0.8f, Friction = 1f });


                SceneTimer.RunSetup(new List<Tuple<int, Action>> {
                    new Tuple<int, Action>(1000, () => {
                        _wofObject.RotationPerSecond = 0.8f;
                    }),
                    new Tuple<int, Action>(3500, () => {
                        CreateBallCourt();
                    }),

                    new Tuple<int, Action>(4500, () => {
                        AddFigure(()=>new Rectangle(_worldLoop.World, 175, 10, 0.3f, 300, 330, "blue", true) { Restitution = 0f });
                    }),
                    new Tuple<int, Action>(5500, () => {
                        AddFigure(()=>new Rectangle(_worldLoop.World, 175, 10, -0.3f, 430, 260, "blue", true) { Restitution = 0f });

                    }),
                    new Tuple<int, Action>(6500, () => {
                        AddFigure(()=>new Rectangle(_worldLoop.World, 175, 10, 0.3f, 300, 150, "blue", true) { Restitution = 0f });
                    }),
                    new Tuple<int, Action>(4000, () => {
                        _waitRectangle = AddFigure(()=>new Rectangle(_worldLoop.World, 800, 20, 0f, 1150, 150, "red", true) { Restitution = 0f });

                    }),
                });

            }
        }

        private void CreateBallCourt()
        {
            //_rollRectangle = AddFigure(new Rectangle(_worldLoop.World, 85, 20, 0.3f, 295, 340, "red", true) { Restitution = 0f });
            //_rollRectangle = AddFigure(new Rectangle(_worldLoop.World, 150, 20, -0.3f, 390, 330, "red", true) { Restitution = 0f });

            _rollRectangle = AddFigure(() => new Rectangle(_worldLoop.World, 85, 20, 0.6f, 295, 440, "red", true) { Restitution = 0f });
            AddFigure(() => new Rectangle(_worldLoop.World, 150, 20, -0.6f, 390, 430, "red", true) { Restitution = 0f });

            //AddFigure(new Rectangle(_worldLoop.World, 10, 200, 0, 250, 300, "red", true) { Restitution = 0f });
            //AddFigure(new Rectangle(_worldLoop.World, 10, 200, 0, 450, 300, "red", true) { Restitution = 0f });
        }

        private void StartZigZaggers()
        {
            for (int i = 825; i < 750 + 750; i += 50)
            {
                AddFigure(() => new Triangle(_worldLoop.World, 0.15f, (float)random.NextDouble(), i, 150, "#000", true));
            }

            ////var rects = new List<IFigure>();


            bool oddRow = true;
            for (int j = 250; j < 650; j += 50)
            {
                oddRow = !oddRow;
                for (int i = 805; i < 750 + 725; i += 100)
                {
                    var left = i + (50 / 2);
                    if (oddRow) left += 50;

                    var rect = AddFigure(() => new Rectangle(_worldLoop.World, 50, 5, 0, left, j, null, true));

                    rect.OnCollision += Rect_OnCollision;
                }
            }

            //var arcHeight = 10;
            //var arcWidth = 50;
            ////var rects = new List<IFigure>();

            //for (int j = 250; j < 650; j += arcHeight)
            //{
            //    for (int i = 775; i < 750 + 800; i += arcWidth)
            //    {
            //        var rect = AddFigure(new Rectangle(_worldLoop.World, arcWidth, arcHeight, 0, i + (arcHeight / 2), j, null, true));
            //        bool noCollisionMode = false;//j > 450 && random.NextDouble() < 0.1f;
            //        if (!noCollisionMode)
            //        {
            //            rect.OnCollision += Rect_OnCollision;
            //        }
            //        else
            //        {
            //            rect.OnCollision += Rect_OnCollisionHard;
            //            rect.HtmlColor = "#000";
            //        }
            //    }
            //}
        }


        private bool Rect_OnCollision(IFigure arg1, IFigure arg2, FarseerPhysics.Dynamics.Contacts.Contact arg3)
        {
            arg2.LinearVelocity *= 1.1f;
            if (Math.Abs((int)arg2.LinearVelocity.X * (int)arg2.LinearVelocity.Y) > 5000) arg2.LinearVelocity *= 0.9f;
            //Task.Run(() =>
            //{
            //    Thread.Sleep(25);
            //    //_worldLoop.RemoveFigure(arg1);
            //    arg2.LinearVelocity *= 1.1f;
            //    if (Math.Abs((int)arg2.LinearVelocity.X * (int)arg2.LinearVelocity.Y) > 5000) arg2.LinearVelocity *= 0.9f;

            //});
            return true;
        }

        private bool Rect_OnCollisionHard(IFigure arg1, IFigure arg2, FarseerPhysics.Dynamics.Contacts.Contact arg3)
        {
            arg2.LinearVelocity *= 1.1f;
            if (Math.Abs((int)arg2.LinearVelocity.X * (int)arg2.LinearVelocity.Y) > 5000) arg2.LinearVelocity *= 0.9f;
            return true;
        }

    }
}
