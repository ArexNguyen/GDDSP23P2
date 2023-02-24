using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    #region Editor Variable
    [SerializeField]
    [Tooltip("Speed of the enemy")]
    private float speed = 4;

    [SerializeField]
    [Tooltip("field of view")]
    private FieldOfView fieldOfView;

    [SerializeField]
    [Tooltip("Initial moving direction")]
    private Vector2 initVector;

    [SerializeField] 
    [Tooltip("the player's transform")]
    private Transform playerTransform;

    [SerializeField]
    [Tooltip("the player")]
    private PlayerController playerController;

    [SerializeField]
    [Tooltip("time to die")]
    private float timeToDie;

    [SerializeField]
    [Tooltip("time between each attack")]
    private float attackCoolDown;

    [SerializeField]
    [Tooltip("damage of the enemy")]
    private float damage;
    #endregion

    #region Private Variables
    private Vector2 northVector = new Vector2(0, 1);
    private Vector2 southVector = new Vector2(0, -1);
    private Vector2 eastVector = new Vector2(1, 0);
    private Vector2 westVector = new Vector2(-1, 0);
    private Vector2 movementVector;
    private Animator anim;
    private bool detected;
    private bool dead;
    private float deadtimer;
    private float attackTimer;
    #endregion

    #region Cached Components
    private Rigidbody2D cc_Enemy;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        attackTimer = attackCoolDown;
        dead = false;
        deadtimer = timeToDie;
        anim = GetComponent<Animator>();
        initVector.Normalize();
        movementVector = initVector;
        cc_Enemy = GetComponent<Rigidbody2D>();
        fieldOfView.SetOrigin(transform.position);
        fieldOfView.SetAimDirection(movementVector);
    }

    // Update is called once per frame
    void Update()
    {     
       if (dead)
        {
            movementVector = new Vector2(0, 0);
            deadtimer -= Time.deltaTime;
            if (deadtimer <= 0)
            {
                
                Destroy(this.gameObject);
                Destroy(fieldOfView.gameObject);
            }
        }
       if (fieldOfView.fovDetected())
        {
            detected = true;
        }
       fieldOfView.SetOrigin(transform.position);
       fieldOfView.SetAimDirection(movementVector);
       moveFunction(movementVector);
    }

    private void moveFunction(Vector2 mv)
    {
        if (!detected)
        {
            mv = mv * speed;
            cc_Enemy.velocity = mv;
        }
        else if (detected)
        {
            Vector2 direction = playerTransform.position - transform.position;
            direction.Normalize();
            mv = direction * (speed + 2);
            movementVector = mv;
            cc_Enemy.velocity = movementVector;
            
        }
        anim.SetFloat("dirX", mv.x);
        anim.SetFloat("dirY", mv.y);
    }

    #region
    #endregion

    #region Collision Method
    private void OnCollisionStay2D(Collision2D collision)
    {
        GameObject other = collision.collider.gameObject;
        if ((other.CompareTag("Wall") || (other.CompareTag("Chest")) || (other.CompareTag("Enemy"))) && !detected && !dead)
        {
            int directionCode = Random.Range(0, 4);
            if (directionCode == 0)
            {
                movementVector = northVector;
            }
            else if (directionCode == 1)
            {
                movementVector = southVector;
            }
            else if (directionCode == 2)
            {
                movementVector = eastVector;
            }
            else if (directionCode == 3)
            {
                movementVector = westVector;
            }
        }
        else if (other.CompareTag("Player") && !dead)
        {
            detected = true;
            
            if (attackTimer <= 0)
            {
                anim.SetTrigger("Attack");
                FindObjectOfType<AudioManager>().Play("Swipe");
                playerController.TakeDamage(damage);
                attackTimer = attackCoolDown;
            } else if (attackTimer > 0)
            {
                anim.ResetTrigger("Attack");
                attackTimer -= Time.deltaTime;
            }
        } else if (detected)
        {
            detected = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject other = collision.collider.gameObject;
        if (other.CompareTag("Enemy"))
        {
            Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
        }
    }
    private void OnTriggerEnter2D(Collider2D collider)
    {
        GameObject other = collider.gameObject;
        if (other.CompareTag("Bullet"))
        {
            anim.SetBool("Dead", true);
            FindObjectOfType<AudioManager>().Play("Death");
            Destroy(other);
            dead = true;
        }
    }
    #endregion
}
