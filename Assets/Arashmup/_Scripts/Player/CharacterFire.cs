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
        public GenericReference<bool> IsDead;
        public BoolVariable FireAllowed;
        public GameInputs Inputs;

        [Header("Firing")]
        public FloatVariable FireElaspedTime;
        public GenericReference<float> FireRate;

        public Transform gunPos1;
        public Transform gunPos2;

        [HideInInspector] public CameraController FollowCamera;

        CharacterProxy proxy;
        [HideInInspector] public Animator weaponAnimator;

        void Start()
        {
            FireAllowed.SetValue(false);
            proxy = GetComponent<CharacterProxy>();
            foreach(Animator anim in GetComponentsInChildren<Animator>())
            {
                if (anim.name == "Weapon")
                {
                    weaponAnimator = anim;
                    break;
                }
            }
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
            bool hasTriggered = Inputs.Actions.Gameplay.Fire.ReadValue<float>() > 0.1f;

            if (hasTriggered || direction.magnitude > 0.1f)
            {
                // in case of click, we need to compute the direction using the mouse position
                if (direction.magnitude <= 0.1f)
                {
                    direction = (FollowCamera.GetWorldPoint() - new Vector2(transform.position.x, transform.position.y));
                }

                direction.Normalize();

                if (FireAllowed.Value && !IsDead.Value && FireElaspedTime.Value > FireRate.Value)
                {
                    FireElaspedTime.SetValue(0.0f);

                    // adding a bit of the direction vector to the position so the bullet does spawn at the character center
                    Vector3 position = transform.position + new Vector3(direction.x, direction.y, transform.position.z) * 0.9f; 

                    proxy.Fire(PhotonNetwork.LocalPlayer.ActorNumber, position, direction);
                    weaponAnimator.SetTrigger("Fire");
                }
            }

            Vector3 cameraWorldPosition = new Vector3(FollowCamera.GetWorldPoint().x, FollowCamera.GetWorldPoint().y, weaponAnimator.transform.position.z);

            if (cameraWorldPosition.x < weaponAnimator.transform.position.x)
            {
                weaponAnimator.GetComponent<SpriteRenderer>().flipX = true;
                weaponAnimator.transform.position = gunPos2.position;
                weaponAnimator.transform.right = weaponAnimator.transform.position - cameraWorldPosition;
            }
            else
            {
                weaponAnimator.GetComponent<SpriteRenderer>().flipX = false;
                weaponAnimator.transform.position = gunPos1.position;
                weaponAnimator.transform.right = cameraWorldPosition - weaponAnimator.transform.position;
            }
        }
    }
}