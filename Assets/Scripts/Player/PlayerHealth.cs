using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour {


	public Slider healthSlider;

	Controller3D controller3D;

	PlayerAttributes playerAttributes;

	bool isDead;

	bool damaged;


	void Start () {
		
	}

	void Update () {
		
	}

	void Awake() {
		
		controller3D = GetComponent<Controller3D> ();

		playerAttributes = GetComponent<PlayerAttributes> ();

		playerAttributes.currentHealth = playerAttributes.MaxHP;

	}

	public void TakeDamage(int amount){

		damaged = true;

		playerAttributes.currentHealth -= amount;

		healthSlider.value = playerAttributes.currentHealth;

		if (playerAttributes.currentHealth <= 0 && !isDead) {
			Death ();
		}
	}

	void Death() {
		isDead = true;
        controller3D.Kill();

		//Controller3D.enabled = false;
	}

    public void Respawn()
    {
        isDead = false;
        playerAttributes.currentHealth = playerAttributes.MaxHP;
        healthSlider.value = playerAttributes.currentHealth;
    }
}
