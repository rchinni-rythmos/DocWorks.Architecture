using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace DocWorks.BuildingBlocks.DataAccess.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class CollectionNameAttribute : Attribute
    {
        private string _name { get; set; }

        public virtual string Name
        {
            get { return _name; }
        }

        public CollectionNameAttribute(string name)
        {
            this._name = name;
        }

        public static string GetCollectionName<T>()
        {
            //var dnAttribute = typeof(T).GetCustomAttributes(
            //    typeof(CollectionNameAttribute), true
            //).FirstOrDefault() as CollectionNameAttribute;
            //if (dnAttribute != null)
            //{
            //    return dnAttribute.Name;
            //}

            // TODO: the above commented code is not compatible with .net core 2.0. so directly using the class name.
            return typeof(T).Name;
            //return null;
        }
    }
}
