using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Arashmup
{
    public class PlayerCharacter : MonoBehaviour
    {
        public GameObject FollowCamera;
        public BoolReference IsDead;
        public StringVariable PlayerName;
        public GameEvent NameChangeEvent;

        PhotonView PV;
        CharacterProxy proxy;

        CharacterMovement movement;
        CharacterFire fire;
        CharacterDamage damage;
        Rigidbody2D rigidBody;
        Collider2D collider2d;


        void Awake()
        {
            PV = GetComponent<PhotonView>();
            proxy = GetComponent<CharacterProxy>();
            movement = GetComponent<CharacterMovement>();
            fire = GetComponent<CharacterFire>();
            damage = GetComponent<CharacterDamage>();
            rigidBody = GetComponent<Rigidbody2D>();
            collider2d = GetComponent<Collider2D>();

            if (PV.IsMine)
            {
                CameraController cameraController = Instantiate(FollowCamera, Vector3.zero, Quaternion.identity).GetComponent<CameraController>();
                fire.FollowCamera = cameraController;
            }
            else
            {
                Destroy(movement);
                Destroy(fire);
                Destroy(damage);
                Destroy(rigidBody);
                Destroy(collider2d);

                PlayerName = ScriptableObject.CreateInstance<StringVariable>();
                GetComponentInChildren<TextReplacer>().Variable = PlayerName;

                IsDead.Variable = ScriptableObject.CreateInstance<BoolVariable>();
                proxy.IsDead = IsDead.Variable;
            }

            PlayerName.Value = PV.Owner.NickName;
            NameChangeEvent.Raise();
        }

        public void OnDeath()
        {

        }

        #region Runtime Set
        public PlayerCharacterRuntimeSet RuntimeSet;

        private void OnEnable()
        {
            RuntimeSet.Add(this);
        }

        private void OnDisable()
        {
            RuntimeSet.Remove(this);
        }
        #endregion
    }
}