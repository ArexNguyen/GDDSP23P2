using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
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

    [SerializeField]
    [Tooltip("Health of the boss")]
    private int Health;
    #endregion
    #region Cached Components
    private Rigidbody2D cc_Boss;
    #endregion

    #region private variables
    private Vector2 movementVector;
    private Animator anim;
    private bool detected;
    private bool dead;
    private float deadtimer;
    private float attackTimer;
    private int currHealth;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        attackTimer = 0;
        dead = false;
        deadtimer = timeToDie;
        anim = GetComponent<Animator>();
        currHealth = Health;
        cc_Boss = GetComponent<Rigidbody2D>();
        initVector.Normalize();
        movementVector = initVector;
        fieldOfView.SetOrigin(transform.position);
        fieldOfView.SetAimDirection(movementVector);
    }

    // Update is called once per frame
    void Update()
    {
        if (cc_Boss.velocity == Vector2.zero)
        {
            anim.SetBool("Moving", false);
        } else
        {
            anim.SetBool("Moving", true);
        }
        if (currHealth <= 0)
        {
            anim.SetBool("Dead", true);
            dead = true;
            FindObjectOfType<AudioManager>().Play("Death");
        }
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
        detected = false;
    }

    private void moveFunction(Vector2 mv)
    {
        if (!detected)
        {
            mv = mv * speed;
            cc_Boss.velocity = mv;
        }
        else if (detected)
        {
            Vector2 direction = playerTransform.position - transform.position;
            direction.Normalize();
            mv = direction * (speed + 5);
            movementVector = mv;
            cc_Boss.velocity = movementVector;

        }
        anim.SetFloat("dirX", mv.x);
        anim.SetFloat("dirY", mv.y);
    }

    #region Collision Method
    private void OnCollisionStay2D(Collision2D collision)
    {
        GameObject other = collision.collider.gameObject;
        if (other.CompareTag("Wall") && !detected && !dead)
        {
            movementVector = -movementVector;
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
            }
            else if (attackTimer > 0)
            {
                anim.ResetTrigger("Attack");
                attackTimer -= Time.deltaTime;
            }
        }
        else if (detected)
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
            currHealth -= 1;
            Destroy(other);
        }
    }
    #endregion
}
