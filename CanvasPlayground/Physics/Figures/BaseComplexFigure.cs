using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FarseerPhysics;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace CanvasPlayground.Physics.Figures
{
    public class BaseComplexFigure : Base, IComplexFigure
    {
        public int Id { get; }
        public World World { get; }
        public List<IFigure> Figures { get; } = new List<IFigure>();
        public int X { get; }
        public int Y { get; }
        public Vector2 Position { get; }


        private float _currentAngle;

        public void Step(float seconds)
        {
            if (RotationPerSecond != 0)
            {
                var stepAngle = RotationPerSecond * seconds;
                _currentAngle += stepAngle;

                foreach (var figure in Figures)
                {
                    var posO = figure.Body.Position;
                    var pos = ConvertUnits.ToDisplayUnits(posO);
                    //figure.Body.SetTransform(pos, _currentAngle);
                    figure.Body.SetTransform(Vector2.Zero, _currentAngle);
                    figure.Body.SetTransform(posO, _currentAngle);

                    var vec = new Vector2(pos.X - X, pos.Y - Y);
                    var pos2 = RotateVector2(vec, stepAngle);
                    var vec2 = new Vector2(pos2.X + X, pos2.Y + Y);
                    figure.Body.Position = ConvertUnits.ToSimUnits(vec2);

                    //figure.Body.SetTransform(Vector2.Zero, _currentAngle);
                }


                //Rotate each element
                //foreach (var figure in Figures)
                //{
                //    var pos = figure.Body.Position;
                //    figure.Body.SetTransform(Vector2.Zero, _currentAngle);
                //    figure.Body.SetTransform(pos, _currentAngle);
                //}

            }
        }

        public BaseComplexFigure(World world, int x, int y, string color = null)
        {
            World = world;
            X = x;
            Y = y;
        }

        public float Density
        {
            get
            {
                foreach (var figure in Figures)
                {
                    return figure.Density;
                }
                return 0;
            }
            set
            {
                foreach (var figure in Figures)
                {
                    figure.Density = value;
                }
            }


        }

        public float Mass
        {
            get
            {
                foreach (var figure in Figures)
                {
                    return figure.Mass;
                }
                return 0;
            }
            set
            {
                foreach (var figure in Figures)
                {
                    figure.Mass = value;
                }
            }
        }

        public float Restitution
        {
            get
            {
                foreach (var figure in Figures)
                {
                    return figure.Restitution;
                }
                return 0;
            }
            set
            {
                foreach (var figure in Figures)
                {
                    figure.Restitution = value;
                }
            }
        }
        public float Friction
        {
            get
            {
                foreach (var figure in Figures)
                {
                    return figure.Friction;
                }
                return 0;
            }
            set
            {
                foreach (var figure in Figures)
                {
                    figure.Friction = value;
                }
            }
        }

        public bool Static
        {
            get
            {
                foreach (var figure in Figures)
                {
                    return figure.Static;
                }
                return false;
            }
            set
            {
                foreach (var figure in Figures)
                {
                    figure.Static = value;
                }
            }
        }

        public void Clear()
        {
            foreach (var figure in Figures)
            {
                figure.Clear();
            }
        }

        public bool IsOutOfBounds
        {
            get
            {
                bool isTrue = false;
                foreach (var figure in Figures)
                {
                    if (figure.IsOutOfBounds) isTrue = true;
                }
                return isTrue;
            }
        }

        public string HtmlColor
        {
            get
            {
                foreach (var figure in Figures)
                {
                    return figure.HtmlColor;
                }
                return "white";
            }
        }

        public Vector2 LinearVelocity
        {
            get
            {
                foreach (var figure in Figures)
                {
                    return figure.LinearVelocity;
                }
                return Vector2.Zero;
            }
        }

        public float RotationPerSecond { get; set; }


        internal Vector2 RotateVector2(Vector2 v1, float angle)
        {
            Matrix rotation = Matrix.Identity;
            rotation *= Matrix.CreateRotationZ(angle);
            var vector = v1;
            vector = Vector2.Transform(vector, rotation);
            return new Vector2(vector.X, vector.Y);
        }


    }
}
