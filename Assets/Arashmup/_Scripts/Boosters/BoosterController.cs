using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Realtime;
using TMPro;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.IO;
using System;
using ExitGames.Client.Photon;

namespace Arashmup
{
    public class BoosterController : MonoBehaviour
    {
        [Header("Walking")]
        public GenericReference<float> WalkSpeedStandard;
        public FloatVariable WalkSpeed;

        [Header("Dashing")]
        public GenericReference<float> DashRateStandard;
        public GenericReference<float> DashRateBooster;
        public FloatVariable DashRate;


        CharacterProxy proxy;
        List<Booster> currentBoosters;
        SpriteRenderer spriteRenderer;

        void Start()
        {
            CustomTypesSerialization.Register();

            currentBoosters = new List<Booster>();
            proxy = GetComponent<CharacterProxy>();
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        void Update()
        {
            currentBoosters.RemoveAll(booster =>
            {
                bool remove = false;

                if (booster.useDuration)
                {
                    booster.durationLeft -= Time.deltaTime;

                    if (booster.durationLeft <= 0)
                    {
                        OnRemove(booster.type);
                        remove = true;
                    }
                }

                return remove;
            });
        }

        public bool IsDamageDone(int damage)
        {
            bool damageDone = true;

            currentBoosters.RemoveAll(b =>
            {
                bool remove = false;

                switch (b.type)
                {
                    case Booster.Type.Shield:
                        damageDone = false;
                        b.strength--;
                        if (b.strength <= 0)
                        {
                            remove = true;
                        }
                        break;

                    case Booster.Type.Invincible:
                        damageDone = false;
                        break;
                }

                return remove;
            });

            return damageDone;
        }

        public void Add(Booster booster)
        {
            currentBoosters.RemoveAll(b =>
            {
                if (b.type == booster.type)
                {
                    OnRemove(b.type);
                    return true;
                }

                return false;
            });


            booster.durationLeft = booster.duration;

            switch (booster.type)
            {
                case Booster.Type.Shield:
                    break;
                case Booster.Type.Speed:
                    WalkSpeed.SetValue(booster.walkSpeedBooster);
                    break;
                case Booster.Type.Invincible:
                    proxy.EnableInvincibleColor(true);
                    break;
                case Booster.Type.NoCooldownDash:
                    DashRate.SetValue(DashRateBooster);
                    break;
            }

            currentBoosters.Add(booster);
        }

        void OnRemove(Booster.Type boosterType)
        {
            switch (boosterType)
            {
                case Booster.Type.Speed:
                    WalkSpeed.SetValue(WalkSpeedStandard);
                    break;
                case Booster.Type.Invincible:
                    proxy.EnableInvincibleColor(false);
                    break;
                case Booster.Type.NoCooldownDash:
                    DashRate.SetValue(DashRateStandard);
                    break;
            }
        }
    }
}