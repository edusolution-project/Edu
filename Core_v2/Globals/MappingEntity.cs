using Core_v2.Repositories;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Core_v2.Globals
{
    public class MappingEntity<T, TN> where T : EntityBase where TN : EntityBase, new()
    {
        public MappingEntity()
        {

        }
        public T AutoWithoutID(T oldItem, T newItem)
        {
            if (oldItem == null) return newItem;
            //lay typeOldItem
            Type oldType = oldItem.GetType();
            IList<PropertyInfo> oldProps = new List<PropertyInfo>(oldType.GetProperties());
            //lay typenewItem
            Type newType = oldItem.GetType();
            IList<PropertyInfo> newProps = new List<PropertyInfo>(newType.GetProperties());

            for (int i = 0; oldProps != null && i < oldProps.Count - 1; i++)
            {
                var item = newProps[i];
                if (item.Name == "ID" || item.Name == "id" || item.Name == "_id") continue;
                if (newProps.Contains(item))
                {
                    var value = item.GetValue(oldItem);
                    var type = item.GetType();
                    if ((newItem[item.Name] == null) && value != null)
                    {
                        newItem[item.Name] = value;
                    }
                }
            }
            return newItem;
        }
        public T Auto(T oldItem, T newItem)
        {
            if (oldItem == null) return newItem;
            //lay typeOldItem
            Type oldType = oldItem.GetType();
            IList<PropertyInfo> oldProps = new List<PropertyInfo>(oldType.GetProperties());
            //lay typenewItem
            Type newType = oldItem.GetType();
            IList<PropertyInfo> newProps = new List<PropertyInfo>(newType.GetProperties());

            for (int i = 0; oldProps != null && i < oldProps.Count - 1; i++)
            {
                var item = newProps[i];
                if (newProps.Contains(item))
                {
                    var value = item.GetValue(oldItem);
                    if ((newItem[item.Name] == null || item.Name == "IsActive") && value != null)//stupid maping
                    {
                        switch (item.PropertyType.Name)
                        {
                            case "Boolean":
                                if (!(bool)newItem[item.Name])
                                    newItem[item.Name] = value;
                                break;
                            case "DateTime":
                                if ((DateTime)newItem[item.Name] <= DateTime.MinValue && (DateTime)value > DateTime.MinValue)
                                    newItem[item.Name] = value;
                                break;
                            default:
                                newItem[item.Name] = value;
                                break;
                        }
                    }
                }
            }
            return newItem;
        }

        public TN AutoOrtherType(T oldItem, TN newItem)
        {
            if (oldItem == null) return newItem;
            //lay typeOldItem
            Type oldType = oldItem.GetType();
            IList<PropertyInfo> oldProps = new List<PropertyInfo>(oldType.GetProperties());
            //lay typenewItem
            Type newType = oldItem.GetType();
            IList<PropertyInfo> newProps = new List<PropertyInfo>(newType.GetProperties());

            for (int i = 0; oldProps != null && i < oldProps.Count - 1; i++)
            {
                var item = newProps[i];
                var type = item.GetType();
                if (newProps.Contains(item))
                {
                    var value = item.GetValue(oldItem);
                    if(newItem[item.Name] == null) newItem[item.Name] = value;
                    else
                    {
                        switch (item.PropertyType.Name)
                        {
                            case "Boolean":
                                if (!(bool) newItem[item.Name])
                                    newItem[item.Name] = value;
                                break;
                            case "DateTime":
                                if ((DateTime) newItem[item.Name] <= DateTime.MinValue)
                                    newItem[item.Name] = value;
                                break;
                            default:
                                newItem[item.Name] = value;
                                break;
                        }
                    }
                }
            }
            return newItem;
        }
        public TN AutoOrtherTypeWithoutID(T oldItem, TN newItem)
        {
            if (oldItem == null) return newItem;
            //lay typeOldItem
            Type oldType = oldItem.GetType();
            IList<PropertyInfo> oldProps = new List<PropertyInfo>(oldType.GetProperties());
            //lay typenewItem
            Type newType = oldItem.GetType();
            IList<PropertyInfo> newProps = new List<PropertyInfo>(newType.GetProperties());

            for (int i = 0; oldProps != null && i < oldProps.Count - 1; i++)
            {
                var item = newProps[i];
                if (item.Name == "ID" || item.Name == "id" || item.Name == "_id") continue;
                if (newProps.Contains(item))
                {
                    var value = item.GetValue(oldItem);
                    if ((newItem[item.Name] == null || item.Name == "IsActive") && value != null)
                    {
                        newItem[item.Name] = value;
                    }
                }
            }
            return newItem;
        }
    }
}
