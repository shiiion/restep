using System;
using System.Collections.Generic;
using System.Text;

//TODO: move me to helper namespace

namespace restep.Framework.ResourceManagement
{
    internal delegate void HashChangedDelegate(ResourceHash oldHash, ResourceHash newHash);

    /// <summary>
    /// Abstract definition for any resource that will have references to it managed by ReferenceCounter
    /// </summary>
    internal abstract class CountableResource
    {
        public int RefCount { get; set; }

        public event HashChangedDelegate IdentifierHashChanged;

        private ResourceHash identifierHash;
        public ResourceHash IdentifierHash
        {
            get
            {
                return identifierHash;
            }

            set
            {
                IdentifierHashChanged?.Invoke(IdentifierHash, value);
                identifierHash = value;
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
        private static Dictionary<ResourceHash, CountableResource> resources 
            = new Dictionary<ResourceHash, CountableResource>(new ResourceHash.RHComparer());

        public static bool ReferenceExists(ResourceHash identifier)
        {
            return resources.ContainsKey(identifier);
        }

        public static ResType GetReference<ResType>(ResourceHash identifier) where ResType : CountableResource
        {
            CountableResource ret;
            if(resources.TryGetValue(identifier, out ret))
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

        private static CountableResource removeReference(ResourceHash hashCode)
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
                removeReference(reference);
                reference.OnDestroy();
            }
        }
    }
}
