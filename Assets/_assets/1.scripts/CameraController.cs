using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Realtime;
using TMPro;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.IO;

namespace Arashmup
{
    public class CameraController : MonoBehaviour
    {
        public FloatReference CameraDepth;
        public Vector3Reference PlayerPosition;

        Camera cam;

        void Start()
        {
            cam = GetComponent<Camera>();
        }

        public Vector2 GetWorldPoint()
        {
            return cam.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));

        }

        void FixedUpdate()
        {
            Vector3 newPosition = transform.position * 0.9f + PlayerPosition.Value * 0.1f;

            transform.position = new Vector3(newPosition.x, newPosition.y, CameraDepth);
        }
    }
}