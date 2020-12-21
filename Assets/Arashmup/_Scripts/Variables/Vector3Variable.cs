using UnityEngine;

namespace Arashmup
{
    [CreateAssetMenu]
    public class Vector3Variable : GenericVariable<Vector3>
    {
        public void ApplyChange(Vector3 amount)
        {
            Value += amount;
        }

        public void ApplyChange(Vector3Variable amount)
        {
            Value += amount.Value;
        }
    }
}