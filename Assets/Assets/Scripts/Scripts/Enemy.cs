using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    #region Movement_vars
    public float movespeed;

    #endregion

    #region Physics_comps
    Rigidbody2D EnemyRB;
    #endregion

    #region Targetting_vars
    public Transform player;
    #endregion

    #region Attack_vars
    public float dmg;
    public float explosionRadius;
    public GameObject explosionPrefab;
    public GameObject dropPrefab;
    public float dropChance = 0.5f;
    #endregion

    #region Health_vars
    public float maxHealth;
    float currHealth;
    #endregion


    #region Unity_funcs

    private void Awake()
    {
        EnemyRB = GetComponent<Rigidbody2D>();
        currHealth = maxHealth;
    }

    private void Update()
    {
        if(player == null)
        {
            return;
        }

        Move();
    }

    #endregion

    #region Movement_funcs
    private void Move()
    {
        //calculate movement vector
        Vector2 direction = player.position - transform.position;

        EnemyRB.velocity = direction.normalized * movespeed;
    }
    #endregion

    
    #region Attack_funcs
    private void Explode()
    {
        //sound
        FindObjectOfType<AudioManager>().Play("Explosion");

        //raycast
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, explosionRadius, Vector2.zero);

        foreach(RaycastHit2D hit in hits)
        {
            if (hit.transform.CompareTag("Player"))
            {
                //cause damage
                Debug.Log("Ouch tons of damage");

                //spawn explode
                Instantiate(explosionPrefab, transform.position, transform.rotation);
                hit.transform.GetComponent<PlayerController>().TakeDamage(dmg);


            }
        }
        //deactivate
        Destroy(this.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.CompareTag("Player"))
        {
            Explode();
        }
    }
    #endregion

    #region Health_funcs
    public void TakeDamage(float value)
    {
        //sound
        FindObjectOfType<AudioManager>().Play("bleh");
        
        //decrement hp
        currHealth -= value;
        Debug.Log("Enemy health is now: " + currHealth.ToString());

        //check for death
        if (currHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        //chance to spawn drop
        float drop = Random.Range(0f, 1f);
        if (drop > 0.5f)
        {
            Instantiate(dropPrefab, transform.position, transform.rotation);
        }
        //destroy
        Destroy(this.gameObject);
    }
    #endregion

}
