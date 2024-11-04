using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // [SerializeField] private TMP_InputField inputField;
    // [SerializeField] private TMP_InputField cursorTypeInputField;
    // private Cursor gameCursor;
    // private StudyBehavior studyBehavior;
    // private int participantID;
    // private CursorType cursorType;

    // private void Awake()
    // {
    //     DontDestroyOnLoad(this);
    // }

    // public void StartStudy()
    // {
    //     if (inputField.text == string.Empty) return;
    //     else if (cursorTypeInputField.text == string.Empty) return;

    //     participantID = int.Parse(inputField.text);
    //     if (cursorTypeInputField.text.Equals("point", StringComparison.OrdinalIgnoreCase) || cursorTypeInputField.text.Equals("p", StringComparison.OrdinalIgnoreCase)) {
    //         cursorType = CursorType.PointCursor;
    //     } else if (cursorTypeInputField.text.Equals("bubble", StringComparison.OrdinalIgnoreCase) || cursorTypeInputField.text.Equals("b", StringComparison.OrdinalIgnoreCase)) {
    //         cursorType = CursorType.BubbleCursor;
    //     } else {
    //         Debug.LogError("Invalid cursor type!");
    //         return;
    //     }

    //     SceneManager.LoadScene("StudyScene");
    //     SceneManager.sceneLoaded += OnGameSceneLoaded;
    // }

    // private void OnGameSceneLoaded(Scene scene, LoadSceneMode mode)
    // {
    //     gameCursor = FindObjectOfType<Cursor>();
    //     studyBehavior = FindObjectOfType<StudyBehavior>();

    //     if (gameCursor == null || studyBehavior == null)
    //     {
    //         Debug.LogError("Cursor or StudyBehavior not found in the scene!");
    //         return;
    //     }

    //     studyBehavior.ParticipantID = participantID;
        
    //     CSVManager.SetFilePath(cursorType.ToString());
    //     SetCursor(cursorType);

    //     // Unsubscribe from the sceneLoaded event to avoid duplicate calls
    //     SceneManager.sceneLoaded -= OnGameSceneLoaded;
    // }

    // public void SetCursor(CursorType cursor)
    // {
    //     if (gameCursor == null) return;
    //     studyBehavior.StudySettings.cursorType = cursor;
    //     gameCursor.radius = cursor switch
    //     {
    //         CursorType.PointCursor => 0f,
    //         CursorType.BubbleCursor => 10f,
    //         _ => throw new ArgumentOutOfRangeException(nameof(cursor), cursor, null)
    //     };
    // }
}
