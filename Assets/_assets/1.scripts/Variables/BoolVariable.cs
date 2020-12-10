using UnityEngine;

namespace Arashmup
{
    [CreateAssetMenu]
    public class BoolVariable : ScriptableObject
    {
#if UNITY_EDITOR
        [Multiline]
        public string DeveloperDescription = "";
#endif
        private bool value = false;

        public bool Value
        {
            get { return value; }
            set { this.value = value; }
        }
    }
}