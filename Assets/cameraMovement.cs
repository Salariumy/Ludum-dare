using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraMovement : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        transform.position= new Vector3(transform.position.x + Time.deltaTime * PlayerData.playerMoveSpeed, transform.position.y, transform.position.z);
    }
}
