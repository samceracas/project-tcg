using System;
using System.Collections.Generic;
using System.Linq; 
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Library.Utils.Unity
{
    public static class Vector3Utils
    {
        public static Vector3 GetQuadraticCoordinates(Vector3 p0, Vector3 p1, Vector3 c0, float t)
        {
            return Mathf.Pow(1 - t, 2) * p0 + 2 * t * (1 - t) * c0 + Mathf.Pow(t, 2) * p1;
        }

        public static Vector3 GetControlPointBetweenTwoPoints(Vector3 p0, Vector3 p1, float height = 5.0f)
        {
            return p0 + (p1 - p0) / 2 + Vector3.up * height;
        }
    }
}
