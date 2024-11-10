using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RespawnTriggerBoxBehavior : MonoBehaviour
{
    [SerializeField] UnityEvent _onRespawn;

    private PlayerCharacter _playerCharacter = null;
    private ValuableItemBehavior _valuableItem = null;

    //set player and first valuable item
    private void Start()
    {
        PlayerCharacter player = FindObjectOfType<PlayerCharacter>();

        if (player) _playerCharacter = player;

        ValuableItemBehavior item = FindObjectOfType<ValuableItemBehavior>();

        if (item) _valuableItem = item;
    }

    //respawn player/item depending on state
    public void OnTriggerEnter(Collider other)
    {
        if (_playerCharacter == null) return;

        if(_valuableItem == null)
        {
            _valuableItem = _playerCharacter.getAttachedItem();
        }

        if (other.gameObject == _playerCharacter.gameObject)
        {
            _playerCharacter.Respawn();

            if (_valuableItem) _valuableItem.Respawn();

            _onRespawn?.Invoke();
        }

        ValuableItemBehavior valuableItem = other.gameObject.GetComponent<ValuableItemBehavior>();
        if (valuableItem)
        {
            valuableItem.Respawn();
        }
    }
}
