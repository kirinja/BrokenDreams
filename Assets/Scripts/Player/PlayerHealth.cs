using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour {


	public int currentHealth;

	public Slider healthSlider;

	Controller2D controller2D;

	PlayerAttributes playerAttributes;

	bool isDead;

	bool damaged;


	void Start () {
		
	}

	void Update () {
		
	}

	void Awake() {
		
		controller2D = GetComponent<Controller2D> ();

		playerAttributes = GetComponent<PlayerAttributes> ();

		currentHealth = playerAttributes.MaxHP;

	}

	public void TakeDamage(int amount){

		damaged = true;

		currentHealth -= amount;

		healthSlider.value = currentHealth;

		if (currentHealth <= 0 && !isDead) {
			Death ();
		}
	}

	void Death() {
		isDead = true;

		//Controller2D.enabled = false;
	}
}
