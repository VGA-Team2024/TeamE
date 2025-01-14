using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingCall : MonoBehaviour
{
    GameObject _endingOne;
    GameObject _endingTwo;
    void Start()
    {
        _endingOne = transform.GetChild(0).gameObject;
        _endingTwo = transform.GetChild(1).gameObject;
        _endingOne.SetActive(false);
        _endingTwo.SetActive(false);
    }
    public void EndingOne()
    {
        _endingOne.SetActive(true);
    }

    public void EndingTwo()
    {
        _endingTwo.SetActive(true);
    }
}
