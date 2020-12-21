using UnityEngine;

namespace Arashmup
{
    [CreateAssetMenu]
    public class FloatVariable : GenericVariable<float>
    {
        public void ApplyChange(float amount)
        {
            Value += amount;
        }

        public void ApplyChange(FloatVariable amount)
        {
            Value += amount.Value;
        }
    }
}