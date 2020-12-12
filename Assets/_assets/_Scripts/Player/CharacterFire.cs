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
        public GameInputs Inputs;

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

        PhotonView PV;
        void Update()
        {
            FireElaspedTime.ApplyChange(Time.deltaTime);

            if (PV == null)
            {
                PV = GetComponent<PhotonView>();
            }

            Vector2 direction = Inputs.Actions.Gameplay.FireDirection.ReadValue<Vector2>();
            bool hasClicked = Inputs.Actions.Gameplay.Fire.ReadValue<float>() > 0.1f;

            if (hasClicked || direction.magnitude > 0.1f)
            {
                if (FireAllowed.Value && !IsDead.Value && FireElaspedTime.Value > FireRate.Value)
                {
                    FireElaspedTime.SetValue(0.0f);

                    // in case of click, we need to compute the direction using the mouse position
                    if (direction.magnitude <= 0.1f)
                    {
                        direction = (FollowCamera.GetWorldPoint() - new Vector2(transform.position.x, transform.position.y));
                    }

                    direction.Normalize();

                    // adding a bit of the direction vector to the position so the bullet does spawn at the character center
                    Vector3 position = transform.position + new Vector3(direction.x, direction.y, transform.position.z) * 0.9f; 

                    proxy.Fire(PhotonNetwork.LocalPlayer.ActorNumber, position, direction);
                }
            }
        }
    }
}