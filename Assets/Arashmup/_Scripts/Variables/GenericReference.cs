using System;
using System.Collections.Generic;
using UnityEngine;

namespace Arashmup
{

    [Serializable]
    public class GenericReference<T>
    {
        public bool UseConstant = true;
        public T ConstantValue;

        [SerializeField]
        public GenericVariable<T> Variable;

        public GenericReference()
        { }

        public GenericReference(T value)
        {
            UseConstant = true;
            ConstantValue = value;
        }

        public T Value
        {
            get { return UseConstant ? ConstantValue : Variable.Value; }
        }

        public static implicit operator T(GenericReference<T> reference)
        {
            return reference.Value;
        }
    }



}


// walk speed
// fire rate
// name
// dash rate