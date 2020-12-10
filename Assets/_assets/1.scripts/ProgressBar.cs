using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Arashmup
{
    [ExecuteInEditMode()]
    public class ProgressBar : MonoBehaviour
    {
        public float maximum;
        public float current;
        public Image image;

        void Update()
        {
            image.fillAmount = current / maximum;
        }


    }
}