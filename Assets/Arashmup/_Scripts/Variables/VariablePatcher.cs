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

    public class VariablePatcher<T> : MonoBehaviour
    {
        public GenericVariable<T> Variable;
        public List<GenericReference<T>> References;

        void OnEnable()
        {
            Variable = ScriptableObject.CreateInstance<GenericVariable<T>>();
            References.ForEach(r => r.Variable = Variable);

            Destroy(gameObject);
        }
    }

}


// walk speed
// fire rate
// name
// dash rate