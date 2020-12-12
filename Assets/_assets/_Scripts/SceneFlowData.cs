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
    [CreateAssetMenu ]
    public class SceneFlowData : ScriptableObject
    {
        public bool backFromGameplay = false;
        public bool stayInRoom = false;

        void OnEnable()
        {
            Reset();
        }

        public void Reset()
        {
            backFromGameplay = false;
            stayInRoom = false;
        }
    }
}