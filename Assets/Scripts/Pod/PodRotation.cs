using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PodRotation : MonoBehaviour
{
    private Camera _camera;
    private Vector3 _currentRot;

    public delegate void GimbalLimitReached();

    /// <summary>
    /// Called when the Targeting Pod reaches a gimbal limit
    /// </summary>
    public static event GimbalLimitReached OnGimbalLimitReached;

    /// <summary>
    /// The rotation range of the Camera on the Y axis
    /// </summary>
    [Header("Rotation Values")]
    [Range(-180.0f, 180.0f)]
    public float YRotationRange;

    /// <summary>
    /// The rotation range of the Camera on the X axis
    /// </summary>
    [Range(-75.0f, 75.0f)]
    public float XRotationRange;

    /// <summary>
    /// Is the Pod currently rotating? (Tracking a target or slewing)
    /// </summary>
    public static bool IsPodRotating { get; private set; }

    private void Awake()
    {
        _camera = GetComponent<Camera>();

        _currentRot = transform.rotation.eulerAngles;
        YRotationRange = 0.0f;
        XRotationRange = 0.0f;
        IsPodRotating = false;

        if (!_camera)
        {
            Debug.LogError("Failed to get Camera on " + gameObject.name.ToString());
        }

        PodTargetDesignatorBox.OnTrackingUpdate += GetTrackMode;
        OnGimbalLimitReached += StopTracking;
    }


    private void OnDisable()
    {
        PodTargetDesignatorBox.OnTrackingUpdate -= GetTrackMode;
        OnGimbalLimitReached -= StopTracking;
        StopAllCoroutines();
    }

    private void Update()
    {
        _currentRot = transform.rotation.eulerAngles;
        HandleKeys();
        
    }

    void HandleKeys()
    {
        
        if (Input.GetKey(KeyCode.D))
        {
            
                if (YRotationRange < 180.0f)
                {
                    YRotationRange += 1f;
                    transform.rotation = Quaternion.Euler(_currentRot.x, YRotationRange, _currentRot.z);
                }
                else
                {
                    transform.rotation.eulerAngles.Set(transform.rotation.eulerAngles.x, 180.0f, transform.rotation.eulerAngles.z);
                    Debug.LogWarning("GIMBAL LIMIT!");
                    OnGimbalLimitReached?.Invoke();
                }
                
        }

        if (Input.GetKey(KeyCode.A))
        {
            
                if (YRotationRange > -180.0f)
                {
                    YRotationRange -= 1f;
                    transform.rotation = Quaternion.Euler(_currentRot.x, YRotationRange, _currentRot.z);
                }
                else
                {
                    transform.rotation.eulerAngles.Set(transform.rotation.eulerAngles.x, -180.0f, transform.rotation.eulerAngles.z);
                    Debug.LogWarning("GIMBAL LIMIT!");
                    OnGimbalLimitReached?.Invoke();
                }
          
        }

        if (Input.GetKey(KeyCode.W))
        {
            if (XRotationRange > -75.0f)
            {
                XRotationRange -= 1f;
                transform.rotation = Quaternion.Euler(XRotationRange, _currentRot.y, _currentRot.z);
            }
            else
            {
                transform.rotation.eulerAngles.Set(-75.0f, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
                Debug.LogWarning("GIMBAL LIMIT!");
                OnGimbalLimitReached?.Invoke();
            }
        }

        if (Input.GetKey(KeyCode.S))
        {
            if (XRotationRange < 75.0f)
            {
                XRotationRange += 1f;
                transform.rotation = Quaternion.Euler(XRotationRange, _currentRot.y, _currentRot.z);
            }
            else
            {
                transform.rotation.eulerAngles.Set(75.0f, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
                Debug.LogWarning("GIMBAL LIMIT!");
                OnGimbalLimitReached?.Invoke();
            }
        }
    }

    void GetTrackMode()
    {
        var trackState = PodTargetDesignatorBox.TrackingState;
        switch (trackState)
        {
            case PodTargetDesignatorBox.TRACK_STATE.NONE:
                StopCoroutine(MaintainLock());
                break;
            case PodTargetDesignatorBox.TRACK_STATE.SLEWING:
                break;
            case PodTargetDesignatorBox.TRACK_STATE.AREA:
                StartCoroutine(MaintainLock());
                break;
            case PodTargetDesignatorBox.TRACK_STATE.POINT:
                StartCoroutine(MaintainLock());
                break;
            case PodTargetDesignatorBox.TRACK_STATE.RATES:
                break;
            default:
                break;

        }

    }

    void StopTracking()
    {
        StopCoroutine(MaintainLock());
    }

    IEnumerator MaintainLock()
    {
        while (PodTargetDesignatorBox.TrackingState == PodTargetDesignatorBox.TRACK_STATE.POINT || PodTargetDesignatorBox.TrackingState == PodTargetDesignatorBox.TRACK_STATE.AREA)
        {
           transform.LookAt(PodTargetDesignatorBox.TargetPosition);
           yield return null;
        }
    }

}
