using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicCharacter : MonoBehaviour
{
    protected MovementBehavior _movementBehavior;
    protected PickupBehavior _pickupBehavior;
    protected PlayerCharacterEffectManager _characterEffectManager;

    protected virtual void Awake()
    {
        _movementBehavior = GetComponent<MovementBehavior>();
        _pickupBehavior = GetComponent<PickupBehavior>();
        _characterEffectManager = GetComponent<PlayerCharacterEffectManager>();
    }
}
