using System;
using System.Collections.Generic;
using System.Text;

//TODO: move me to helper namespace

namespace restep.Framework.ResourceManagement
{

    internal abstract class CountableResource
    {
        public int RefCount { get; set; }

        private int identifierHash;
        public int IdentifierHash
        {
            get
            {
                return identifierHash;
            }

            set
            {
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
        private static List<CountableResource> resources;

        public static bool ReferenceExists(object identifier)
        {
            int OHC = identifier.GetHashCode();
            foreach (CountableResource resource in resources)
            {
                if (resource.IdentifierHash == OHC) return true;
            }
            return false;
        }

        public static ResType GetReference<ResType>(object identifier) where ResType : CountableResource
        {
            int OHC = identifier.GetHashCode();
            foreach (CountableResource resource in resources)
            {
                if (resource.IdentifierHash == OHC)
                {
                    return (ResType)resource;
                }
            }
            return null;
        }

        public static void AddNewReference(CountableResource reference)
        {
            if(reference.Hashed)
            {
                resources.Add(reference);
            }
        }
    }
}
