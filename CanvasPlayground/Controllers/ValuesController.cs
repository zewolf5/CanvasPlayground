﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.SessionState;
using CanvasPlayground.Models;
using CanvasPlayground.Physics;
using Newtonsoft.Json;

namespace CanvasPlayground.Controllers
{
    //[Authorize]
    [SessionState(SessionStateBehavior.ReadOnly)]
    public class ValuesController : ApiController
    {

        private static Lazy<Timer> updateTimer = new Lazy<Timer>(() => new Timer(UpdateClientsCallback, null, 0, 15));
        private static ConcurrentDictionary<StreamWriter, bool> clientSubscribers = new ConcurrentDictionary<StreamWriter, bool>();


        public object Get(int sizeX, int sizeY)
        {
            RenderingHub.Instance.SetWorldBox(sizeX, sizeY);
            return "OK";
        }

        public object Get(string method, int clickX, int clickY)
        {
            RenderingHub.Instance.AddBall(clickX, clickY);
            Debug.WriteLine($"clicked: {clickX},{clickY}");
            return "OK";
        }




        // GET api/values/5
        public object Get(string method)
        {
            var touch = updateTimer.Value;

            if (method == "getObjectsStream")
            {
                Request.Headers.AcceptEncoding.Clear();
                HttpResponseMessage response = Request.CreateResponse();
                response.Headers.Add("Access-Control-Allow-Origin", "*");
                response.Content = new PushStreamContent(OnStreamAvailable, "text/event-stream");
                return response;
            }

            if (method == "getObjects")
            {
                var scene = new SceneUpdate();
                scene.FrameNo = RenderingHub.Instance.FrameNo;
                //scene.Objects = new List<ObjectInfo>();//RenderingEngine.Instance.GetObjects().ToList();
                scene.Objects = RenderingHub.Instance.GetObjects().ToList();
                return scene;
            }

            if (method == "start")
            {
                RenderingHub.Instance.Start();
                return "OK";
            }

            if (method == "stop")
            {
                RenderingHub.Instance.Stop();
                return "OK";
            }

            if (method == "reverseGravity")
            {
                RenderingHub.Instance.ReverseGravity();
                return "OK";
            }

            if (method == "addBall")
            {
                RenderingHub.Instance.AddRandomBall();
                return "OK";
            }
            if (method == "addTriangle")
            {
                for (int i = 0; i < 3; i++) RenderingHub.Instance.AddTriangle();
                return "OK";
            }
            if (method == "addRect")
            {
                for (int i = 0; i < 3; i++) RenderingHub.Instance.AddRect();
                return "OK";
            }
            if (method == "addStuff")
            {
                RenderingHub.Instance.CreateStuff();
                return "OK";
            }
            if (method == "addCross")
            {
                for (int i = 0; i < 3; i++) RenderingHub.Instance.CreateCross();
                return "OK";
            }

            if (method == "add10Ball")
            {
                for (int i = 0; i < 10; i++) RenderingHub.Instance.AddRandomBall();
                return "OK";
            }

            if (method == "bigWheel")
            {
                RenderingHub.Instance.BigWheel();
                return "OK";
            }

            if (method == "removeItem")
            {
                RenderingHub.Instance.RemoveLastItem();
                return "OK";
            }

            if (method.StartsWith("scene"))
            {
                var sceneName = method.Substring(5);
                RenderingHub.Instance.SceneStart(sceneName);
                return "OK";
            }
            if (method.StartsWith("event"))
            {
                var eventName = method.Substring(5);
                RenderingHub.Instance.SceneEvent(eventName);
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


        private static Task OnStreamAvailable(Stream stream, HttpContent headers, TransportContext context)
        {
            return Task.Run(() =>
            {
                StreamWriter writer = new StreamWriter(stream);
                clientSubscribers.TryAdd(writer, false);

            });
        }

        private static void UpdateClientsCallback(object state)
        {
            var scene = new SceneUpdate();
            scene.FrameNo = RenderingHub.Instance.FrameNo;
            scene.Objects = RenderingHub.Instance.GetObjects().ToList();
            scene.Info = RenderingHub.Instance.GetInfos().ToList();

            foreach (var pair in clientSubscribers.ToArray())
            {
                StreamWriter writer = pair.Key;

                try
                {
                    writer.Write(JsonConvert.SerializeObject(scene) + "\n\n");
                    //writer.Write(JsonConvert.SerializeObject(scene.FrameNo + "" + scene.FrameNo + "" + scene.FrameNo + "" + scene.FrameNo + "" + scene.FrameNo + "" + scene.FrameNo + "" + scene.FrameNo + "" + scene.FrameNo + "" + scene.FrameNo + "" + scene.FrameNo + "" + scene.FrameNo + "" + scene.FrameNo + "" + scene.FrameNo + "" + scene.FrameNo + "" + scene.FrameNo + "" + scene.FrameNo + "" + scene.FrameNo + "" + scene.FrameNo + "" + scene.FrameNo + "" + scene.FrameNo + "" + scene.FrameNo + "" + scene.FrameNo + "" + scene.FrameNo + "" + scene.FrameNo + "" + scene.FrameNo + "" + scene.FrameNo + "" + scene.FrameNo + "" + scene.FrameNo + "" + scene.FrameNo + "" + scene.FrameNo + "" + scene.FrameNo + "" + scene.FrameNo + "" + scene.FrameNo + "" + scene.FrameNo + "" + scene.FrameNo + "" + scene.FrameNo + "" + scene.FrameNo + "" + scene.FrameNo + "" + scene.FrameNo + "" + scene.FrameNo + "" + scene.FrameNo + "" + scene.FrameNo + "" + scene.FrameNo + "" + scene.FrameNo + "" + scene.FrameNo + "" + scene.FrameNo + "" + scene.FrameNo + "" + scene.FrameNo + "" + scene.FrameNo + "" + scene.FrameNo + "" + scene.FrameNo) + "\n\n");
                    writer.Flush();
                }
                catch (Exception)
                {
                    bool dummy;
                    if (clientSubscribers.TryRemove(writer, out dummy))
                    {
                        writer.Close();
                    }
                }
            }
        }

    }
}
