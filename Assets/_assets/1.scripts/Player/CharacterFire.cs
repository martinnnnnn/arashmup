using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Realtime;
using Photon.Pun;
using System.IO;
using TMPro;
using DG.Tweening;

using Hashtable = ExitGames.Client.Photon.Hashtable;


namespace Arashmup
{
    public class CharacterFire : MonoBehaviour
    {
        public BoolReference IsDead;
        public BoolVariable FireAllowed;

        [Header("Firing")]
        public FloatVariable FireElaspedTime;
        public FloatReference FireRate;

        [HideInInspector] public CameraController FollowCamera;

        CharacterProxy proxy;

        void Start()
        {
            FireAllowed.SetValue(false);
            proxy = GetComponent<CharacterProxy>();
        }

        public void OnCountdownOver()
        {
            FireAllowed.SetValue(true);
        }

        void Update()
        {
            FireElaspedTime.ApplyChange(Time.deltaTime);

            if (Input.GetMouseButton(0) && FireAllowed.Value && !IsDead.Value && FireElaspedTime.Value > FireRate)
            {
                FireElaspedTime.SetValue(0.0f);

                Vector2 direction = (FollowCamera.GetWorldPoint() - new Vector2(transform.position.x, transform.position.y)).normalized;
                Vector3 position = transform.position + new Vector3(direction.x, direction.y, transform.position.z) * 0.9f;

                proxy.Fire(PhotonNetwork.LocalPlayer.ActorNumber, position, direction);
            }
        }
    }
}