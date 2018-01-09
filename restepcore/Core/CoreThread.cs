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

        public Collision.DualColliderPartitioner partitioner;

        public List<GameObject> ObjectList { get; set; } = new List<GameObject>();
        private Stopwatch sw;

        public static void Initialize()
        {
            Instance = new CoreThread();
            Instance.Start();
        }

        public CoreThread()
        {
            Tick += (dt, objects) =>
            {
                for(int a=0;a<objects.Count;a++)
                {
                    if(objects[a].Destroy)
                    {
                        partitioner.RemoveObject(objects[a]);
                        objects[a].DisposeObject();
                        objects.RemoveAt(a);
                        a--;
                        continue;
                    }
                    objects[a].Position += objects[a].Velocity * dt;
                    objects[a].TickObject(dt);
                }
            };
        }

        public float GetEngineTime()
        {
            if(sw != null)
            {
                return sw.ElapsedMilliseconds / 1000.0f;
            }
            return 0;
        }

        private void coreFunction()
        {
            sw = new Stopwatch();
            sw.Start();
            float lastTime = sw.ElapsedMilliseconds;
            partitioner = new Collision.DualColliderPartitioner(null, null);
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

        public bool OnPossibleCollision(GameObject a, GameObject b)
        {
            return a.TestCollision(b);
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
                obj.SpawnTime = GetEngineTime();
                ObjectList.Add(obj);
                partitioner.AddNewObject(obj);
            }
        }

        public void RemoveObject(GameObject obj)
        {
            lock(CoreLock)
            {
                ObjectList.Remove(obj);
            }
        }

        public void RemoveObject(ulong id)
        {
            lock(CoreLock)
            {
                for (int a = 0; a < ObjectList.Count; a++)
                {
                    if (id == ObjectList[a].ObjectID)
                    {
                        ObjectList.RemoveAt(a);
                        break;
                    }
                }
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
