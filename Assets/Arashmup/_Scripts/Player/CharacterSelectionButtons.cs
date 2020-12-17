﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Arashmup
{
    public class CharacterSelectionButtons : MonoBehaviour
    {
        void OnEnable()
        {
            foreach (Button button in GetComponentsInChildren<Button>())
            {
                if (button.name == PlayerPrefs.GetString(PlayerPrefsNames.PlayerCharacter))
                {
                    // select the button (so that is deselected when clicking on another one)
                    button.Select();
                    // highlight the button
                    button.OnSelect(null);
                    break;
                }
            }
        }
    }

}
