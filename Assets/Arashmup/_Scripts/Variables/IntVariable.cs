using UnityEngine;

namespace Arashmup
{
    [CreateAssetMenu]
    public class IntVariable : GenericVariable<int>
    {
        public void ApplyChange(int amount)
        {
            Value += amount;
        }

        public void ApplyChange(IntVariable amount)
        {
            Value += amount.Value;
        }
    }
}