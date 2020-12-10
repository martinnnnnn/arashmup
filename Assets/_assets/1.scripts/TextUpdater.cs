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
    public class TextUpdater : MonoBehaviour
    {
        TMP_InputField inputField;


        public StringVariable Variable;


        private void OnEnable()
        {
            inputField = GetComponent<TMP_InputField>();
            inputField.onDeselect.AddListener(UpdateVariable);
        }

        private void OnDisable()
        {
            inputField.onDeselect.RemoveListener(UpdateVariable);
        }

        void UpdateVariable(string value)
        {
            Variable.Value = value;
        }

    }
}