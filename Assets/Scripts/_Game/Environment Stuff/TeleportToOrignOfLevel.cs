using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(BoxCollider))]
public class TeleportToOrignOfLevel : MonoBehaviour
{
    [SerializeField] private PlayerPosessionManager possesionManager;
    [SerializeField] private Transform respawnPosition;

    public UnityEngine.Events.UnityEvent OnDeath;

    private BoxCollider _boxCollider;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            OnDeath?.Invoke();
            possesionManager.MoveToPositionAndSplit(respawnPosition.transform.position);
        }
    }

    private void OnValidate()
    {
        _boxCollider = GetComponent<BoxCollider>();
    }
}
