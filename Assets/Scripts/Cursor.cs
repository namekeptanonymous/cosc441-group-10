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

    // Serialize the enum field so it shows as a dropdown in the Inspector
    [SerializeField] private CursorMode cursorMode = CursorMode.Point;

    private Camera mainCam;
    private List<Collider2D> results = new();
    private GameObject previousDetectedKey;
    private GameObject[] keys;

    private Vector3 previousMousePosition;
    [SerializeField] private float decelerationThreshold = 0.1f; // Adjust as needed in Inspector
    [SerializeField] private float minDistanceToSnap = 0.7f;     // Distance within which snapping can occur
    [SerializeField] private float snapCooldownTime = 0.5f;      // Time after stopping before snapping resumes
    private float snapCooldownTimer = 0f;
    private bool isStopped = false;

    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;
        previousMousePosition = Input.mousePosition;
    }

    // Update is called once per frame
    void Update()
    {
        keys = GameObject.FindGameObjectsWithTag("Key");

        // Get Mouse Position on screen and convert it to world position
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z += 9f;
        mousePosition.x = Mathf.Clamp(mousePosition.x, 0f, Screen.width);
        mousePosition.y = Mathf.Clamp(mousePosition.y, 0f, Screen.height);
        transform.position = mainCam.ScreenToWorldPoint(mousePosition);

        float distance = 9999f;
        GameObject closestKey = null;

        if (cursorMode == CursorMode.DPad) {
            // D-Pad behaviour goes here
            Debug.Log("DPad mode is not implemented.");
        } 
        else {
            // Find the nearest Key
            foreach (GameObject currentKey in keys) {
                float distanceToKey = Vector2.Distance(currentKey.transform.position, transform.position);
                if (distanceToKey < distance) {
                    distance = distanceToKey;
                    closestKey = currentKey;
                }
            }

            // Update snap cooldown timer if cursor is stopped
            if (isStopped)
            {
                snapCooldownTimer += Time.deltaTime;
                if (snapCooldownTimer >= snapCooldownTime)
                {
                    isStopped = false; // Grace period has passed, snapping can resume if conditions are met
                    Debug.Log("Snap cooldown ended, snapping can resume.");
                }
            }

            // Snapping logic
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

            // Regular Point Cursor behaviour if snapping conditions are not met
            if (cursorMode == CursorMode.Point || cursorMode != CursorMode.Snap || distance >= minDistanceToSnap || !IsDecelerating())
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

                // Handle selection/deselection
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
        // Calculate mouse movement speed
        float speed = (Input.mousePosition - previousMousePosition).magnitude / Time.deltaTime;

        // Detect if the cursor has stopped (speed near zero)
        if (speed < Mathf.Epsilon)
        {
            snapCooldownTimer = 0f; // Reset snap timer since cursor just stopped
            isStopped = true;
            Debug.Log("Cursor stopped, starting cooldown.");
            return false;
        }
        
        // Check if speed is below the deceleration threshold
        bool isDecelerating = speed < decelerationThreshold;
        Debug.Log("Speed: " + speed + " | Is Decelerating: " + isDecelerating);
        return isDecelerating;
    }

    private void SnapToKey(GameObject key)
    {
        // Set in-game cursor position to the key position
        transform.position = key.transform.position;
        Debug.Log("Snapped to key: " + key.name);
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
        if (collider == null)  {
            return;
        }
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
    //   