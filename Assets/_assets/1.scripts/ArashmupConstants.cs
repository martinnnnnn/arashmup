﻿using System.Collections;
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
        public static readonly string PlayerCharacter = Path.Combine("Prefabs", "PlayerCharacter");
    }

    internal static class RPC_Functions
    {
        public static readonly string SetDead = "RPC_SetDead";
        public static readonly string Add = "RPC_Add";
        public static readonly string DestroyObject = "RPC_DestroyObject";
        public static readonly string Fire = "RPC_Fire";
        public static readonly string Equip = "RPC_Equip";
    }

    internal static class PlayerPrefsNames
    {
        public static readonly string PlayerName = "PlayerName";
    }
}