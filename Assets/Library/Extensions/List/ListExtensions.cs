using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Library.Extensions.List
{
    public static class ListExtensions
    {
        public static bool AddIfNotExists<T>(this List<T> list, T item)
        {
            if (!list.Contains(item))
            {
                list.Add(item);
                return true;
            }
            return false;
        }

        public static void LogAll<T>(this List<T> list)
        {
            foreach (T item in list)
            {
                Debug.Log(item.ToString());
            }
        }

        public static bool HasType<T>(this List<T> list, Type type)
        {
            return list.Any(o => o.GetType() == type);
        }
    }
}