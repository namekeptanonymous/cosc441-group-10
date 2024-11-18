using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardKey : MonoBehaviour
{
    private SpriteRenderer sprite;
    public bool onSelect;
    
    void Start()
    {
        sprite = GetComponentInChildren<SpriteRenderer>();
    }
    void Update()
    {
        
    }

    public void OnHoverEnter()
    {
        if(onSelect)return;
        Color currentColor = sprite.color;
        sprite.color = new Color(currentColor.r, currentColor.g, currentColor.b, 0.1f);
        Debug.Log("Hovering over key "+gameObject.name);
    }

    public void OnHoverExit()
    {
        onSelect = false;
        Color currentColor = sprite.color;
        sprite.color = new Color(currentColor.r, currentColor.g, currentColor.b, 0f);
        Debug.Log("Left key "+gameObject.name);
    }

    public void OnSelect()
    {
        onSelect = true;
        Color currentColor = sprite.color;
        sprite.color = new Color(currentColor.r, currentColor.g, currentColor.b, 0.2f);
        Debug.Log("Key "+gameObject.name+" was pressed.");

        FindObjectOfType<StudyBehavior>()?.RegisterKeyPress(gameObject.name);
    }

    public void OnDeSelect()
    {
        onSelect = false;
        OnHoverEnter();
    }
}