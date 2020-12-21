using System;
using System.Collections.Generic;
using UnityEngine;

namespace Arashmup
{
    [Serializable]
    public abstract class GenericVariable<T> : ScriptableObject
    {
        [SerializeField]
        public T Value;

        public void SetValue(T value)
        {
            Value = value;
        }

        public void SetValue(GenericVariable<T> value)
        {
            Value = value.Value;
        }
    }
}