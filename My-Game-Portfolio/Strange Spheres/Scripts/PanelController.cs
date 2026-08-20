using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelController : MonoBehaviour
{
    public GameObject panel;
    private bool isPanelOpen = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPanelOpen)
            {
                ClosePanel();
            }
            else
            {
                OpenPanel();
            }
        }
    }

    void OpenPanel()
    {
        panel.SetActive(true);
        isPanelOpen = true;
    }

    void ClosePanel()
    {
        panel.SetActive(false);
        isPanelOpen = false;
    }
}