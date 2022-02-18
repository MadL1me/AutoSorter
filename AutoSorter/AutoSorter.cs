using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using AutoSorter.Attributes;

namespace AutoSorter
{
    public static class AutoSorter
    {
        private static Dictionary<Type, Dictionary<int, Delegate>> _cache =
            new Dictionary<Type, Dictionary<int, Delegate>>();

        public static IOrderedEnumerable<TObject> Sort<TObject>(IEnumerable<TObject> source, int sortOrderId,
            bool isDescending)
        {
            if (!source.Any())
                throw new ArgumentException(nameof(source));

            var func = ReadFromCache<TObject>(sortOrderId);
            
            if (func == null)
                func = GetFunc<TObject>(sortOrderId);
            
            return isDescending ? source.OrderByDescending(func) : source.OrderBy(func);
        }

        private static List<(TAttribute, PropertyInfo)?> GetAttrList<TAttribute>(Type type) where TAttribute : Attribute
        {
            var props = type.GetProperties().Where(prop => prop.IsDefined(typeof(TAttribute), false));
            var attrToProp = new List<(TAttribute, PropertyInfo)?>();
            foreach (var prop in props)
            {
                attrToProp.Add(((TAttribute) prop.GetCustomAttributes(typeof(TAttribute), false).First(), prop));
            }
            return attrToProp;
        }

        private static void AddToCache(Type type, int orderId, Delegate @delegate)
        {
            if (!_cache.ContainsKey(type))
                _cache.Add(type, new Dictionary<int, Delegate>());

            _cache[type][orderId] = @delegate;
        }

        private static Func<TObject, object> ReadFromCache<TObject>(int orderId)
        {
            var type = typeof(TObject);
            
            if (!_cache.ContainsKey(type))
                return null;

            if (!_cache[type].ContainsKey(orderId))
                return null;

            return t => _cache[type][orderId].DynamicInvoke(t);
        }
        
        private static Func<TObject, object> GetFunc<TObject>(int sortOrderId)
        {
            var autoSortList = GetAttrList<AutoSortAttribute>(typeof(TObject));
            var autoSortArrayList = GetAttrList<AutoSortArrayAttribute>(typeof(TObject));
            
            var singleAttr = autoSortList.SingleOrDefault(a => a.Value.Item1.OrderId == sortOrderId);
            if (singleAttr != null)
            {
                var param = Expression.Parameter(typeof(TObject), "t");
                var property = Expression.Property(param, singleAttr.Value.Item2.Name);
                var lambda = Expression.Lambda(property, param);
                var @delegate = lambda.Compile();
                AddToCache(typeof(TObject), sortOrderId, @delegate);
                
                return t => @delegate.DynamicInvoke(t);
            }
            
            var arrayAttr = autoSortArrayList.SingleOrDefault(a => a.Value.Item1.OrderIdToIndex.ContainsKey(sortOrderId));
            if (arrayAttr != null)
            {
                var param = Expression.Parameter(typeof(TObject), "t");
                var property = Expression.Property(param, arrayAttr.Value.Item2.Name);
                var access = Expression.ArrayIndex(property, Expression.Constant(arrayAttr.Value.Item1.OrderIdToIndex[sortOrderId]));
                
                var lambda = Expression.Lambda(access, param);
                var @delegate = lambda.Compile();
                AddToCache(typeof(TObject), sortOrderId, @delegate);
                
                return t => @delegate.DynamicInvoke(t);
            }

            throw new ArgumentException();
        }
    }
}