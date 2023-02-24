using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollow : MonoBehaviour
{
    #region Editor Variables
    [SerializeField]
    [Tooltip("the player that the camera follows")]
    private Transform playerTransform;
    #endregion

    #region Cached Components
    private Camera cc_camera;
    #endregion
    void LateUpdate()
    {
        Vector3 newPos = playerTransform.position;
        newPos.z = transform.position.z;
        transform.position = newPos;
    }
}
