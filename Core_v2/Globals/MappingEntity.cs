using Core_v2.Repositories;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Core_v2.Globals
{
    public class MappingEntity<T,TN> where T:EntityBase where TN : EntityBase,new ()
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
                    if (Verify(newItem[item.Name]) && value != null)
                    {
                        newItem[item.Name] = value;
                    }
                }
            }
            return newItem;
        }
        public T Auto(T oldItem,T newItem)
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
                    if (Verify(newItem[item.Name] == null) && value != null)
                    {
                        newItem[item.Name] = value;
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
                    if (Verify(newItem[item.Name]) && value != null)
                    {
                        newItem[item.Name] = value;
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

            for(int i = 0; oldProps != null && i < oldProps.Count - 1; i++)
            {
                var item = newProps[i];
                if (item.Name == "ID" || item.Name == "id" || item.Name == "_id") continue;
                if (newProps.Contains(item)) {
                    var value = item.GetValue(oldItem);
                    if (Verify(newItem[item.Name]) && value != null)
                    {
                        newItem[item.Name] = value;
                    }
                }
            }
            return newItem;
        }

        private bool Verify(object value)
        {
            try
            {
                if (value == null) return true;
                if (IsBoolean(value))
                {
                    return !(bool)value;
                }
                if (IsDateTime(value))
                {
                    return (DateTime) value <= DateTime.MinValue;
                }
                if (IsNumber(value)) return (int)value == 0;

                return false;

            }
            catch (Exception)
            {
                return true;
            }
        }
        private bool IsBoolean(object value)
        {
            try
            {
                bool x;
                return bool.TryParse(value.ToString(), out x);
            }
            catch (Exception)
            {
                return false;
            }
        }
        private bool IsDateTime(object value)
        {
            try
            {
                DateTime x;
                return DateTime.TryParse(value.ToString(), out x);
            }
            catch (Exception)
            {
                return false;
            }
        }
        private bool IsNumber(object value)
        {
            try
            {
                string pText = value.ToString();
                Regex regex = new Regex(@"^[-+]?[0-9]*\.?[0-9]+$");
                return regex.IsMatch(pText);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
