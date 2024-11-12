using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleUI : MonoBehaviour
{
    public GameObject StartButton;

    public void StartButtonOnPressed()
    {
        StartButton.SetActive(false);
    }
}
