using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;

public class FOVClickHandler : MonoBehaviour
{
    private Button _button;
    private TextMeshProUGUI _text;
    private UnityAction _onClickAction;

    private string _naroString;
    private string _wideString;

    void Awake()
    {
        _naroString = "NARO";
        _wideString = "WIDE";

        _onClickAction += SwitchString;

        if (GetComponent<Button>() != null)
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(_onClickAction);
        }
        else
        {
            Debug.LogError("Failed to get Button on " + gameObject.name.ToString());
        }

        if (GetComponent<TextMeshProUGUI>() != null)
        {
            _text = GetComponent<TextMeshProUGUI>();
        }
        else
        {
            Debug.LogError("Failed to get TextMeshPro Text on " + gameObject.name.ToString());
        }
    }

    void SwitchString()
    {
        var currString = _text.text;

        if (currString == _wideString)
        {
            _text.text = _naroString;
        }
        else
        {
            _text.text = _wideString;
        }
    }
}
