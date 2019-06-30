using Core_v2.Repositories;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Core_v2.Globals
{
    public class MappingEntity<T,TN> where T:EntityBase where TN : EntityBase,new ()
    {
        public MappingEntity()
        {

        }

        public T Auto(T oldItem,T newItem)
        {
            Type myType = oldItem.GetType();
            IList<PropertyInfo> props = new List<PropertyInfo>(myType.GetProperties());
            var i = 0;
            foreach (PropertyInfo prop in props)
            {
                if (i >= props.Count - 1) break;
                object propValue = prop.GetValue(oldItem);
                if (propValue != null && newItem[prop.Name] == null)
                {
                    newItem[prop.Name] = propValue;
                }
                i++;
                // Do something with propValue
            }

            return newItem;
        }
        public TN AutoOrtherType(T oldItem, TN newItem)
        {
            Type myType = oldItem.GetType();
            IList<PropertyInfo> props = new List<PropertyInfo>(myType.GetProperties());
            var i = 0;
            foreach (PropertyInfo prop in props)
            {
                if (i >= props.Count - 1) break;
                object propValue = prop.GetValue(oldItem);
                if (propValue != null && newItem[prop.Name] == null)
                {
                    newItem[prop.Name] = propValue;
                }
                i++;
                // Do something with propValue
            }

            return newItem;
        }
    }
}
