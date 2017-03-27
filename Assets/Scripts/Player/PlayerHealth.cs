using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public Slider healthSlider;

    private Controller3D controller3D;
    private PlayerAttributes playerAttributes;
    bool isDead;
    //bool damaged;

    private void Start()
    {

    }

    private void Update()
    {

    }

    private void Awake()
    {

        controller3D = GetComponent<Controller3D>();

        playerAttributes = GetComponent<PlayerAttributes>();

        playerAttributes.currentHealth = playerAttributes.MaxHP;

    }

    public void TakeDamage(int amount)
    {
        //damaged = true;

        playerAttributes.currentHealth -= amount;

        healthSlider.value = playerAttributes.currentHealth;

        if (playerAttributes.currentHealth <= 0 && !isDead)
        {
            Death();
        }
    }

    public void Heal(int amount)
    {
        var health = playerAttributes.currentHealth + amount;
        if (health > playerAttributes.MaxHP)
        {
            health = playerAttributes.MaxHP;
        }
        playerAttributes.currentHealth = health;
        healthSlider.value = health;
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
        healthSlider.value = playerAttributes.currentHealth;
    }
}
