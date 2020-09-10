using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class AudioSourceHandler : MonoBehaviour
{
    private AudioSource _audioSource;
    private UnityAction _onClickAction;

    private void Awake()
    {
        if (GetComponent<AudioSource>() != null)
        {
            _audioSource = GetComponent<AudioSource>();
        }
        else
        {
            Debug.LogError("Failed to get Audio Source on " + gameObject.name.ToString());
        }

        _onClickAction += PlayClickSound;    
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (var butt in FindObjectsOfType<Button>())
        {
            butt.onClick.AddListener(_onClickAction);
        }
    }

    void PlayClickSound()
    {
        _audioSource.Play();
    }
}
