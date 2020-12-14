using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Realtime;
using Photon.Pun;
using System.IO;
using TMPro;
using DG.Tweening;
using System;
using ExitGames.Client.Photon;

namespace Arashmup
{
    public class Booster : ScriptableObject
    {
        public enum Type
        {
            Shield,
            Invincible,
            Speed,
            NoCooldownDash
        }

        public Type type;
        public bool useDuration;

        [ConditionalHide("useDuration", true)]
        public float duration;

        [HideInInspector] public float durationLeft;

        [Header("Shield")]
        public int strength;

        [Header("Speed")]
        public float walkSpeedBooster;
    }
}
