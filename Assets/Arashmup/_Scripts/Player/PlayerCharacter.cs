using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

using Hashtable = ExitGames.Client.Photon.Hashtable;


namespace Arashmup
{
    public class PlayerCharacter : MonoBehaviour
    {
        public GameObject FollowCamera;
        public BoolReference IsDead;
        public StringVariable PlayerName;
        public GameEvent NameChangeEvent;
        public GameEvent LocalPlayerDied;
        public GameEvent LocalPlayerWon;

        PhotonView PV;
        CharacterProxy proxy;

        CharacterMovement movement;
        CharacterFire fire;
        CharacterDamage damage;
        Rigidbody2D rigidBody;
        Collider2D collider2d;

        bool isAlreadyDead;

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
                Destroy(movement); movement = null;
                Destroy(fire); fire = null;
                Destroy(damage); damage = null;
                Destroy(rigidBody); rigidBody = null;
                Destroy(collider2d); collider2d = null;

                PlayerName = ScriptableObject.CreateInstance<StringVariable>();
                GetComponentInChildren<TextReplacer>().Variable = PlayerName;

                IsDead.Variable = ScriptableObject.CreateInstance<BoolVariable>();
                proxy.IsDead = IsDead.Variable;
            }

            PlayerName.Value = PV.Owner.NickName;
            NameChangeEvent.Raise();

            isAlreadyDead = false;
        }

        public void OnCharacterDeath()
        {
            if (!isAlreadyDead && IsDead.Value)
            {
                isAlreadyDead = true;

                if (PV.IsMine)
                {
                    LocalPlayerDied.Raise();
                }

                if (collider2d != null)
                {
                    Destroy(collider2d);
                } 
            }
        }

        public void OnGameOver()
        {
            if (!IsDead.Value && PV.IsMine)
            {
                AddWinToLocalPlayer();
                LocalPlayerWon.Raise();
            }
        }

        public Player GetNetworkPlayer()
        {
            return PV.Owner;
        }

        void AddWinToLocalPlayer()
        {
            int victoryCount = 0;
            if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey(CustomPropertiesKeys.VictoryCount))
            {
                victoryCount = (int)PhotonNetwork.LocalPlayer.CustomProperties[CustomPropertiesKeys.VictoryCount];
            }
            Hashtable hash = new Hashtable();
            hash.Add(CustomPropertiesKeys.VictoryCount, ++victoryCount);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
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