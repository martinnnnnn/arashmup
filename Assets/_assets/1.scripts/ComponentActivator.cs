using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Realtime;
using TMPro;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.IO;
using System;

namespace Arashmup
{
    public class ComponentActivator : MonoBehaviour
    {
        public MonoBehaviour component;
        public bool defaultValue;
        public float automaticDeactivationTime;

        private void OnEnable()
        {
            component.enabled = defaultValue;
        }

        public void Activate(bool value)
        {
            component.enabled = value;
            if (automaticDeactivationTime > 0)
            {
                StartCoroutine(Deactivation());
            }
        }

        IEnumerator Deactivation()
        {
            yield return new WaitForSeconds(automaticDeactivationTime);
            component.enabled = false;
        }
    }
}