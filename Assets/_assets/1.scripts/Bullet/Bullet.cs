using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Arashmup
{
    public class Bullet : MonoBehaviour
    {
        [HideInInspector] public int ID;

        #region Runtime Set
        public BulletRuntimeSet RuntimeSet;
    
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
