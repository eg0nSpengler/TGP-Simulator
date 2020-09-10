using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PodTargetDesignatorBox : MonoBehaviour
{
    public delegate void TrackModeUpdate();
    public delegate void TargetTrack();

    /// <summary>
    /// The tracking state of the Pod
    /// </summary>
    public enum TRACK_STATE
    {
        NONE,
        /// <summary>
        /// The pod is being slewed (when the slewing ends it will return to the last tracking state)
        /// </summary>
        SLEWING,
        /// <summary>
        /// Area tracking (large structure, fixed point in space, etc)
        /// </summary>
        AREA,
        /// <summary>
        /// Point tracking (Individual well-defined structure, vehicle, etc)
        /// </summary>
        POINT,
        /// <summary>
        /// RATES tracking
        /// <para>This is done when the point the pod is looking at has been obscured/mask from its view</para>
        /// <para>The pod will remain in AREA track mode at the LAST aspect in which it had visual of the point/target</para>
        /// <para>This condition is usually reached by the host aircraft's external stores/tanks obscuring LoS</para>
        /// </summary>
        RATES
    };

    /// <summary>
    /// Called when the Targeting Pod has updated it's tracking mode
    /// </summary>
    public static event TrackModeUpdate OnTrackingUpdate;


    /// <summary>
    /// Called when the Targeting Pod is tracking a target
    /// <para> Note - The only time the pod will <b>NOT</b> be considered to be tracking is if the user is slewing the pod</para>
    /// </summary>
    public static event TargetTrack OnTargetTrack;

    /// <summary>
    /// The position of our currently locked target
    /// </summary>
    public static Vector3 TargetPosition { get; private set; }
    /// <summary>
    /// The current tracking state of the Pod
    /// </summary>
    public static TRACK_STATE TrackingState { get; private set; }

    private static TRACK_STATE PreviousTrackingState { get; set; }

    [Header("References")]
    public Image TargetDesignatorFrustrum;
    public SphereCollider TargetDesignatorSphere;
    public Camera PodCamera;

    /// <summary>
    /// The transform of the currently tracked target
    /// <para> This is used primarily for POINT track mode</para>
    /// </summary>
    public Transform _trackTransform;

    /// <summary>
    /// The amount of time the user must hold TMS UP to initiate a AREA track
    /// </summary>
    private float _areaTrackTime;

    /// <summary>
    /// The amount of seconds the user has held down a TMS key;
    /// </summary>
    private float _tmsHoldTime;

    private void Awake()
    {

        if (!TargetDesignatorFrustrum || !TargetDesignatorSphere|| !PodCamera)
        {
            Debug.LogWarning(gameObject.name.ToString() + "is missing a reference!");
        }

        TrackingState = TRACK_STATE.NONE;
        PreviousTrackingState = TRACK_STATE.NONE;

        _areaTrackTime = 1.0f;
        _tmsHoldTime = 0.0f;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CheckForTrackingUpdate());
    }
    

    // Update is called once per frame
    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Keypad8))
        {
           AreaTrackTarget();   
        }

        if (Input.GetKeyUp(KeyCode.Keypad8))
        {
            PointTrackTarget();
        }

        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            CancelTrack();
        }
    }

    void UpdateTrackingState(TRACK_STATE state)
    {
        PreviousTrackingState = TrackingState;
        TrackingState = state;
    }

    void PointTrackTarget()
    {
        Ray ray = new Ray(PodCamera.transform.position, PodCamera.transform.forward);
        RaycastHit hit;
        Debug.DrawRay(PodCamera.transform.position, PodCamera.transform.forward, Color.green);

        
        if (Physics.Raycast(ray, out hit) == true)
        {
            Debug.Log(hit.transform.name);
            TargetDesignatorSphere.transform.position = hit.point;
            TargetDesignatorSphere.radius = hit.collider.bounds.size.magnitude;
            TargetPosition = hit.transform.position;
            _trackTransform = hit.transform;
            UpdateTrackingState(TRACK_STATE.POINT);
            StartCoroutine(UpdatePointTrack());
            OnTargetTrack?.Invoke();
            
        }
        else
        {
            Debug.LogWarning("POINT track failed due to no defined object in view frustrum!");
            AreaTrackTarget();
        }
    }

    void AreaTrackTarget()
    {
        UpdateTrackingState(TRACK_STATE.AREA);
        Ray ray = new Ray(PodCamera.transform.position, PodCamera.transform.forward);
        RaycastHit hit;
        Debug.DrawRay(PodCamera.transform.position, PodCamera.transform.forward, Color.green);


            if (Physics.Raycast(ray, out hit) == true)
            {
                TargetPosition = hit.point;
                TrackingState = TRACK_STATE.AREA;
            }
            else
            {
                TargetPosition = ray.GetPoint(5.0f);
            }
        OnTargetTrack?.Invoke();
        
    }

    void CancelTrack()
    {
        UpdateTrackingState(TRACK_STATE.NONE);
        TargetPosition = new Vector3(0.0f, 0.0f, 0.0f);
        
    }

    IEnumerator UpdatePointTrack()
    {
        
        while (TrackingState != TRACK_STATE.POINT)
        {
            yield return null;
        }

        while (TrackingState == TRACK_STATE.POINT)
        {
            TargetPosition = _trackTransform.position;

            Ray ray = new Ray(PodCamera.transform.position, PodCamera.transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit) == true)
            {
                if (hit.transform.position != TargetPosition)
                {
                    
                }
            }
            yield return null;    
        }
    }

    IEnumerator UpdateRatesTrack()
    {
        while(TrackingState == TRACK_STATE.RATES)
        {
            Ray ray = new Ray(PodCamera.transform.position, PodCamera.transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit) == true)
            {
                if (hit.transform.position == TargetPosition)
                {
                    PointTrackTarget();
                    yield return null;
                }
            }
        }
        
    }

    IEnumerator CheckForTrackingUpdate()
    {
        yield return new WaitUntil(() => PreviousTrackingState != TrackingState);
        OnTrackingUpdate?.Invoke();
        Debug.Log("Invoking OnTrackingUpdate!");
        StartCoroutine(CheckForTrackingUpdate());
    }
}
