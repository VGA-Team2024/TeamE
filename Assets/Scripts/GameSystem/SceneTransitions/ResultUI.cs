using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultUI : MonoBehaviour
{
    public GameObject ResultButton;

    public void ResultButtonOnPressed()
    {
        SceneLoader.LoadScene("Title");
    }
}
