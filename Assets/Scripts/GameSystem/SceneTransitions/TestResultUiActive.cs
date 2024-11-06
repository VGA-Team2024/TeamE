using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class TestResultUiActive : MonoBehaviour
{
    private UiInputSystem _resultUiInputSystem;
    public GameObject ResultButton;
    // Start is called before the first frame update
    void Start()
    {
        _resultUiInputSystem = new UiInputSystem();
        _resultUiInputSystem.Enable();
    }

    // Update is called once per frame
    async void Update()
    {
        if (_resultUiInputSystem.UI.Submit.triggered)
        {
            await Task.Delay(1000);
            ResultButton.SetActive(true);
        }
    }

    void OnDisable()
    {
        _resultUiInputSystem.Disable();
    }
}
