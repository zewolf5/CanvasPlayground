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
using System.Text;
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

        private static Dictionary<long, string> _queuedUpFrames = new Dictionary<long, string>();

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
                //return GetExtendedObjectInfo();
                HttpResponseMessage response = Request.CreateResponse();
                response.Content = new StringContent(GetExtendedObjectInfo());
                return response;
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

                lock (_queuedUpFrames)
                {
                    var now = (long)DateTime.Now.Subtract(startedTime).TotalMilliseconds;
                    if (!_queuedUpFrames.ContainsKey(scene.Ms))
                    {
                        _queuedUpFrames.Add(scene.Ms, JsonConvert.SerializeObject(scene) + "\n\n");
                    }
                    var old = _queuedUpFrames.Where(o => now - o.Key > 270).ToList();
                    foreach (var keyValuePair in old)
                    {
                        _queuedUpFrames.Remove(keyValuePair.Key);
                    }
                }

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
                            try
                            {
                                writer.Close();

                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine($"Error closing stream: {ex.Message}");
                            }
                        }
                    }
                }
            }
        }

        private static string GetExtendedObjectInfo()
        {
            var sb = new StringBuilder();
            lock (_queuedUpFrames)
            {
                foreach (var queuedUpFrame in _queuedUpFrames.OrderBy(o=>o.Key))
                {
                    sb.Append(queuedUpFrame.Value);
                }
            }
            return sb.ToString();
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
      

    }
}
