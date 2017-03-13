using System;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;

namespace restep.Core
{
    public delegate void TickDelegate(float dt, List<GameObject> objectList);

    public class CoreThread
    {
        public static CoreThread Instance;

        public event TickDelegate Tick;

        private Thread thread;
        public bool Running { get; set; } = false;
        public readonly object CoreLock = new object();

        private Collision.ObjectPartitioner partitioner;

        public List<GameObject> ObjectList { get; set; } = new List<GameObject>();

        public static void Initialize()
        {
            Instance = new CoreThread();
            Instance.Start();
        }

        public CoreThread() {}

        private void coreFunction()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            float lastTime = sw.ElapsedMilliseconds;
            lock (CoreLock)
            {
                while (Running)
                {
                    float time = (sw.ElapsedMilliseconds - lastTime);
                    lastTime = sw.ElapsedMilliseconds;
                    Input.InputManager.UpdateStates();
                    Tick?.Invoke(time / 1000.0f, ObjectList);
                    Monitor.Wait(CoreLock);
                }
            }
        }

        public void Start()
        {
            Running = true;
            thread = new Thread(coreFunction);
            thread.Start();
        }

        public void Pulse()
        {
            lock (CoreLock)
            {
                Monitor.Pulse(CoreLock);
            }
        }

        public void AddObject(GameObject obj)
        {
            lock(CoreLock)
            {
                ObjectList.Add(obj);
            }
        }

        public GameObject GetObject(ulong id)
        {
            foreach (GameObject obj in ObjectList)
            {
                if(obj.ObjectID == id)
                {
                    return obj;
                }
            }

            return null;
        }
    }
}
