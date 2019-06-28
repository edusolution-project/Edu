﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace Core_v2.Repositories
{
    public abstract class EntityBase
    {
        [BsonId]
        [DataMember]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonProperty("ID")]
        public virtual string ID { get; set; }
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
