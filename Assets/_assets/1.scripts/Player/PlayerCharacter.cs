using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Arashmup
{
    public class PlayerCharacter : MonoBehaviour
    {
        #region Runtime Set
        public PlayerCharacterRuntimeSet RuntimeSet;

        private void OnEnable()
        {
            RuntimeSet.Add(this);
        }

        private void OnDisable()
        {
            RuntimeSet.Remove(this);
        }
        #endregion
    }
}