using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchActiveStateButton : SetUIStats
{
    [SerializeField]
    private List<GameObject> objectsToDisable = new List<GameObject>(), objectsToSwitchActiveState = new List<GameObject>();

    public override void Awake()
    {
        base.Awake();
        GetComponent<Button>().onClick.AddListener(SwitchActiveState);
    }

    // Switches active states of objects when the button is clicked
    private void SwitchActiveState()
    {
        for (int i = 0; i < objectsToDisable.Count; i++)
        {
            objectsToDisable[i].SetActive(false);
        }

        for (int i = 0; i < objectsToSwitchActiveState.Count; i++)
        {
            objectsToSwitchActiveState[i].SetActive(!objectsToSwitchActiveState[i].activeSelf);
        }
    }
}
