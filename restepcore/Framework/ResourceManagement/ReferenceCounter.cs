using System;
using System.Collections.Generic;
using System.Text;

//TODO: move me to helper namespace

namespace restep.Framework.ResourceManagement
{
    internal delegate void HashChangedDelegate(int oldHash, int newHash);

    /// <summary>
    /// Abstract definition for any resource that will have references to it managed by ReferenceCounter
    /// </summary>
    internal abstract class CountableResource
    {
        public int RefCount { get; set; }

        public event HashChangedDelegate IdentifierHashChanged;

        public int IdentifierHash
        {
            get;
            private set;
        }

        public string IdentifierHashString
        {
            set
            {
                int hashCode = value.GetHashCode();
                IdentifierHashChanged?.Invoke(IdentifierHash, hashCode);
                IdentifierHash = hashCode;
                Hashed = true;
            }
        }
        public bool Loaded { get; protected set; }
        public bool Hashed { get; private set; }

        public CountableResource()
        {
            RefCount = 1;
            Loaded = false;
        }

        public abstract void OnDestroy();
    }

    /// <summary>
    /// Reference counter for loaded resources
    /// optimizes for texture and mesh re-use
    /// </summary>
    internal class ReferenceCounter
    {
        private static Dictionary<int, CountableResource> resources = new Dictionary<int, CountableResource>();

        public static bool ReferenceExists(string identifier)
        {
            int IHC = identifier.GetHashCode();
            return resources.ContainsKey(IHC);
        }

        public static ResType GetReference<ResType>(string identifier) where ResType : CountableResource
        {
            int IHC = identifier.GetHashCode();
            CountableResource ret;
            if(resources.TryGetValue(IHC, out ret))
            {
                return (ResType)ret;
            }
            return null;
        }

        public static void AddNewReference(CountableResource reference)
        {
            if(reference.Hashed)
            {
                resources.Add(reference.IdentifierHash, reference);
                reference.IdentifierHashChanged += (oh, nh) =>
                {
                    CountableResource remap = removeReference(oh);
                    if(remap != null)
                    {
                        resources.Add(nh, remap);
                    }
                };
            }
        }

        private static void removeReference(CountableResource reference)
        {
            resources.Remove(reference.IdentifierHash);
        }

        private static CountableResource removeReference(int hashCode)
        {
            CountableResource ret = null;

            if (resources.TryGetValue(hashCode, out ret))
            {
                resources.Remove(hashCode);
            }

            return ret;
        }

        public static void ReleaseReference(CountableResource reference)
        {
            if(reference.Loaded && reference.Hashed && reference.RefCount > 0)
            {
                reference.RefCount--;
            }

            if(reference.RefCount == 0)
            {
                reference.OnDestroy();
                removeReference(reference);
            }
        }
    }
}
