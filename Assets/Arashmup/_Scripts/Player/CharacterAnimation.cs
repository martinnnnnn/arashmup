using System;
using System.Collections.Generic;
using UnityEngine;

namespace Arashmup
{
    public class CharacterAnimation : MonoBehaviour
    {
        public List<RuntimeAnimatorController> AnimControllers;

        void Start()
        {
            string animatorName = PlayerPrefs.GetString(PlayerPrefsNames.PlayerCharacter);
            foreach (RuntimeAnimatorController animController in AnimControllers)
            {
                if (animController.name == animatorName)
                {
                    GetComponent<Animator>().runtimeAnimatorController = animController;
                    break;
                }
            }
        }
    }
}
