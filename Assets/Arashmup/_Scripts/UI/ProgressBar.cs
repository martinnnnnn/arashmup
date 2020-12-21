using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Arashmup
{
    [ExecuteInEditMode()]
    public class ProgressBar : MonoBehaviour
    {
        public GenericReference<float> Variable;
        public GenericReference<float> Min;
        public GenericReference<float> Max;

        public Image Image;

        private void Update()
        {
            Image.fillAmount = Mathf.Clamp01(
                Mathf.InverseLerp(Min, Max, Variable));
        }

    }
}