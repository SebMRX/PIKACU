using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Baddie : MonoBehaviour
{
    [SerializeField] private float maxHealth = 3f;
    [SerializeField] private float damageThreshold = 0.2f;
    [SerializeField] private GameObject baddieDeathParticle;


    private float currentHealth;


    private void Awake()
    {
        currentHealth = maxHealth;
    }



    public void DamageBaddie(float damageAmount)
    {
        currentHealth -= damageAmount;

        if (currentHealth <= 0f)
        {
            Die();
        }
    }



    private void Die()
    {
        GameManager.instance.RemoveBaddie(this);

        Instantiate(baddieDeathParticle, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        float impactVelocity = collision.relativeVelocity.magnitude;
        if (impactVelocity > damageThreshold)
        {
            DamageBaddie(impactVelocity);
        }
    }
}