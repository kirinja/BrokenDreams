using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    //public Slider healthSlider;

	public Heart[] hearts;

    private Controller3D controller3D;
    private PlayerAttributes playerAttributes;
    bool isDead;
    //bool damaged;

    private void Start()
    {
        UpdateHearts();
    }

    private void Update()
    {
        /*
		for (var i = 0; i < hearts.Length; ++i) {
			if (playerAttributes.currentHealth > i) {
				hearts [i].SetFilled (true);
			} else {
				hearts [i].SetFilled (false);
			}
		}*/
    }

    void UpdateHearts()
    {
        for (var i = 0; i < hearts.Length; ++i)
        {
            hearts[i].SetFilled(playerAttributes.currentHealth > i);
        }
    }

    private void Awake()
    {

        controller3D = GetComponent<Controller3D>();

        playerAttributes = GetComponent<PlayerAttributes>();

        playerAttributes.currentHealth = playerAttributes.MaxHP;

    }

    // Returns true if player dies
    public bool TakeDamage(int amount)
    {
        //damaged = true;

        playerAttributes.currentHealth -= amount;

        UpdateHearts();


       // healthSlider.value = playerAttributes.currentHealth;

        if (playerAttributes.currentHealth <= 0 && !isDead)
        {
            Death();
            return true;
        }

        return false;
    }

    public void Heal(int amount)
    {
        var health = playerAttributes.currentHealth + amount;
        if (health > playerAttributes.MaxHP)
        {
            health = playerAttributes.MaxHP;
        }
        playerAttributes.currentHealth = health;
        UpdateHearts();
        //healthSlider.value = health;
    }

    void Death()
    {
        isDead = true;
        controller3D.Kill();

        //Controller3D.enabled = false;
    }

    public void Respawn()
    {
        isDead = false;
        playerAttributes.currentHealth = playerAttributes.MaxHP;
        UpdateHearts();
        //healthSlider.value = playerAttributes.currentHealth;
    }
}
