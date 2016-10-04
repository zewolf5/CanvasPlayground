using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.SessionState;
using CanvasPlayground.Physics;
using Newtonsoft.Json;

namespace CanvasPlayground.Controllers
{
    //[Authorize]
    [SessionState(SessionStateBehavior.ReadOnly)]
    public class ValuesController : ApiController
    {
        static Stopwatch _sw = Stopwatch.StartNew();
        //// GET api/values
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        public object Get(int sizeX, int sizeY)
        {
            RenderingEngine.Instance.SetWorldBox(sizeX, sizeY);
            return "OK";
        }

        public object Get(string method, int clickX, int clickY)
        {
            RenderingEngine.Instance.AddBall(clickX, clickY);
            return "OK";
        }

        // GET api/values/5
        public object Get(string method)
        {
            //Debug.WriteLine($"calltime: {(int)sw.ElapsedMilliseconds} - {RenderingEngine.Instance.FrameNo}");

            _sw = Stopwatch.StartNew();

            if (method == "getObjectsxxx")
            {
                var scene = new SceneUpdate();
                scene.FrameNo = 0;
                scene.Objects = new List<ObjectInfo>();
                return scene;
            }

            if (method == "getObjects")
            {
                var scene = new SceneUpdate();
                scene.FrameNo = RenderingEngine.Instance.FrameNo;
                //scene.Objects = new List<ObjectInfo>();//RenderingEngine.Instance.GetObjects().ToList();
                scene.Objects = RenderingEngine.Instance.GetObjects().ToList();
                return scene;
            }

            //Debug.WriteLine($"calltime: {(int)sw.ElapsedMilliseconds} - {RenderingEngine.Instance.FrameNo}");
            _sw = Stopwatch.StartNew();

            if (method == "start")
            {
                RenderingEngine.Instance.Start();
                return "OK";
            }

            if (method == "stop")
            {
                RenderingEngine.Instance.Stop();
                return "OK";
            }

            if (method == "reverseGravity")
            {
                RenderingEngine.Instance.ReverseGravity();
                return "OK";
            }

            if (method == "addBall")
            {
                RenderingEngine.Instance.AddRandomBall();
                return "OK";
            }
            if (method == "addTriangle")
            {
                for (int i = 0; i < 3; i++) RenderingEngine.Instance.AddTriangle();
                return "OK";
            }
            if (method == "addRect")
            {
                for (int i = 0; i < 3; i++) RenderingEngine.Instance.AddRect();
                return "OK";
            }
            if (method == "addStuff")
            {
                RenderingEngine.Instance.CreateStuff();
                return "OK";
            }
            if (method == "addCross")
            {
                for (int i = 0; i < 3; i++) RenderingEngine.Instance.CreateCross();
                return "OK";
            }

            if (method == "add10Ball")
            {
                for (int i = 0; i < 10; i++) RenderingEngine.Instance.AddRandomBall();
                return "OK";
            }

            if (method == "removeItem")
            {
                RenderingEngine.Instance.RemoveLastItem();
                return "OK";
            }

            return "no method found";
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
