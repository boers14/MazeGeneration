using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100;

    private GameObject completeInGameUI = null;

    private Image greenHealth = null, animatedHealth = null;

    [SerializeField]
    private Color32 healthDetractColor = Color.red, healthAddedColor = Color.yellow;

    private float healthBarEndPos = 0, diffInHealthBarPos = 0, healthPercentage = 100;

    private float currentHealth = 0;

    private Vector3 newHealthBarPos = Vector3.zero;

    private void Start()
    {
        greenHealth = PlayerUIHandler.instance.greenHealth;
        animatedHealth = PlayerUIHandler.instance.animatedHealth;
        completeInGameUI = PlayerUIHandler.instance.completeInGameUI;

        currentHealth = maxHealth;
        completeInGameUI.SetActive(true);
        StartCoroutine(SetHealthBarPosStats());
    }

    private IEnumerator SetHealthBarPosStats()
    {
        yield return new WaitForEndOfFrame();
        greenHealth.rectTransform.sizeDelta = greenHealth.transform.parent.GetComponent<RectTransform>().sizeDelta;
        animatedHealth.rectTransform.sizeDelta = greenHealth.transform.parent.GetComponent<RectTransform>().sizeDelta;

        healthBarEndPos = greenHealth.rectTransform.localPosition.x - greenHealth.rectTransform.sizeDelta.x;
        diffInHealthBarPos = greenHealth.rectTransform.sizeDelta.x;
    }

    public void ChangeHealth(float healthChange)
    {
        currentHealth += healthChange;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        healthPercentage = currentHealth / maxHealth;
        float healthBarXPos = healthBarEndPos + (diffInHealthBarPos * healthPercentage);
        newHealthBarPos = greenHealth.rectTransform.localPosition;

        if (healthChange < 0)
        {
            Vector3 newPos = GetNewPos(healthBarXPos, healthDetractColor);
            iTween.MoveTo(animatedHealth.gameObject, iTween.Hash("position", newPos, "time", healthChange / -5, "easetype",
                iTween.EaseType.easeOutBack));

            greenHealth.rectTransform.localPosition = newHealthBarPos;
            SetHealthColor();
        } else
        {
            Vector3 newPos = GetNewPos(healthBarXPos, healthAddedColor);
            iTween.MoveTo(animatedHealth.gameObject, iTween.Hash("position", newPos, "time", healthChange / 10, "easetype",
                iTween.EaseType.easeInSine, "oncomplete", "SetHealthBarPos", "oncompletetarget", gameObject));
        }
    }

    private Vector3 GetNewPos(float healthBarXPos, Color32 animatedHealthColor)
    {
        animatedHealth.color = animatedHealthColor;
        iTween.Stop(animatedHealth.gameObject);
        newHealthBarPos.x = healthBarXPos;

        animatedHealth.transform.localPosition = greenHealth.transform.localPosition;
        Vector3 newPos = animatedHealth.transform.position;
        newPos.x += healthBarXPos - greenHealth.rectTransform.localPosition.x;
        return newPos;
    }

    private void SetHealthBarPos()
    {
        greenHealth.rectTransform.localPosition = newHealthBarPos;
        SetHealthColor();
    }

    private void SetHealthColor()
    {
        float greenValue = 0;
        float redValue = 255;

        if (healthPercentage <= 0.5f)
        {
            greenValue = 255 * (healthPercentage * 2);
        }
        else
        {
            redValue = 255 * ((1 - healthPercentage) * 2);
            greenValue = 255;
        }

        greenHealth.color = new Color32((byte)redValue, (byte)greenValue, 0, 255);
    }
}
