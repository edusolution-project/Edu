using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Core_v2.Globals
{
    public static class DbQueryExtends
    {
        public static IFindFluent<T, T> Find<T>(this IMongoCollection<T> collection,bool IsCheck, Expression<Func<T, bool>> filter)
        {
            if (IsCheck)
            {
                return collection.Find(filter);
            }
            else
            {
                return collection.Find(_=>true);
            }
        }
        public static IFindFluent<T, T> Find<T>(this IMongoCollection<T> collection, bool IsCheck, FilterDefinition<T> filter)
        {
            if (IsCheck)
            {
                return collection.Find(filter);
            }
            else
            {
                return collection.Find(_ => true);
            }
        }
        public static IFindFluent<T, T> Find<T>(this IFindFluent<T, T> source, bool IsCheck, FilterDefinition<T> filter)
        {
            if (IsCheck)
            {
                source.Filter = filter;
                return source;
            }
            else
            {
                return source;
            }
        }
    }
}
