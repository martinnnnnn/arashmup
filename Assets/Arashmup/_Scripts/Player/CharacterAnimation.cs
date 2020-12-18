using System;
using System.Collections.Generic;
using UnityEngine;

namespace Arashmup
{
    public class CharacterAnimation : MonoBehaviour
    {
        [Serializable]
        public struct Anim
        {
            public string name;
            public RuntimeAnimatorController controller;
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
