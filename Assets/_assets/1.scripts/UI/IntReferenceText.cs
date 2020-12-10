using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;


namespace Arashmup
{
    public class IntReferenceText : MonoBehaviour
    {
        public IntReference Value;

        TMP_Text text;

        void Awake()
        {
            text = GetComponent<TMP_Text>();
        }

        void Update()
        {
            text.text = Value.Value.ToString();
        }

    }
}