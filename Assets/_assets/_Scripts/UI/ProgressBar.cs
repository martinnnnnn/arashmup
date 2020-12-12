﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Arashmup
{
    [ExecuteInEditMode()]
    public class ProgressBar : MonoBehaviour
    {
        public FloatReference Variable;
        public FloatReference Min;
        public FloatReference Max;

        public Image Image;

        private void Update()
        {
            Image.fillAmount = Mathf.Clamp01(
                Mathf.InverseLerp(Min, Max, Variable));
        }

    }
}