using System;
using System.Drawing;
using System.Linq;
using ChipmunkSharp;
using FarseerPhysics;

namespace CanvasPlayground.Physics.Figures
{
    public class BaseFigure : Base, IFigure
    {
        public PWorld World { get; private set; }
        public cpBody Body { get; private set; }
        public cpShape Shape { get; private set; }
        //public Fixture Fixture { get; private set; }
        public Vertices OriginalVertices { get; private set; }

        public Vertices Vertices
        {
            get
            {
                if (Shape is cpPolyShape)
                {
                    //return RotateVerticesAndToDisplayUnits(((cpPolyShape)Shape).GetVertices().Select(o=>new Vector2(o.x, o.y)).ToList(), Body.Rotation);
                    var pos = Body.GetPosition();
                    return new Vertices(((cpPolyShape)Shape).GetVertices().Select(o => new Vector2(o.x - pos.x, o.y - pos.y)));
                }
                else
                {
                    return new Vertices();
                }
            }
        }

        public Vector2 Position
        {
            get { return new Vector2((int)Body.GetPosition().x, (int)Body.GetPosition().y); }
        }

        public Vector2 LinearVelocity
        {
            get { return new Vector2((int)Body.GetVelocity().x, (int)Body.GetVelocity().y); }
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


        public float Restitution
        {
            get { return Shape.GetElasticity(); }
            set { Shape.SetElasticity(value); }
        }

        public float Friction
        {
            get { return Shape.GetFriction(); }
            set { Shape.SetFriction(value); }
        }

        public float Mass
        {
            get { return Body.GetMass(); }
            set
            {
                if (Shape.GetBody().bodyType == cpBodyType.DYNAMIC)
                {
                    Body.SetMass(value);
                }
            }
        }

        public bool Static
        {
            get
            {
                return Shape.GetBody().bodyType == cpBodyType.STATIC;
            }
            set
            {
                if (Shape.GetBody().bodyType == cpBodyType.STATIC && !value)
                {
                    var oldPos = Body.GetPosition();
                    var oldMass = Body.GetMass();
                    var oldMoment = Body.GetMoment();
                    World.Space.RemoveBody(Body);
                    Body = World.Space.AddBody(new cpBody(oldMass, oldMoment));
                    Body.SetPosition(oldPos);
                    Shape.SetBody(Body);
                }
                else
                {
                    var oldPos = Body.GetPosition();
                    World.Space.RemoveBody(Body);
                    Body = World.Space.AddBody(cpBody.NewStatic());
                    Body.SetPosition(oldPos);
                    Shape.SetBody(Body);
                }
            }
        }

        public bool SleepingAllowed
        {

            get { return false; }
            set { }
        }


        public BaseFigure(PWorld world, int x, int y)
        {
            World = world;
            X = x;
            Y = y;
        }

        public void Create(int x, int y, Vertices shape, int radius = 1)
        {

            float mass = 1;

            float moment = cp.MomentForCircle(mass, 0, radius, cpVect.Zero);




            //var location = ConvertUnits.ToSimUnits(x, y);

            //Body = new Body(World, location, 0, 0);
            //Body.BodyType = BodyType.Dynamic;
            //Body.AngularVelocity = 0f;
            //Body.Mass = 0.001f;
            ////Body.LinearVelocity = new Vector2((float)(r.NextDouble() * 3f) - 1.5f, (float)(r.NextDouble() * 3f) - 1.5f);
            ////Body.Restitution = restitution;
            ////Body.Friction = friction;


            if (shape.Count <= 1)
            {
                Body = World.Space.AddBody(new cpBody(mass, moment));
                Body.SetPosition(new cpVect(x, y));
                Body.SetVelocity(new cpVect((float)(r.NextDouble() * 100 - 50), (float)(r.NextDouble() * 100 - 50)));
                //circle
                Shape = World.Space.AddShape(new cpCircleShape(Body, radius, cpVect.Zero));
                Shape.SetFriction(0.7f);
                Shape.SetElasticity(0.5f);
                Shape.SetCollisionType(1);
            }
            else
            {
                Body = World.Space.AddBody(new cpBody(mass, moment));
                Body.SetPosition(new cpVect(x, y));

                Shape = World.Space.AddShape(new cpPolyShape(Body, shape.Count, shape.Select(o => new cpVect(o.X, o.Y)).ToArray(), radius));
                Shape.SetFriction(0.7f);
                Shape.SetCollisionType(1);
            }

            //try
            //{
            //    Fixture = Body.CreateFixture(Shape);
            //}
            //catch (Exception)
            //{

            //    throw;
            //}
            ////Fixture.Restitution = restitution;
            ////Fixture.Friction = friction;

            //OriginalVertices = shape;

        }

        public void Clear()
        {
            World.Space.RemoveBody(Body);
            World.Space.RemoveShape(Shape);
        }

        public bool IsOutOfBounds
        {
            get { return Body.GetPosition().x < -1000 || Body.GetPosition().x > 4000 || Body.GetPosition().y < -1000 || Body.GetPosition().y > 3000; }
        }

        internal Vertices RotateVerticesAndToDisplayUnits(Vertices verts, float angle)
        {
            var list = new Vertices();

            //Matrix rotation = Matrix.Identity;
            //rotation *= Matrix.CreateRotationZ(angle);
            //for (int i = 0; i < verts.Count; i++)
            //{
            //    var vector = ConvertUnits.ToDisplayUnits(verts[i]);
            //    vector = Vector2.Transform(vector, rotation);
            //    list.Add(new Vector2((int)vector.X, (int)vector.Y));
            //}
            return list;
        }

        internal Vertices RotateVertices(Vertices verts, float angle)
        {
            var list = new Vertices();

            //Matrix rotation = Matrix.Identity;
            //rotation *= Matrix.CreateRotationZ(angle);
            //for (int i = 0; i < verts.Count; i++)
            //{
            //    var vector = verts[i];
            //    vector = Vector2.Transform(vector, rotation);
            //    list.Add(new Vector2(vector.X, vector.Y));
            //}
            return list;
        }
    }
}
