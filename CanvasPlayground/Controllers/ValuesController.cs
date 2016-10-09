using System;
using System.Collections;
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
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.SessionState;
using CanvasPlayground.Models;
using CanvasPlayground.Physics;
using Microsoft.Web.Administration;
using Microsoft.Web.Management.Server;
using Newtonsoft.Json;

namespace CanvasPlayground.Controllers
{
    //[Authorize]
    [SessionState(SessionStateBehavior.ReadOnly)]
    public class ValuesController : ApiController
    {

        private static Lazy<Timer> updateTimer = new Lazy<Timer>(() => new Timer(UpdateClientsCallback, null, 0, 5));
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
                return GetObjectInfo();
            }

            if (method == "start")
            {
                RenderingHub.Instance.Start();
                return "OK";
            }

            if (method == "stop")
            {
                updateTimer.Value.Dispose();
                updateTimer = new Lazy<Timer>(() => new Timer(UpdateClientsCallback, null, 0, 15));

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
            if (method == "RECYCLE")
            {
                updateTimer.Value.Dispose();
                updateTimer = new Lazy<Timer>(() => new Timer(UpdateClientsCallback, null, 0, 15));
                HttpRuntime.UnloadAppDomain();
                return "RECYCLED!";
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

        private static long _lastFrame = 0;
        private static Stopwatch timStopwatch = Stopwatch.StartNew();
        private static long timCount = 0;

        private static DateTime startedTime;

        //private static Stopwatch timStopwatchTotal;

        private static void UpdateClientsCallback(object state)
        {
            if (_lastFrame < RenderingHub.Instance.FrameNo)
            {
                if (startedTime == null) startedTime = DateTime.Now; ;
                if (timStopwatch.ElapsedMilliseconds < 1000)
                {
                    timCount++;
                }
                else
                {
                    Debug.WriteLine("FPS sent: " + timCount);
                    timCount = 0;
                    timStopwatch.Restart();
                }
                var scene = GetObjectInfo();
                _lastFrame = scene.FrameNo;
                //Debug.WriteLine("Frame sent: " + _lastFrame);

                foreach (var pair in clientSubscribers.ToArray())
                {
                    StreamWriter writer = pair.Key;

                    try
                    {
                        writer.Write(JsonConvert.SerializeObject(scene) + "\n\n");
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

        private static SceneUpdate GetObjectInfo()
        {
            var scene = new SceneUpdate();
            scene.Ms = (long)RenderingHub.Instance.FrameRenderTime.Subtract(startedTime).TotalMilliseconds;
            scene.FrameNo = RenderingHub.Instance.FrameNo;
            scene.Objects = RenderingHub.Instance.GetObjects().ToList();
            scene.Info = RenderingHub.Instance.GetInfos().ToList();
            return scene;
        }
        //// Gets the application pool collection from the server.
        //[ModuleServiceMethod(PassThrough = true)]
        //public ArrayList GetApplicationPoolCollection()
        //{
        //    // Use an ArrayList to transfer objects to the client.
        //    ArrayList arrayOfApplicationBags = new ArrayList();

        //    ServerManager serverManager = new ServerManager();
        //    ApplicationPoolCollection applicationPoolCollection = serverManager.ApplicationPools;
        //    foreach (ApplicationPool applicationPool in applicationPoolCollection)
        //    {
        //        PropertyBag applicationPoolBag = new PropertyBag();
        //        //applicationPoolBag[ServerManagerDemoGlobals.ApplicationPoolArray] = applicationPool;
        //        arrayOfApplicationBags.Add(applicationPoolBag);
        //        // If the applicationPool is stopped, restart it.
        //        if (applicationPool.State == ObjectState.Stopped)
        //        {
        //            applicationPool.Start();
        //        }

        //    }

        //    // CommitChanges to persist the changes to the ApplicationHost.config.
        //    serverManager.CommitChanges();
        //    return arrayOfApplicationBags;
        //}

    }
}
