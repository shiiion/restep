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
            partitioner = new Collision.ObjectPartitioner();
            lock (CoreLock)
            {
                while (Running)
                {
                    float time = (sw.ElapsedMilliseconds - lastTime);
                    lastTime = sw.ElapsedMilliseconds;
                    Input.InputManager.UpdateStates();
                    Tick?.Invoke(time / 1000.0f, ObjectList);
                    partitioner.ForEachPossibleCollision(OnPossibleCollision);
                    Monitor.Wait(CoreLock);
                }
            }
        }

        public void OnPossibleCollision(GameObject a, GameObject b)
        {
            bool overlapping = false;
            if(a.ObjectCollider.Type == Collision.ColliderType.CT_AABB &&
                b.ObjectCollider.Type == Collision.ColliderType.CT_AABB)
            {
                overlapping = true;
            }
            else
            {
                overlapping = a.TestCollision(b);
            }

            if(overlapping)
            {
                a.OnOverlap(b);
                b.OnOverlap(a);
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
                partitioner.AddNewObject(obj);
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
