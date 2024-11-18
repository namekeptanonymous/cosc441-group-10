using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField participantIDInputField;
    [SerializeField] private TMP_InputField cursorModeInputField;

    private Cursor gameCursor;
    private StudyBehavior studyBehavior;
    private int participantID;
    private Cursor.CursorMode cursorMode;

    private void Awake()
    {
        DontDestroyOnLoad(this); // Persist GameManager across scenes
    }

    public void StartStudy()
    {
        // Validate inputs
        if (string.IsNullOrWhiteSpace(participantIDInputField.text))
        {
            Debug.LogError("Participant ID is required!");
            return;
        }
        if (string.IsNullOrWhiteSpace(cursorModeInputField.text))
        {
            Debug.LogError("Cursor mode is required!");
            return;
        }
        if (!int.TryParse(participantIDInputField.text, out participantID))
        {
            Debug.LogError("Invalid participant ID!");
            return;
        }
        string modeInput = cursorModeInputField.text.Trim().ToUpper();
        switch (modeInput)
        {
            case "P":
            case "POINT":
                cursorMode = Cursor.CursorMode.Point;
                break;

            case "S":
            case "SNAP":
                cursorMode = Cursor.CursorMode.Snap;
                break;

            case "D":
            case "DPAD":
                cursorMode = Cursor.CursorMode.DPad;
                break;

            default:
                Debug.LogError("Invalid cursor mode! Use 'C' for Cursor, 'S' for Snap, or 'D' for DPad.");
                return;
        }

        SceneManager.LoadScene("StudyScene");
        SceneManager.sceneLoaded += OnStudySceneLoaded;
    }

    private void OnStudySceneLoaded(Scene scene, LoadSceneMode mode)
    {
        gameCursor = FindObjectOfType<Cursor>();
        studyBehavior = FindObjectOfType<StudyBehavior>();

        if (gameCursor == null || studyBehavior == null)
        {
            Debug.LogError("Cursor or StudyBehavior not found in the scene!");
            return;
        }

        studyBehavior.ParticipantID = participantID;

        CSVManager.SetFilePath(cursorMode.ToString());
        ApplyCursorMode(cursorMode);

        SceneManager.sceneLoaded -= OnStudySceneLoaded;
    }

    private void ApplyCursorMode(Cursor.CursorMode mode)
    {
        if (gameCursor == null)
        {
            Debug.LogError("Cursor not found!");
            return;
        }
        gameCursor.SetCursorMode(mode);
    }
}
