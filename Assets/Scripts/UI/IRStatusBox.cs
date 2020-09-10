using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class IRStatusBox : MonoBehaviour
{
    private TextMeshProUGUI _irText;

    private void Awake()
    {
        _irText = GetComponent<TextMeshProUGUI>();

        if (!_irText)
        {
            Debug.LogError("Failed to get TextMeshPro Text on " + gameObject.name.ToString());
        }
        else
        {
            _irText.text = " ";
            PodTargetDesignatorBox.OnTrackingUpdate += GetTrackMode;
        }

    }

    private void OnDisable()
    {
        PodTargetDesignatorBox.OnTrackingUpdate -= GetTrackMode;
    }

    void GetTrackMode()
    {
        var trackState = PodTargetDesignatorBox.TrackingState;
        switch (trackState)
        {
            case PodTargetDesignatorBox.TRACK_STATE.NONE:
                _irText.text = " ";
                break;
            case PodTargetDesignatorBox.TRACK_STATE.AREA:
                _irText.text = "IR AREA";
                break;
            case PodTargetDesignatorBox.TRACK_STATE.POINT:
                _irText.text = "IR POINT";
                    break;
            case PodTargetDesignatorBox.TRACK_STATE.RATES:
                _irText.text = "IR INR";
                break;

        }
        
    }
}
