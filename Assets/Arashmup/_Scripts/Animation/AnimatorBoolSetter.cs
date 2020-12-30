using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Arashmup
{
    public class AnimatorBoolSetter : MonoBehaviour
    {
        public GenericReference<bool> Variable;
        public string ParameterName;

        Animator animator;
        [SerializeField] private int parameterHash;

        void Start()
        {
            animator = GetComponent<Animator>();
        }

        private void OnValidate()
        {
            parameterHash = Animator.StringToHash(ParameterName);
        }

        private void Update()
        {
            animator.SetBool(parameterHash, Variable.Value);
        }
    }
}