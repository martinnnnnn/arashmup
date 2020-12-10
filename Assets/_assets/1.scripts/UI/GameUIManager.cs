using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Realtime;
using TMPro;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.IO;
using DG.Tweening;
using System;
using UnityEngine.UI;


namespace Arashmup
{
    public class GameUIManager : MonoBehaviour
    {
        public TMP_Text aliveCount;
        public TMP_Text ammoLeft;

        private void Awake()
        {
            aliveCount.gameObject.SetActive(false);
        }
    }
}