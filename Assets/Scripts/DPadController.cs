using System.Collections.Generic;
using UnityEngine;

public class DPadController : MonoBehaviour
{
    public GameObject keysParent;
    public int keysPerRow = 10;
    private List<KeyboardKey> keys = new List<KeyboardKey>();
    private int currentIndex = 0;

    void Start()
    {
        InitializeKeys();
        if (keys.Count > 0)
        {
            currentIndex = 0;
            keys[currentIndex].OnHoverEnter();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveToKey(currentIndex + 1);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveToKey(currentIndex - 1);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            MoveToKey(currentIndex + keysPerRow);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            MoveToKey(currentIndex - keysPerRow);
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
        if (newIndex >= 0 && newIndex < keys.Count)
        {
            int currentRow = currentIndex / keysPerRow;
            int newRow = newIndex / keysPerRow;

            if (Mathf.Abs(newIndex - currentIndex) == 1 && currentRow != newRow)
            {
                return;
            }

            keys[currentIndex].OnHoverExit();
            currentIndex = newIndex;
            keys[currentIndex].OnHoverEnter();
        }
    }
}
