using System;
using System.Collections.Generic;

namespace AutoSorter.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
    public class AutoSortArrayAttribute : Attribute
    {
        public Dictionary<int, int> OrderIdToIndex { get; private set; } = new Dictionary<int, int>();

        public AutoSortArrayAttribute(int[] orderIds, int[] arrayIndexes)
        {
            if (orderIds.Length != arrayIndexes.Length)
                throw new ArgumentException("orderIds and arrayIndexes arrays must have same size");

            for (int i = 0; i < orderIds.Length; i++)
            {
                OrderIdToIndex.Add(orderIds[i], arrayIndexes[i]);
            }
        }
    }
    
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Struct)]
    public class AutoSortAttribute : Attribute
    {
        public int OrderId { get; private set; }
        
        public AutoSortAttribute(int orderId)
        {
            OrderId = orderId;
        }
    }

}