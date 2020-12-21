using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;


namespace Arashmup
{
    public class IntReferenceText : MonoBehaviour
    {
        public GenericReference<int> Value;
        public bool AlwaysUpdate;

        TMP_Text text;

        void Awake()
        {
            text = GetComponent<TMP_Text>();
        }

        void Update()
        {
            if (AlwaysUpdate)
            {
                UpdateOnce();
            }
        }

        public void UpdateOnce()
        {
            text.text = Value.Value.ToString();
        }

    }
}