using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentParticles : MonoBehaviour
{
    private Transform player = null;

    private Vector3 playerOffset = Vector3.zero;

    // Find player and set offset
    private void Start()
    {
        player = FindObjectOfType<PlayerMovement>().transform;
        playerOffset = new Vector3(0, -player.localScale.y - 0.03f, 0);
    }

    // Set particles to player pos
    private void FixedUpdate()
    {
        transform.position = player.position + playerOffset;
    }
}
