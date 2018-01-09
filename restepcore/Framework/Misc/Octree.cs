using System;
using System.Collections.Generic;
using System.Text;


//TODO: this
namespace restep.Framework.Misc
{
    internal class Octree<T> where T : IBoundingBox
    {
        private List<T> objects;


        protected Octree<T>[] leaves;

        public List<T> QueryTree(T source)
        {
            return null;
        }

        public void Insert(T obj)
        {

        }
    }
}
