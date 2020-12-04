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

[Serializable]
public class Booster : ScriptableObject
{
    public float duration;
    [HideInInspector] public float durationLeft;
}

[CreateAssetMenu(fileName = "Booster_Shield", menuName = "MyAssets/Booster/Booster_Shield"), Serializable]
public class Booster_Shield : Booster
{
    public int count;
}

[CreateAssetMenu(fileName = "Booster_Invincible", menuName = "MyAssets/Booster/Booster_Invincible"), Serializable]
public class Booster_Invincible : Booster
{
}

[CreateAssetMenu(fileName = "Booster_Speed", menuName = "MyAssets/Booster/Booster_Speed"), Serializable]
public class Booster_Speed : Booster
{
    public float value;
}

[CreateAssetMenu(fileName = "Booster_NoCooldownDash", menuName = "MyAssets/Booster/Booster_NoCooldownDash"), Serializable]
public class Booster_NoCooldownDash : Booster
{
}

internal static class ArashmupCustomTypes
{
    /// <summary>Register de/serializer methods for PUN specific types. Makes the type usable in RaiseEvent, RPC and sync updates of PhotonViews.</summary>
    internal static void Register()
    {
        PhotonPeer.RegisterType(typeof(Booster), (byte)1, SerializeBooster, DeserializeBooster);
        PhotonPeer.RegisterType(typeof(Booster_Shield), (byte)2, SerializeBooster_Shield, DeserializeBooster_Shield);
        PhotonPeer.RegisterType(typeof(Booster_Invincible), (byte)3, SerializeBooster_Invincible, DeserializeBooster_Invincible);
        PhotonPeer.RegisterType(typeof(Booster_Speed), (byte)4, SerializeBooster_Speed, DeserializeBooster_Speed);
        PhotonPeer.RegisterType(typeof(Booster_NoCooldownDash), (byte)5, SerializeBooster_NoCooldownDash, DeserializeBooster_NoCooldownDash);
    }

    #region Booster
    public static readonly byte[] memBooster = new byte[4];

    private static short SerializeBooster(StreamBuffer outStream, object customobject)
    {
        Booster booster = ((Booster)customobject);

        lock (memBooster)
        {
            byte[] bytes = memBooster;
            int off = 0;
            Protocol.Serialize(booster.duration, bytes, ref off);
            outStream.Write(bytes, 0, 4);
        }
        return 4;
    }

    private static object DeserializeBooster(StreamBuffer inStream, short length)
    {
        Booster booster = new Booster();

        lock (memBooster)
        {
            inStream.Read(memBooster, 0, length);
            int off = 0;
            Protocol.Deserialize(out booster.duration, memBooster, ref off);
        }

        return booster;
    }

    #endregion

    #region Booster_Shield
    public static readonly byte[] memBooster_Shield = new byte[4 * 2];

    private static short SerializeBooster_Shield(StreamBuffer outStream, object customobject)
    {
        Booster_Shield shield = ((Booster_Shield)customobject);

        lock (memBooster_Shield)
        {
            byte[] bytes = memBooster_Shield;
            int off = 0;
            Protocol.Serialize(shield.duration, bytes, ref off);
            Protocol.Serialize(shield.count, bytes, ref off);
            outStream.Write(bytes, 0, 4 * 2);
        }
        return 4 * 2;
    }

    private static object DeserializeBooster_Shield(StreamBuffer inStream, short length)
    {
        Booster_Shield shield = new Booster_Shield();

        lock (memBooster_Shield)
        {
            inStream.Read(memBooster_Shield, 0, length);
            int off = 0;
            Protocol.Deserialize(out shield.duration, memBooster_Shield, ref off);
            Protocol.Deserialize(out shield.count, memBooster_Shield, ref off);
        }

        return shield;
    }
    #endregion

    #region Booster_Invincible

    public static readonly byte[] memBooster_Invincible = new byte[4];

    private static short SerializeBooster_Invincible(StreamBuffer outStream, object customobject)
    {
        Booster_Invincible invincible = ((Booster_Invincible)customobject);

        lock (memBooster_Invincible)
        {
            byte[] bytes = memBooster_Invincible;
            int off = 0;
            Protocol.Serialize(invincible.duration, bytes, ref off);
            outStream.Write(bytes, 0, 4);
        }
        return 4;
    }

    private static object DeserializeBooster_Invincible(StreamBuffer inStream, short length)
    {
        Booster_Invincible invincible = new Booster_Invincible();

        lock (memBooster_Invincible)
        {
            inStream.Read(memBooster_Invincible, 0, length);
            int off = 0;
            Protocol.Deserialize(out invincible.duration, memBooster_Invincible, ref off);
        }

        return invincible;
    }
    #endregion

    #region Booster_Speed
    public static readonly byte[] memBooster_Speed = new byte[4 * 2];

    private static short SerializeBooster_Speed(StreamBuffer outStream, object customobject)
    {
        Booster_Speed speed = ((Booster_Speed)customobject);

        lock (memBooster_Speed)
        {
            byte[] bytes = memBooster_Speed;
            int off = 0;
            Protocol.Serialize(speed.duration, bytes, ref off);
            Protocol.Serialize(speed.value, bytes, ref off);
            outStream.Write(bytes, 0, 4 * 2);
        }
        return 4 * 2;
    }

    private static object DeserializeBooster_Speed(StreamBuffer inStream, short length)
    {
        Booster_Speed speed = new Booster_Speed();

        lock (memBooster_Speed)
        {
            inStream.Read(memBooster_Speed, 0, length);
            int off = 0;
            Protocol.Deserialize(out speed.duration, memBooster_Speed, ref off);
            Protocol.Deserialize(out speed.value, memBooster_Speed, ref off);
        }

        return speed;
    }
    #endregion

    #region Booster_NoCooldownDash

    public static readonly byte[] memBooster_NoCooldownDash = new byte[4];
    private static short SerializeBooster_NoCooldownDash(StreamBuffer outStream, object customobject)
    {
        Booster_NoCooldownDash dash = ((Booster_NoCooldownDash)customobject);

        lock (memBooster_NoCooldownDash)
        {
            byte[] bytes = memBooster_NoCooldownDash;
            int off = 0;
            Protocol.Serialize(dash.duration, bytes, ref off);
            outStream.Write(bytes, 0, 4);
        }
        return 4;
    }

    private static object DeserializeBooster_NoCooldownDash(StreamBuffer inStream, short length)
    {
        Booster_NoCooldownDash dash = new Booster_NoCooldownDash();

        lock (memBooster_NoCooldownDash)
        {
            inStream.Read(memBooster_NoCooldownDash, 0, length);
            int off = 0;
            Protocol.Deserialize(out dash.duration, memBooster_NoCooldownDash, ref off);
        }

        return dash;
    }
    #endregion
}