using System;
using System.Reflection;

namespace CoreEDB.CoreModels
{
    public abstract class EntityBase
    {
        protected EntityBase()
        {
           
        }
        public EntityBase Clone()
        {
            return  (EntityBase)MemberwiseClone();
        }
        public virtual int ID { get; set; }
        public object this[string propertyName]
        {
            get
            {
                Type myType = GetType();
                PropertyInfo myPropInfo = myType.GetProperty(propertyName);
                return myPropInfo.GetValue(this, null);
            }
            set
            {
                Type myType = GetType();
                PropertyInfo myPropInfo = myType.GetProperty(propertyName);
                myPropInfo.SetValue(this, value, null);

            }
        }

    }
}
