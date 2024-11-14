using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor : MonoBehaviour
{
    public enum CursorMode
    {
        Point,
        Snap,
        DPad
    }

    [SerializeField] private CursorMode cursorMode = CursorMode.Point;

    private Camera mainCam;
    private List<Collider2D> results = new();
    private GameObject previousDetectedKey;
    private GameObject[] keys;

    private Vector3 previousMousePosition;
    [SerializeField] private float decelerationThreshold = 0.1f;
    [SerializeField] private float minDistanceToSnap = 0.7f;
    [SerializeField] private float snapCooldownTime = 0.5f;
    private float snapCooldownTimer = 0f;
    private bool isStopped = false;
    private bool isSnapping = false; // New variable to track if snapping is active

    void Start()
    {
        mainCam = Camera.main;
        previousMousePosition = Input.mousePosition;
    }

    void Update()
    {
        keys = GameObject.FindGameObjectsWithTag("Key");

        // Update cursor position based on mouse position only if not snapping
        if (!isSnapping)
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z += 9f;
            mousePosition.x = Mathf.Clamp(mousePosition.x, 0f, Screen.width);
            mousePosition.y = Mathf.Clamp(mousePosition.y, 0f, Screen.height);
            transform.position = mainCam.ScreenToWorldPoint(mousePosition);
        }

        float distance = 9999f;
        GameObject closestKey = null;

        if (cursorMode == CursorMode.DPad) {
            Debug.Log("DPad mode is not implemented.");
        } 
        else {
            foreach (GameObject currentKey in keys) {
                float distanceToKey = Vector2.Distance(currentKey.transform.position, transform.position);
                if (distanceToKey < distance) {
                    distance = distanceToKey;
                    closestKey = currentKey;
                }
            }

            if (isStopped)
            {
                snapCooldownTimer += Time.deltaTime;
                if (snapCooldownTimer >= snapCooldownTime)
                {
                    isStopped = false;
                    Debug.Log("Snap cooldown ended, snapping can resume.");
                }
            }

            if (cursorMode == CursorMode.Snap && closestKey != null && !isStopped)
            {
                Debug.Log("Attempting to snap. Distance: " + distance + ", Deceleration: " + IsDecelerating());
                if (distance <= minDistanceToSnap && IsDecelerating())
                {
                    SnapToKey(closestKey);
                }
                else
                {
                    Debug.Log("Conditions not met for snapping.");
                }
            }

            if (cursorMode == CursorMode.Point || cursorMode != CursorMode.Snap || distance <= minDistanceToSnap || !IsDecelerating())
            {
                if (closestKey && previousDetectedKey != closestKey)
                {
                    if (previousDetectedKey != null)
                    {
                        UnHoverPreviousKey();
                    }
                    HoverKey(closestKey.GetComponent<Collider2D>());
                    previousDetectedKey = closestKey;
                }

                if (Input.GetMouseButtonDown(0) && closestKey != null)
                {
                    SelectKey(closestKey.GetComponent<Collider2D>());
                }
                else if (Input.GetMouseButtonUp(0) && closestKey != null)
                {
                    DeSelectKey(closestKey);
                }
            }
        }

        previousMousePosition = Input.mousePosition;
    }

    private bool IsDecelerating()
    {
        float speed = (Input.mousePosition - previousMousePosition).magnitude / Time.deltaTime;

        if (speed < Mathf.Epsilon)
        {
            snapCooldownTimer = 0f;
            isStopped = true;
            isSnapping = true;
            Debug.Log("Cursor stopped, starting cooldown.");
            return false;
        }
        
        isSnapping = false;
        bool isDecelerating = speed < decelerationThreshold;
        Debug.Log("Speed: " + speed + " | Is Decelerating: " + isDecelerating);
        return isDecelerating;
    }

    private void SnapToKey(GameObject key)
    {
        if (key != null)
        {
            transform.position = key.transform.position;
            Debug.Log("Snapped to key: " + key.name);
        }
    }

    private void HoverKey(Collider2D collider)
    {
        if (collider.TryGetComponent(out KeyboardKey keyboardKey)) {
            keyboardKey.OnHoverEnter();
        }
        else {
            Debug.LogWarning("Not a valid Key?");
        }
    }

    private void UnHoverPreviousKey()
    {
        if (previousDetectedKey != null) {
            if (previousDetectedKey.TryGetComponent(out KeyboardKey k)) {
                k.OnHoverExit();
            }
        }
    }

    private void DeSelectKey(GameObject key)
    {
        if (key.TryGetComponent(out KeyboardKey k)) {
            if (k.onSelect == false) return;
            k.OnDeSelect();
        }
    }

    private void SelectKey(Collider2D collider)
    {
        if (collider == null) return;
        if (collider.TryGetComponent(out KeyboardKey k)) {
            k.OnSelect();
        }
        else {
            Debug.LogWarning("Not a valid Key?");
        }
    }
}




    // OLD CODE BELOW (for reference):

    // private StudyBehavior studyBehavior;

    // private void Awake()
    // {
    //     mainCam = Camera.main;
    //     studyBehavior = FindObjectOfType<StudyBehavior>();
    // }

    // void Update()
    // {
    //     keys = GameObject.FindGameObjectsWithTag("Key");

    //     //Get Mouse Position on screen, and get the corresponding position in a Vector3 World Co-Ordinate
    //     Vector3 mousePosition = Input.mousePosition;

    //     //Change the z position so that cursor does not get occluded by the camera
    //     mousePosition.z += 9f;
    //     mousePosition.x = Mathf.Clamp(mousePosition.x, 0f, Screen.width);
    //     mousePosition.y = Mathf.Clamp(mousePosition.y, 0f, Screen.height);
    //     transform.position = mainCam.ScreenToWorldPoint(mousePosition);

    //     if (studyBehavior.StudySettings.cursorType == CursorType.PointCursor)
    //     {
    //         // Point Cursor behaviour
    //         transform.localScale = new Vector2(0,0);
    //         radius = 0f;
    //     }

    //     Collider2D detectedCollider = null;

    //     Physics2D.OverlapCircle(transform.position, radius, contactFilter, results);

    //     //Detect how many targets
    //     //Change previous target back to default colour
    //     if (results.Count < 1)
    //     {
    //         UnHoverPreviousKey();
    //     }
    //     else if (results.Count > 1)
    //     {
    //         UnHoverPreviousKey();
    //         Debug.LogWarning("Too many targets in area");
    //         return;
    //     }
    //     else
    //     {
    //         detectedCollider = results[0];
    //         UnHoverPreviousKey(detectedCollider);
    //         HoverKey(detectedCollider);
    //     }

    //     // On Mouse Click, select the closest target
    //     if (Input.GetMouseButtonDown(0))
    //     {
    //         SelectTarget(detectedCollider);
    //     }

    //     previousDetectedKey = detectedCollider;
    // }

    // //Debug code
    // private void OnDrawGizmos()
    // {
    //     Gizmos.DrawWireSphere(transform.position, radius);
    // }
// }
