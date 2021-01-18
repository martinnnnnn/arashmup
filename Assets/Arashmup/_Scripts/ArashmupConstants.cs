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
    internal static class CustomPropertiesKeys
    {
        public static readonly string VictoryCount = "VictoryCount";
        public static readonly string KillCount = "KillCount";
    }

    internal static class RuntimePrefabsPaths
    {
        public static readonly string PlayerCharacter = "PlayerCharacter";
    }

    internal static class RPC_Functions
    {
        public static readonly string SetDead = "RPC_SetDead";
        public static readonly string Kill = "RPC_Kill";
        public static readonly string KillBullet = "RPC_KillBullet";
        public static readonly string Add = "RPC_Add";
        public static readonly string DestroyObject = "RPC_DestroyObject";
        public static readonly string Fire = "RPC_Fire";
        public static readonly string Equip = "RPC_Equip";
        public static readonly string EnableInvincibleColor = "RPC_EnableInvincibleColor";
        public static readonly string SetCharacterAnimation = "PRC_SetCharacterAnimation";
        
    }

    internal static class PlayerPrefsNames
    {
        public static readonly string PlayerName = "PlayerName";
    }

    internal static class CharacterNames
    {
        public static readonly string Girl = "Girl";
        public static readonly string Hoodie = "Hoodie";
        public static readonly string Dragon = "Dragon";
    }
}