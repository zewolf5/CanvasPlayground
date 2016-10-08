using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public class BaseFigure : Base, IFigure
    {
        public event Func<IFigure, IFigure, FarseerPhysics.Dynamics.Contacts.Contact, bool> OnCollision;

        public void Create(int x, int y, Vertices shape, int? radius = null, bool? isStatic = null)
        {
            try
            {
                var location = ConvertUnits.ToSimUnits(x, y);

                Body = new Body(World, location, 0, 0);
                Body.BodyType = BodyType.Dynamic;
                Body.AngularVelocity = 0f;
                Body.Mass = 0.001f;
                if (isStatic != null) Body.IsStatic = isStatic.Value;

                if (shape.Count <= 1)
                {
                    //circle
                    Shape = new CircleShape(ConvertUnits.ToSimUnits(radius.Value), 0.3f);
                }
                else
                {
                    Shape = new PolygonShape(shape, 1f);
                }

                Fixture = Body.CreateFixture(Shape);
                Body.OnCollision += Body_OnCollision;

                OriginalVertices = shape;

                Fixture.Tag = this;
                Shape.Tag = this;
            }
            catch (Exception ex)
            {
                if (World != null) throw;
                Debug.WriteLine($"Shape creation failed ({Id}): {ex.Message}");

            }
        }

        private bool Body_OnCollision(Fixture fixtureA, Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {
            return OnCollision?.Invoke(fixtureA.Tag as IFigure, fixtureB.Tag as IFigure, contact) ?? true;
        }

        public World World { get; private set; }
        public Body Body { get; private set; }
        public Shape Shape { get; private set; }
        public Fixture Fixture { get; private set; }
        public Vertices OriginalVertices { get; private set; }

        public Vertices Vertices
        {
            get
            {
                if (Shape is PolygonShape)
                {
                    return RotateVerticesAndToDisplayUnits(((PolygonShape)Shape).Vertices, Body.Rotation);
                }
                else
                {
                    return new Vertices();
                }
            }
        }

        public Vector2 Position
        {
            get
            {
                return new Vector2() { X = (int)ConvertUnits.ToDisplayUnits(Body.Position.X), Y = (int)ConvertUnits.ToDisplayUnits(Body.Position.Y) };
            }
            set
            {
                Body.Position= new  Vector2() { X = ConvertUnits.ToSimUnits(value.X), Y = ConvertUnits.ToSimUnits(value.Y) };
            }
        }

        public Vector2 LinearVelocity
        {
            get
            {
                return new Vector2() { X = ConvertUnits.ToDisplayUnits(Body.LinearVelocity.X), Y = ConvertUnits.ToDisplayUnits(Body.LinearVelocity.Y) };
            }
            set
            {
                Body.LinearVelocity = new Vector2() { X = ConvertUnits.ToSimUnits(value.X), Y = ConvertUnits.ToSimUnits(value.Y) };
            }
        }


        public int X { get; private set; }
        public int Y { get; private set; }

        private string _htmlColor;
        public string HtmlColor
        {
            get
            {
                if (_htmlColor == null)
                {
                    var color = ColorTranslator.ToHtml(Color.FromArgb(255, (byte)r.Next(255), (byte)r.Next(255), (byte)r.Next(255)));
                    _htmlColor = color.Substring(0, 2) + color.Substring(3, 1) + color.Substring(5, 1);
                }
                return _htmlColor;
            }
            set { _htmlColor = value; }
        }

        public float Density
        {
            get { return Shape.Density; }
            set { Shape.Density = value; }
        }

        public float Restitution
        {
            get { return Fixture.Restitution; }
            set { Fixture.Restitution = value; }
        }

        public float Friction
        {
            get { return Fixture.Friction; }
            set { Fixture.Friction = value; }
        }

        public float Mass
        {
            get { return Body.Mass; }
            set { Body.Mass = value; }
        }

        public bool Static
        {
            get { return Body.IsStatic; }
            set { Body.IsStatic = value; }
        }

        public bool SleepingAllowed
        {
            get { return Body.SleepingAllowed; }
            set { Body.SleepingAllowed = value; }
        }


        public BaseFigure(World world, int x, int y, string color = null)
        {
            World = world;
            X = x;
            Y = y;
            HtmlColor = color;
        }


        public void Clear()
        {
            World.RemoveBody(Body);
            Body.Dispose();
            Fixture.Dispose();
        }

        public bool IsOutOfBounds
        {
            get { return Body.Position.X < -10 || Body.Position.X > 30 || Body.Position.Y < -10 || Body.Position.Y > 40; }
        }

        internal Vertices RotateVerticesAndToDisplayUnits(Vertices verts, float angle)
        {
            var list = new Vertices();

            Matrix rotation = Matrix.Identity;
            rotation *= Matrix.CreateRotationZ(angle);
            for (int i = 0; i < verts.Count; i++)
            {
                var vector = ConvertUnits.ToDisplayUnits(verts[i]);
                vector = Vector2.Transform(vector, rotation);
                list.Add(new Vector2((int)vector.X, (int)vector.Y));
            }
            return list;
        }

        internal Vertices RotateVertices(Vertices verts, float angle)
        {
            var list = new Vertices();

            Matrix rotation = Matrix.Identity;
            rotation *= Matrix.CreateRotationZ(angle);
            for (int i = 0; i < verts.Count; i++)
            {
                var vector = verts[i];
                vector = Vector2.Transform(vector, rotation);
                list.Add(new Vector2(vector.X, vector.Y));
            }
            return list;
        }
    }
}
