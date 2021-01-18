using System;
using System.Collections.Generic;
using UnityEngine;

namespace Arashmup
{
    public class CharacterAnimation : MonoBehaviour
    {
        public List<RuntimeAnimatorController> AnimControllers;

        public void SetAnim(string animatorName)
        {
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
