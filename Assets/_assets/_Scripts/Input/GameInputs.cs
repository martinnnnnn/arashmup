using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameInputs : ScriptableObject
{
    [HideInInspector] public ArashmupInputActions Actions;
    void OnEnable()
    {
        Actions = new ArashmupInputActions();
        Actions.Enable();
    }

    void OnDisable()
    {
        Actions.Enable();
    }
}
