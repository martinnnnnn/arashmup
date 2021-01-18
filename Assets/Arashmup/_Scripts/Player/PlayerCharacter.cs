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
        public GenericReference<bool> IsDead;
        public StringVariable PlayerName;
        public GameEvent NameChangeEvent;
        public GameEvent LocalPlayerDied;
        public GameEvent LocalPlayerWon;

        PhotonView PV;
        CharacterProxy proxy;

        CharacterMovement movement;
        CharacterFire fire;
        CharacterDamage damage;
        BoosterController boosterController;
        WeaponController weaponController;
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
            boosterController = GetComponent<BoosterController>();
            weaponController = GetComponent<WeaponController>();
            rigidBody = GetComponent<Rigidbody2D>();
            collider2d = GetComponent<Collider2D>();

            if (PV.IsMine)
            {
                InitLocal();
            }
            else
            {
                InitProxy();
            }
            
            PlayerName.Value = PV.Owner.NickName;
            NameChangeEvent.Raise();

            isAlreadyDead = false;
        }

        void InitProxy()
        {
            Destroy(movement); movement = null;
            Destroy(fire); fire = null;
            Destroy(damage); damage = null;
            Destroy(boosterController); boosterController = null;
            Destroy(rigidBody); rigidBody = null;
            Destroy(collider2d); collider2d = null;

            // Replace PlayerName
            PlayerName = ScriptableObject.CreateInstance<StringVariable>();
            GetComponentInChildren<TextReplacer>().Variable = PlayerName;

            // Replace IsDead
            proxy.IsDead = ScriptableObject.CreateInstance<BoolVariable>();
            IsDead.Variable = proxy.IsDead;

            // Replace WalkSpeed
            movement.WalkSpeed = ScriptableObject.CreateInstance<FloatVariable>();
            boosterController.WalkSpeed = movement.WalkSpeed;

            // Replace Dash Rate
            movement.DashRate = ScriptableObject.CreateInstance<FloatVariable>();
            boosterController.DashRate = movement.DashRate;

            // Replace FireRate
            weaponController.FireRate = ScriptableObject.CreateInstance<FloatVariable>();
            fire.FireRate.Variable = weaponController.FireRate;
        }

        void InitLocal()
        {
            CameraController cameraController = Instantiate(FollowCamera, Vector3.zero, Quaternion.identity).GetComponent<CameraController>();
            fire.FollowCamera = cameraController;

            proxy.SetCharacterAnimation();
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