﻿using System;
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
    public class BaseFigure : Base, IFigure
    {
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
        }

        public Vector2 LinearVelocity
        {
            get
            {
                return new Vector2() { X = ConvertUnits.ToDisplayUnits(Body.LinearVelocity.X), Y = ConvertUnits.ToDisplayUnits(Body.LinearVelocity.Y) };
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


        public BaseFigure(World world, int x, int y)
        {
            World = world;
            X = x;
            Y = y;
        }

        public void Create(int x, int y, Vertices shape, int radius = 0)
        {
            var location = ConvertUnits.ToSimUnits(x, y);

            Body = new Body(World, location, 0, 0);
            Body.BodyType = BodyType.Dynamic;
            Body.AngularVelocity = 0f;
            Body.Mass = 0.001f;
            //Body.LinearVelocity = new Vector2((float)(r.NextDouble() * 3f) - 1.5f, (float)(r.NextDouble() * 3f) - 1.5f);
            //Body.Restitution = restitution;
            //Body.Friction = friction;


            if (shape.Count <= 1)
            {
                //circle
                Shape = new CircleShape(ConvertUnits.ToSimUnits(radius), 0.3f);
            }
            else
            {
                Shape = new PolygonShape(shape, 1f);
            }

            try
            {
                Fixture = Body.CreateFixture(Shape);
            }
            catch (Exception)
            {

                throw;
            }
            //Fixture.Restitution = restitution;
            //Fixture.Friction = friction;

            OriginalVertices = shape;

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