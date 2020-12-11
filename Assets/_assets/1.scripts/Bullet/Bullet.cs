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
            Debug.Log("added bullet " + ID);
            RuntimeSet.Add(this);
        }
    
        private void OnDisable()
        {
            Debug.Log("removed bullet " + ID);
            RuntimeSet.Remove(this);
        }
        #endregion
    }
}
