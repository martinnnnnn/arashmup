using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Arashmup
{
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