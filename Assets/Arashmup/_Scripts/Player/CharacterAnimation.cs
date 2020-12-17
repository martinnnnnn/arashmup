using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;

namespace Arashmup
{
    public class CharacterAnimation : MonoBehaviour
    {
        [Serializable]
        public struct Anim
        {
            public string name;
            public AnimatorController controller;
        }

        public List<Anim> Animators;

        void Start()
        {
            string animatorName = PlayerPrefs.GetString(PlayerPrefsNames.PlayerCharacter);
            foreach (Anim anim in Animators)
            {
                if (anim.name == animatorName)
                {
                    GetComponent<Animator>().runtimeAnimatorController = anim.controller;
                    break;
                }
            }
        }
    }
}
