using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu: MonoBehaviour
{
    public enum Type
    {
        Title,
        Loading,
        RoomCreate,
        Room,
        RoomFind,
        Error
    }

    public Type type;

    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
