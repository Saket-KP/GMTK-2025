using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera_follow : MonoBehaviour
{

    public GameObject player;

    // Update is called once per frame
    void Update()
    {
        float yOffset = 2.0f; // adjust as needed
        transform.position = new Vector3(transform.position.x, player.transform.position.y + yOffset, transform.position.z);
    }
}
