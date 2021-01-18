using UnityEngine;
using UnityEngine.UI;

namespace Arashmup
{
    public class CharacterSelectionButtons : MonoBehaviour
    {
        public StringVariable AnimatorControllerName;

        void OnEnable()
        {
            foreach (Button button in GetComponentsInChildren<Button>())
            {
                if (button.name == AnimatorControllerName.Value)
                {
                    // select the button (so it is deselected when clicking on another one)
                    button.Select();
                    // highlight the button
                    button.OnSelect(null);
                    break;
                }
            }
        }
    }

}
