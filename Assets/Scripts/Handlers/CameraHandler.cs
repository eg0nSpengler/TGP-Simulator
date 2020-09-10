using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;


public class CameraHandler : MonoBehaviour
{

    private Camera _cam;
    private UnityAction _onClickAction;

    private float _defaultFOV;

    [Header("Button References")]
    public Button FOVButton;

    private void Awake()
    {
        _defaultFOV = 60.0f;

        _onClickAction += ChangeFOV;

        if (GetComponent<Camera>() != null)
        {
            _cam = GetComponent<Camera>();
        }
        else
        {
            Debug.LogError("Failed to get Camera on " + gameObject.name.ToString());
        }

        if (!FOVButton)
        {
            Debug.LogWarning(gameObject.name.ToString() + " is missing a Button reference!");
        }

        FOVButton.onClick.AddListener(_onClickAction);
    }

    void ChangeFOV()
    {
        if (_cam.fieldOfView == 20.0f)
        {
            _cam.fieldOfView = _defaultFOV;
        }
        else
        {
            _cam.fieldOfView = 20.0f;
        }
    }
}
