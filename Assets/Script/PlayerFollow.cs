using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollow : MonoBehaviour
{
    public Transform player;
    Vector3 offset;
    

    // Update is called once pr frame
    void Start()
    {
        offset = new Vector3(0.5f,0,0);
    }
    void LateUpdate()
    {
        transform.position = player.transform.position;
        
        transform.parent = player.transform;
        transform.LookAt(player.transform);
    }
}
