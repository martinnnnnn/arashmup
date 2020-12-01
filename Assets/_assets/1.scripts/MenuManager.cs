using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    List<Menu> menus = new List<Menu>();

    public void Awake()
    {
        Instance = this;

        foreach (Transform t in transform)
        {
            Menu menu = t.GetComponent<Menu>();
            if (menu != null)
            {
                menus.Add(menu);
            }
        }
    }

    public void OpenMenu(Menu.Type t)
    {
        menus.ForEach(menu =>
        {
            if (menu.type == t)
            {
                menu.Open();
            }
            else
            {
                menu.Close();
            }
        });
    }

    public void OpenMenu(Menu menu)
    {
        OpenMenu(menu.type);
    }

    public void CloseMenu(Menu menu)
    {
        menu.Close();
    }
}
