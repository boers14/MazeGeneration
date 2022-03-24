using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float walkSpeed = 5, sprintSpeed = 7.5f, slowDownSpeed = 0.85f, speedUp = 0.25f, sprintTime = 5f,
        reloadSprintTimeComparedToSprintTime = 0.5f, turnOffStaminaUITime = 1f;

    private Image staminaFill = null;

    private GameObject staminaUI = null;

    private Rigidbody rb = null;

    private Vector3 moveDirection = Vector3.zero;

    private float maxMoveSpeed = 0, moveSpeedVertical = 0, moveSpeedHorizontal = 0, currentSprintTime = 0, 
        currentTurnOffStaminaUITime = 0, staminaFillEndPos = 0;

    // Set ui elements/ movement vars
    private void Start()
    {
        staminaFill = PlayerUIHandler.instance.staminaFill;
        staminaUI = PlayerUIHandler.instance.completeStaminaUI;
        currentTurnOffStaminaUITime = turnOffStaminaUITime;

        rb = GetComponent<Rigidbody>();
        maxMoveSpeed = walkSpeed;
        currentSprintTime = sprintTime;

        StartCoroutine(SetStaminaFillStartPos());
    }

    // Calculate end pos of stamina bar and turn it off
    private IEnumerator SetStaminaFillStartPos()
    {
        yield return new WaitForEndOfFrame();
        staminaFillEndPos = staminaFill.rectTransform.localPosition.x - staminaFill.rectTransform.sizeDelta.x;
        staminaUI.SetActive(false);
    }

    // Handle movement of player
    private void Update()
    {
        moveDirection = Vector3.zero;

        // Set player max speed to sprint speed if the player has stamina left
        if (Input.GetKey(KeyCode.LeftShift) && currentSprintTime > 0)
        {
            maxMoveSpeed = sprintSpeed;
            currentSprintTime -= Time.deltaTime;

            // Turn on stamina bar if just started sprinting
            if (currentTurnOffStaminaUITime >= turnOffStaminaUITime)
            {
                currentTurnOffStaminaUITime = 0;
                staminaUI.SetActive(true);
            }
        }
        else
        {
            // Normal move speed
            maxMoveSpeed = walkSpeed;
            if (!Input.GetKey(KeyCode.LeftShift))
            {
                // Regain stamina
                currentSprintTime += Time.deltaTime * reloadSprintTimeComparedToSprintTime;
            }
        }

        currentSprintTime = Mathf.Clamp(currentSprintTime, 0, sprintTime);

        // If stamina UI is active move the bar based on the percentage of stamina the player has
        if (staminaUI.activeSelf)
        {
            float sprintPercentage = currentSprintTime / sprintTime;
            Vector3 staminaFillPos = staminaFill.rectTransform.localPosition;
            staminaFillPos.x = staminaFillEndPos + (staminaFill.rectTransform.sizeDelta.x * sprintPercentage);
            staminaFill.rectTransform.localPosition = staminaFillPos;

            // Start counting to turn of the stamina bar with full stamina and sprint button not pressed
            if (!Input.GetKey(KeyCode.LeftShift) && sprintPercentage >= 1)
            {
                currentTurnOffStaminaUITime += Time.deltaTime;
                if (currentTurnOffStaminaUITime >= turnOffStaminaUITime)
                {
                    staminaUI.SetActive(false);
                }
            }
            else
            {
                currentTurnOffStaminaUITime = 0;
            }
        }

        // Calculate the player forward/ back speed
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

        // Calculate the player left/ right speed
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

        // Set movement
        moveDirection.y = rb.velocity.y;
        rb.velocity = moveDirection;
    }

    // Calculate the movespeed and set direction
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
