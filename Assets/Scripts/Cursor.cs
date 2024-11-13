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
    [SerializeField] private CursorMode cursorMode = CursorMode.Snap;

    private Camera mainCam;
    private List<Collider2D> results = new();
    private GameObject previousDetectedKey;
    private GameObject[] keys;

    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        keys = GameObject.FindGameObjectsWithTag("Key");

        //Get Mouse Position on screen, and get the corresponding position in a Vector3 World Co-Ordinate
        Vector3 mousePosition = Input.mousePosition;

        //Change the z position so that cursor does not get occluded by the camera
        mousePosition.z += 9f;
        mousePosition.x = Mathf.Clamp(mousePosition.x, 0f, Screen.width);
        mousePosition.y = Mathf.Clamp(mousePosition.y, 0f, Screen.height);
        transform.position = mainCam.ScreenToWorldPoint(mousePosition);

        float distance = 9999f;
        GameObject closestKey = null;

        // if (studyBehavior.StudySettings.cursorType == CursorType.PointCursor) {}
        if (cursorMode == CursorMode.Point) {
            // Point Cursor behaviour goes here

            // Collider2D detectedCollider = null;
            // Physics2D.OverlapCircle(transform.position, radius, contactFilter, results);

            // //Detect how many targets
            // //Change previous target back to default colour
            // if (results.Count < 1)
            // {
            //     UnHoverPreviousKey();
            // }
            // else if (results.Count > 1)
            // {
            //     UnHoverPreviousKey();
            //     Debug.LogWarning("Too many keys in area");
            //     return;
            // }
            // else
            // {
            //     detectedCollider = results[0];
            //     UnHoverPreviousKey(detectedCollider);
            //     HoverKey(detectedCollider);
            // }
        } else if (cursorMode == CursorMode.Snap) {
            // Locating the nearest Key - Based on the distance to the nearest target to the Cursor
            foreach (GameObject currentKey in keys) {
                float distanceToKey = Vector2.Distance(currentKey.transform.position, transform.position);
                if (distanceToKey < distance) {
                    distance = distanceToKey;
                    closestKey = currentKey;
                }
            }

            if (distance < 0.7f) {
                if (closestKey)
                {
                    if (previousDetectedKey != null && closestKey != previousDetectedKey) UnHoverPreviousKey();
                    HoverKey(closestKey.GetComponent<Collider2D>());
                }

                // On Mouse Click, select the closest target
                if (Input.GetMouseButtonDown(0))
                {
                    SelectKey(closestKey.GetComponent<Collider2D>());
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    DeSelectKey(closestKey);
                }
            }
            else {
                UnHoverPreviousKey();
            }

            previousDetectedKey = closestKey;
        } else {
            // D-Pad behaviour goes here
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

    void SelectKey(Collider2D collider)
    {
        if (collider == null)  {
            // // If nothing was clicked, count it as a misclick
            // if (FindObjectOfType<Key>() == null)
            // {
            //     studyBehavior.HandleMisClick();
            // }
            // return;
        }
        if (collider.TryGetComponent(out KeyboardKey k)) {
            k.OnSelect();
        }
        else {
            Debug.LogWarning("Not a valid Key?");
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
}
