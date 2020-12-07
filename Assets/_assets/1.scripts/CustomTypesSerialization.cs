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

internal static class CustomTypesSerialization
{
    internal static void Register()
    {
        PhotonPeer.RegisterType(typeof(Booster), (byte)'B', SerializeBooster, DeserializeBooster);
    }

    #region Booster
    public static readonly short memBoosterSize = sizeof(Booster.Type) + sizeof(int) + sizeof(float) + sizeof(int) + sizeof(float);
    public static readonly byte[] memBooster = new byte[memBoosterSize];

    private static short SerializeBooster(StreamBuffer outStream, object customobject)
    {
        Booster booster = ((Booster)customobject);

        lock (memBooster)
        {
            byte[] bytes = memBooster;
            int off = 0;
            Protocol.Serialize((int)booster.type, bytes, ref off);
            Protocol.Serialize(Convert.ToInt32(booster.useDuration), bytes, ref off);
            Protocol.Serialize(booster.duration, bytes, ref off);
            Protocol.Serialize(booster.strength, bytes, ref off);
            Protocol.Serialize(booster.speed, bytes, ref off);
            outStream.Write(bytes, 0, memBoosterSize);
        }
        return memBoosterSize;
    }

    private static object DeserializeBooster(StreamBuffer inStream, short length)
    {
        if (length != memBoosterSize)
        {
            Debug.LogError("Length reveiced should be " + memBoosterSize);
        }

        Booster booster = ScriptableObject.CreateInstance<Booster>();

        lock (memBooster)
        {
            inStream.Read(memBooster, 0, length);
            int off = 0;

            int boosterType = -1;
            Protocol.Deserialize(out boosterType, memBooster, ref off);
            booster.type = (Booster.Type)boosterType;

            int boosterUseDuration = 0;
            Protocol.Deserialize(out boosterUseDuration, memBooster, ref off);
            booster.useDuration = Convert.ToBoolean(boosterUseDuration);

            Protocol.Deserialize(out booster.duration, memBooster, ref off);
            Protocol.Deserialize(out booster.strength, memBooster, ref off);
            Protocol.Deserialize(out booster.speed, memBooster, ref off);
        }

        return booster;
    }

    #endregion
}