using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public Slider healthSlider;
    public Slider hydrationSlider;

    public float maxHealth = 100f;
    public float maxHydration = 100f;
    public float thirstSpeed = 1f;

    float health;
    float hydration;

    void Start()
    {
        health = maxHealth;
        hydration = maxHydration;
    }

    void Update()
    {
        hydration -= Time.deltaTime * thirstSpeed;

        healthSlider.value = health; 
        hydrationSlider.value = hydration;     
    }

    public void UpdateHealthAndHydration(float healthUpdate, float hydrationUpdate)
    {
        health += healthUpdate;
        hydration += hydrationUpdate;

        if (health > maxHealth)
        {
            health = maxHealth;
        }
        if (hydration > maxHydration)
        {
            hydration = maxHydration;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other);
        if (other.CompareTag("Enemy Weapon"))
        {
            UpdateHealthAndHydration(-20, 0);
        }
    }
}
