using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    public class Utils
    {
        public static void ShuffleList<T>(List<T> list)
        {
            for (var i = list.Count - 1; i > 0; i--)
            {
                var randomIndex = Random.Range(0, i + 1); // Get a random index
                (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
            }
        }
    }
}