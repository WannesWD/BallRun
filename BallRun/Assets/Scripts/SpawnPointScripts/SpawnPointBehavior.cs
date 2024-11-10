using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class SpawnPointBehavior : MonoBehaviour
{
    [SerializeField] private UnityEvent _onSpawnPointHit;

    private bool _wasHit = false;

    //respawn player or item as it enters trigger
    public void OnTriggerEnter(Collider other)
    {
        if (_wasHit) return;

        _onSpawnPointHit?.Invoke();
        PlayerCharacter player = other.gameObject.GetComponent<PlayerCharacter>();
        if (player)
        {
            player.RespawnPos = transform.position;
            player.SpawnPointChanged = true;
            //Debug.Log((Mathf.Asin(transform.rotation.y) / Mathf.PI * 180) * 2);
            player.RespawnAngle = (Mathf.Asin(transform.rotation.y) / Mathf.PI * 180) * 2;
            _wasHit = true;
        }

        ValuableItemBehavior valuableItem = other.gameObject.GetComponent<ValuableItemBehavior>();
        if (valuableItem)
        {
            valuableItem.RespawnPos = transform.position;
        }

    }
}
