using UnityEngine;

namespace Arashmup
{
    [CreateAssetMenu]
    public class Vector2Variable : GenericVariable<Vector2>
    {
        public void ApplyChange(Vector2 amount)
        {
            Value += amount;
        }

        public void ApplyChange(Vector2Variable amount)
        {
            Value += amount.Value;
        }
    }
}