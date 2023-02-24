using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineofSight : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D coll)
    {
        //check if coll is player
        if (coll.CompareTag("Player"))
        {
            GetComponentInParent<Enemy>().player = coll.transform;
            Debug.Log("I SEE YOU");
        }
    }
}
