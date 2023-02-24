using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    [Tooltip("the player Object")]
    private PlayerController playerController;

    [SerializeField]
    [Tooltip("number of shurikens in the chest")]
    private int numOfBullet;

    [SerializeField]
    [Tooltip("Time to open the chest")]
    private float openingTime;

    private float openTimer;
    private bool opened;
    private Animator anim;

    void Start()
    {
        openTimer = openingTime;
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (opened)
        {
            openTimer -= Time.deltaTime;
        }
        if (openTimer <= 0)
        {
            anim.SetBool("HitPlayer", false);
            anim.SetBool("Opened", true);
        }
    }

    // Update is called once per frame
    private void OnCollisionStay2D(Collision2D collision)
    {
        GameObject other = collision.collider.gameObject;
        if (other.CompareTag("Player") && !opened)
        {
            anim.SetBool("HitPlayer", true);
            playerController.HealAmmo(numOfBullet);
            opened = true;
        }
    }
}
