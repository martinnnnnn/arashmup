﻿using System.Collections;
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
        PlayerController playerController;
        PhotonView photonView;
        List<Booster> currentBoosters;

        SpriteRenderer spriteRenderer;

        [Header("Walking")]
        public FloatReference WalkSpeedStandard;
        public FloatVariable WalkSpeed;

        [Header("Dashing")]
        public FloatReference DashRateStandard;
        public FloatReference DashRateBooster;
        public FloatVariable DashRate;

        void Start()
        {
            CustomTypesSerialization.Register();

            currentBoosters = new List<Booster>();
            playerController = GetComponent<PlayerController>();
            photonView = GetComponent<PhotonView>();
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        void Update()
        {
            currentBoosters.ForEach(booster =>
            {
                if (booster.useDuration)
                {
                    switch (booster.type)
                    {
                        case Booster.Type.Invincible:

                            float r = (Mathf.Sin((Time.timeSinceLevelLoad * 10.0f + (Mathf.PI * 2.0f * 1.0f / 3.0f))) + 1.0f) / 2.0f;
                            float g = (Mathf.Sin((Time.timeSinceLevelLoad * 10.0f + (Mathf.PI * 2.0f * 2.0f / 3.0f))) + 1.0f) / 2.0f;
                            float b = (Mathf.Sin((Time.timeSinceLevelLoad * 10.0f + (Mathf.PI * 2.0f * 3.0f / 3.0f))) + 1.0f) / 2.0f;

                            spriteRenderer.color = new Color(r, g, b);

                            break;
                    }

                    if (booster.durationLeft <= 0)
                    {
                        // On end
                        switch (booster.type)
                        {
                            case Booster.Type.Speed:
                                WalkSpeed.SetValue(WalkSpeedStandard);
                                break;
                            case Booster.Type.Invincible:
                                spriteRenderer.color = Color.white;
                                break;
                            case Booster.Type.NoCooldownDash:
                                DashRate.SetValue(DashRateStandard);
                                break;
                        }
                    }

                    booster.durationLeft -= Time.deltaTime;
                }
            });

            currentBoosters.RemoveAll(booster =>
            {
                bool remove = false;

                if (booster.useDuration)
                {
                    booster.durationLeft -= Time.deltaTime;

                    // On update
                    switch (booster.type)
                    {
                        case Booster.Type.Invincible:
                            
                            float r = (Mathf.Sin((Time.timeSinceLevelLoad * 10.0f + (Mathf.PI * 2.0f * 1.0f / 3.0f))) + 1.0f) / 2.0f;
                            float g = (Mathf.Sin((Time.timeSinceLevelLoad * 10.0f + (Mathf.PI * 2.0f * 2.0f / 3.0f))) + 1.0f) / 2.0f;
                            float b = (Mathf.Sin((Time.timeSinceLevelLoad * 10.0f + (Mathf.PI * 2.0f * 3.0f / 3.0f))) + 1.0f) / 2.0f;

                            spriteRenderer.color = new Color(r, g, b);

                            break;
                    }

                    if (booster.durationLeft <= 0)
                    {
                        remove = true;

                        // On end
                        switch (booster.type)
                        {
                            case Booster.Type.Speed:
                                WalkSpeed.SetValue(WalkSpeedStandard);
                                break;
                            case Booster.Type.Invincible:
                                spriteRenderer.color = Color.white;
                                break;
                            case Booster.Type.NoCooldownDash:
                                DashRate.SetValue(DashRateStandard);
                                break;
                        }
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
            photonView.RPC(RPC_Functions.Add, RpcTarget.All, booster);
        }

        [PunRPC]
        void RPC_Add(Booster booster)
        {
            currentBoosters.RemoveAll(b => b.GetType() == booster.GetType());

            booster.durationLeft = booster.duration;

            switch (booster.type)
            {
                case Booster.Type.Shield:
                    break;
                case Booster.Type.Speed:
                    WalkSpeed.SetValue(booster.WalkSpeedBooster);
                    break;
                case Booster.Type.Invincible:
                    break;
                case Booster.Type.NoCooldownDash:
                    DashRate.SetValue(DashRateBooster);
                    break;
            }

            currentBoosters.Add(booster);
        }
    }
}