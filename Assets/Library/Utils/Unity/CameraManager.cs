using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[ExecuteInEditMode]
public class CameraManager : MonoBehaviour
{
    public float targetAspect = 0.5625f;

    void OnGUI()
    {
        float currentAspect = (float)Screen.width / (float)Screen.height;
        Camera.main.orthographicSize = 11 * (targetAspect / currentAspect);
    }
}
