using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour
{
    #region Movement_vars
    public float movespeed;
    float x_input;
    float y_input;
    #endregion

    #region Attack_vars
    public float dmg;
    public float atkSpeed = 1;
    float atkTimer = 1;
    public float hitboxTime;
    public float endanimTime;
    bool isAttacking;
    Vector2 currDirection;
    public float AmmoCost;
    private float bulletSpeed = 540;
    [SerializeField]
    [Tooltip("The text component that is displaying the ammo")]
    private TMP_Text ammoCounter;
    #endregion

    #region Health_vars
    public float maxHealth;
    float currHealth;
    public float maxAmmo;
    public float initialAmmo;
    float currAmmo;
    public GameObject laserPrefab;
    [SerializeField]
    [Tooltip("The text component that is displaying the health")]
    private TMP_Text healthCounter;
    #endregion

    #region Animation_comps
    Animator anim;

    #endregion


    #region Physics_components
    Rigidbody2D PlayerRB;
    #endregion

    #region Unity_functions
    private void Awake()
    {
        PlayerRB = GetComponent<Rigidbody2D>();
        atkTimer = 0;
        anim = GetComponent<Animator>();
        currHealth = maxHealth;
        currAmmo = initialAmmo;

        ammoCounter.text = "x" + currAmmo;
        healthCounter.text = "x" + currHealth;
    }

    private void Update()
    {
        if (isAttacking)
        {
            return;
        }
        x_input = Input.GetAxisRaw("Horizontal");
        y_input = Input.GetAxisRaw("Vertical");

        Move();
        if (Input.GetKeyDown(KeyCode.K) && (atkTimer <= 0) && (currAmmo >= AmmoCost))
            {
                AttackRanged();
            }
            else
        {
            atkTimer -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            Interact();
        }

    }
    #endregion

    #region Movement_funcs
    private void Move()
    {
        anim.SetBool("Moving", true);
        if (x_input > 0)
        {
            PlayerRB.velocity = Vector2.right * movespeed;
            currDirection = Vector2.right;

        } else if (x_input < 0)
        {
            PlayerRB.velocity = Vector2.left * movespeed;
            currDirection = Vector2.left;

        }

        else if (y_input > 0)
        {
            PlayerRB.velocity = Vector2.up * movespeed;
            currDirection = Vector2.up;

        }
        else if (y_input < 0)
        {
            PlayerRB.velocity = Vector2.down * movespeed;
            currDirection = Vector2.down;

        } 
        else
        {
            PlayerRB.velocity = Vector2.zero;
            anim.SetBool("Moving", false);
        }
        anim.SetFloat("DirX", currDirection.x);
        anim.SetFloat("DirY", currDirection.y);
    }
    #endregion

    #region Attack_funcs
    private void AttackRanged()
    {
        Debug.Log(currDirection);
        if (currAmmo >= AmmoCost)
        {
            atkTimer = atkSpeed;
            TakeAmmo(AmmoCost);
            //attack coroutine
            anim.SetTrigger("Attack");
            StartCoroutine("AttackRangedRoutine");
            
        }
    }

    IEnumerator AttackRangedRoutine()
    {
        //anim
        FindObjectOfType<AudioManager>().Play("Swipe");


        isAttacking = true;
        PlayerRB.velocity = Vector2.zero;

        yield return new WaitForSeconds(hitboxTime);
        Debug.Log("Shooting now");

        GameObject bullet = Instantiate(laserPrefab, transform.position, transform.rotation);
        bullet.GetComponent<Rigidbody2D>().AddForce((Vector3) currDirection * bulletSpeed);

        yield return new WaitForSeconds(hitboxTime);
        isAttacking = false;
        yield return null;
    }
    #endregion

    #region Health_funcs

    public void TakeDamage(float value)
    {
        //decrement hp
        currHealth -= value;
        currHealth = Mathf.Max(currHealth, 0);
        Debug.Log("Health is now: " + currHealth.ToString());

        //change UI
        healthCounter.text = "x" + currHealth;
        FindObjectOfType<AudioManager>().Play("Oof");

        //check ded
        if (currHealth <= 0)
        {
            //Die
            Die();

        }
    }

    public void Heal(float value)
    {
        //increment health by value
        currHealth += value;
        currHealth = Mathf.Min(currHealth, maxHealth);
        Debug.Log("Health is now: " + currHealth.ToString());

        //change UI
        healthCounter.text = "x" + currHealth;
    }

    public void TakeAmmo(float value)
    {
        //decrement hp
        currAmmo -= value;
        Debug.Log("Player Ammo is now: " + currAmmo.ToString());
        Debug.Log("Ammo is now: " + currAmmo.ToString());
        //change UI
        // CHANGE FROM SLIDER TO TEXT COUNTER
        ammoCounter.text = "x" + currAmmo;

        //sound
    }

    public void HealAmmo(float value)
    {
        //increment health by value
        currAmmo += value;
        currAmmo = Mathf.Min(currAmmo, maxAmmo);
        Debug.Log("Ammo is now: " + currAmmo.ToString());

        //change UI
        ammoCounter.text = "x" + currAmmo;
    }

    //destroy player and trigger end scene
    private void Die()
    {
        //Destroy this object
        FindObjectOfType<AudioManager>().Play("Death");
        this.gameObject.SetActive(false);

        //trigger anything to end the game, find GameManager
        GameObject gm = GameObject.FindWithTag("GameController");
        gm.GetComponent<GameManager>().LoseGame();
    }
    #endregion

    #region Interact_funcs

    private void Interact()
    {
        Debug.Log("Casting hitbox now");
        RaycastHit2D[] hits = Physics2D.BoxCastAll(PlayerRB.position + currDirection, Vector2.one, 0f, Vector2.zero);

        foreach (RaycastHit2D hit in hits)
        {
            Debug.Log(hit.transform.name);
            if (hit.transform.CompareTag("Chest") == true)
            {
                Debug.Log("Tons of loot");
                hit.transform.GetComponent<Chest>().Interact();
            }
        }
    }
    #endregion
}
