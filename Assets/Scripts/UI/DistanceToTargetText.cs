using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DistanceToTargetText : MonoBehaviour
{
    [Header("References")]
    public Transform PodTransform;

    private TextMeshProUGUI _text;

    private void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();

        if (!_text)
        {
            Debug.LogError("Failed to get TextMeshPro Text on " + gameObject.name.ToString());
        }
        else
        {
            _text.text = " ";
        }

        if (!PodTransform)
        {
            Debug.LogWarning(gameObject.name.ToString() + " is missing a reference!");
        }

        PodTargetDesignatorBox.OnTargetTrack += GetTargetDistance;
    }

    private void OnDisable()
    {
        PodTargetDesignatorBox.OnTargetTrack -= GetTargetDistance;
        StopAllCoroutines();
    }


    private void Update()
    {
        GetTargetDistance();
    }

    void GetTargetDistance()
    {
        var tgt = PodTargetDesignatorBox.TargetPosition;

        var dist = Math.Round(Vector3.Distance(PodTransform.position, tgt), 1);
        _text.text = dist.ToString();

    }
}
