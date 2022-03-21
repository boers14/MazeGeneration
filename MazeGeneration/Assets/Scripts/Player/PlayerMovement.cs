using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float walkSpeed = 5, sprintSpeed = 7.5f, slowDownSpeed = 0.85f, speedUp = 0.25f;

    private Rigidbody rb = null;

    private Vector3 moveDirection = Vector3.zero;

    private float maxMoveSpeed = 0, moveSpeedVertical = 0, moveSpeedHorizontal = 0;

    [System.NonSerialized]
    public float health = 100;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        maxMoveSpeed = walkSpeed;
    }

    void Update()
    {
        moveDirection = Vector3.zero;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            maxMoveSpeed = sprintSpeed;
        }
        else
        {
            maxMoveSpeed = walkSpeed;
        }

        if (Input.GetKey(KeyCode.W))
        {
            moveSpeedVertical = MovePlayer(moveSpeedVertical > maxMoveSpeed, maxMoveSpeed, speedUp, moveSpeedVertical, 
                transform.forward);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            moveSpeedVertical = MovePlayer(moveSpeedVertical < -maxMoveSpeed, -maxMoveSpeed, -speedUp, moveSpeedVertical, 
                transform.forward);
        }
        else
        {
            if (moveSpeedVertical > -0.05f && moveSpeedVertical < 0.05f)
            {
                moveSpeedVertical = 0;
            }
            else
            {
                moveSpeedVertical *= slowDownSpeed;
            }
        }

        if (Input.GetKey(KeyCode.D))
        {
            moveSpeedHorizontal = MovePlayer(moveSpeedHorizontal > maxMoveSpeed, maxMoveSpeed, speedUp, moveSpeedHorizontal,
                transform.right);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            moveSpeedHorizontal = MovePlayer(moveSpeedHorizontal < -maxMoveSpeed, -maxMoveSpeed, -speedUp, moveSpeedHorizontal,
                transform.right);
        }
        else
        {
            if (moveSpeedHorizontal > -0.05f && moveSpeedHorizontal < 0.05f)
            {
                moveSpeedHorizontal = 0;
            }
            else
            {
                moveSpeedHorizontal *= slowDownSpeed;
            }
        }

        moveDirection.y = rb.velocity.y;
        rb.velocity = moveDirection;
    }

    private float MovePlayer(bool clamp, float maxSpeed, float speedUp, float moveSpeed, Vector3 addedDirection)
    {
        moveSpeed += speedUp;

        if (clamp)
        {
            moveSpeed = maxSpeed;
        }

        addedDirection *= moveSpeed;
        moveDirection += addedDirection;
        return moveSpeed;
    }
}
