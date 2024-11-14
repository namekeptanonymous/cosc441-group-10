using System.Collections.Generic;
using UnityEngine;

public class DPadController : MonoBehaviour
{
    public GameObject keysParent; // Parent object containing all the keys
    public int keysPerRow = 10; // Number of keys per row for calculating up/down movements
    private List<KeyboardKey> keys = new List<KeyboardKey>(); // Flat list of keys
    private int currentIndex = 0; // Index of the currently selected key

    void Start()
    {
        InitializeKeys();
        if (keys.Count > 0)
        {
            currentIndex = 0;
            keys[currentIndex].OnHoverEnter(); // Highlight the first key
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveToKey(currentIndex + 1); // Move right (next key in the list)
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveToKey(currentIndex - 1); // Move left (previous key in the list)
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            MoveToKey(currentIndex + keysPerRow); // Move down by one row
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            MoveToKey(currentIndex - keysPerRow); // Move up by one row
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            keys[currentIndex].OnSelect();
        }
    }

    private void InitializeKeys()
    {
        if (keysParent == null)
        {
            Debug.LogError("Keys parent object is not assigned!");
            return;
        }

        // Populate the flat list of keys from the children of keysParent
        foreach (Transform keyTransform in keysParent.transform)
        {
            KeyboardKey keyboardKey = keyTransform.GetComponent<KeyboardKey>();
            if (keyboardKey != null)
            {
                keys.Add(keyboardKey);
            }
        }

        Debug.Log("Initialized " + keys.Count + " keys.");
    }

    private void MoveToKey(int newIndex)
    {
        // Ensure newIndex is within bounds and prevent wrapping
        if (newIndex >= 0 && newIndex < keys.Count)
        {
            // Calculate row positions to prevent horizontal wraparound
            int currentRow = currentIndex / keysPerRow;
            int newRow = newIndex / keysPerRow;

            if (Mathf.Abs(newIndex - currentIndex) == 1 && currentRow != newRow)
            {
                // Do nothing if it wraps around horizontally
                return;
            }

            keys[currentIndex].OnHoverExit(); // Unhighlight the current key
            currentIndex = newIndex;
            keys[currentIndex].OnHoverEnter(); // Highlight the new key
        }
    }
}
