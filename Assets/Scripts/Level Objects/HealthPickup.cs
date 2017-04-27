using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class HealthPickup : MonoBehaviour {

	private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerHealth>().Heal(1);
            other.GetComponentInChildren<PlayerAudio>().PlayPickupSound();
            Destroy(this.gameObject);
        }
    }
}
