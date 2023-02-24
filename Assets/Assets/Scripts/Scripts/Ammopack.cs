using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammopack : MonoBehaviour
{
    #region Healthpack_vars
    [SerializeField]
    [Tooltip("The amount the player heals")]
    private int healamount;
    #endregion

    #region Heal_funcs
    private void OnTriggerEnter2D(Collider2D collision)
    { 
        if (collision.transform.CompareTag("Player"))
        {
            collision.transform.GetComponent<PlayerController>().HealAmmo(healamount);
            Destroy(this.gameObject);
        }
    }
    #endregion
}
