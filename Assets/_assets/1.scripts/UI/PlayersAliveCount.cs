using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;


namespace Arashmup
{
    public class PlayersAliveCount : MonoBehaviour
    {
        public IntReference Count;

        TMP_Text text;

        void Awake()
        {
            text = GetComponent<TMP_Text>();
        }

        void Update()
        {
            text.text = Count.Value.ToString();
        }

    }
}