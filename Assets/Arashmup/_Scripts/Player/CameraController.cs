using UnityEngine;

namespace Arashmup
{
    public class CameraController : MonoBehaviour
    {
        public GenericReference<float> CameraDepth;
        public GenericReference<Vector3> PlayerPosition;
        public GameInputs Inputs;

        private Camera cam;

        public Vector2 GetWorldPoint()
        {
            if (cam == null)
            {
                cam = GetComponent<Camera>();
            }

            return cam.ScreenToWorldPoint(Inputs.Actions.Gameplay.FireGoal.ReadValue<Vector2>());
        }

        void FixedUpdate()
        {
            Vector3 newPosition = transform.position * 0.9f + PlayerPosition.Value * 0.1f;

            transform.position = new Vector3(newPosition.x, newPosition.y, CameraDepth);
        }
    }
}