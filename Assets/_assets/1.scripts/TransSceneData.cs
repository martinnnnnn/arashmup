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
    public class TransSceneData : MonoBehaviour
    {
        public static TransSceneData Instance;

        public bool backFromGameplay;
        public bool stayInRoom;

        //[PunRPC]
        //bool BackFromGameplay
        //{
        //    get { return backFromGameplay; }
        //    set 
        //    {
        //        backFromGameplay = value;
        //        var hash = new ExitGames.Client.Photon.Hashtable();
        //        hash.Add("backFromGameplay", backFromGameplay);
        //        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        //    }
        //}


        private void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
            Instance = this;

            ResetState();
        }

        public void ResetState()
        {
            backFromGameplay = false;
            stayInRoom = false;
        }
    }
}