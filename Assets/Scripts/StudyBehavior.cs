using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class TrialConditions
{
    public Cursor.CursorMode cursorMode; // Cursor mode for the trial
    public string word;                 // Word to be typed
    public Vector3 keyboardScale;       // Default keyboard scale is 0.6772644.
}



[Serializable]
public class StudySettings
{
    public List<string> wordsToType;            // Words for the trials
    public List<Cursor.CursorMode> cursorModes; // Cursor modes to test
    public List<Vector3> keyboardScales;        // Keyboard scales to test
}

public class StudyBehavior : MonoBehaviour
{
    [SerializeField] private TMP_Text WordBeingTyped;
    [SerializeField] private TMP_Text WordToType;
    public TrialConditions CurrentTrial => blockSequence[currentTrialIndex];
    public StudySettings StudySettings => studySettings;

    public int ParticipantID
    {
        get => participantID;
        set => participantID = value;
    }

    private int participantID;
    [SerializeField] private StudySettings studySettings;
    [SerializeField] private int repetitions;
    [SerializeField] private List<TrialConditions> blockSequence = new();
    
    private float timer = 0f;
    private int currentTrialIndex = 0;
    private int numOfTypos = 0;
    public string typedWord = ""; // Tracks the word being typed
    public string nextCorrectLetter = ""; // Tracks the next letter to be typed
    private Cursor cursor;
    private GameObject keyboard; // Reference to the keyboard object

    private string[] header =
    {
        "PID",
        "CursorMode",
        "Word",
        "CompletionTime",
        "NumOfTypos",
        "KeyboardScale"
    };

    void Awake()
    {
        CreateBlock(); // Generate the trial block sequence
    }

    private void Start()
    {
        cursor = FindObjectOfType<Cursor>();
        if (cursor == null)
        {
            Debug.LogError("Cursor not found in the scene!");
        }

        keyboard = GameObject.Find("Keyboard");
        if (keyboard == null)
        {
            Debug.LogError("Keyboard not found in the scene!");
        }

        LogHeader();
        ApplyTrialConditions();
    }

    private void Update()
    {
        timer += Time.deltaTime;
    }

    public void RegisterKeyPress(string key)
    {
        typedWord += key;
        Debug.Log($"Typed word: {typedWord}");
        WordBeingTyped.text = typedWord;
        if (typedWord.Length < CurrentTrial.word.Length)
        {
            nextCorrectLetter = CurrentTrial.word[typedWord.Length].ToString();
        }
        else
        {
            nextCorrectLetter = "";
        }

        // Check if the typed word matches the trial's word
        if (typedWord.Equals(CurrentTrial.word, StringComparison.OrdinalIgnoreCase))
        {
            Debug.Log($"Word completed: {typedWord}");
            NextTrial();
        }
    }

    public void HandleTypo()
    {
        numOfTypos++;
        Debug.Log("Typo entered!");
    }

    public void NextTrial()
    {
        LogData();
        currentTrialIndex++;

        if (currentTrialIndex >= blockSequence.Count)
        {
            SceneManager.LoadScene("EndScreen");
        }
        else
        {
            ApplyTrialConditions();
        }
    }

    private void CreateBlock()
    {
        foreach (string word in studySettings.wordsToType)
        {
            foreach (Vector3 scale in studySettings.keyboardScales)
            {
                for (int i = 0; i < repetitions; i++)
                {
                    blockSequence.Add(new TrialConditions()
                    {
                        cursorMode = studySettings.cursorModes[0],
                        word = word,
                        keyboardScale = scale
                    });
                }
            }
        }
        blockSequence = YatesShuffle(blockSequence);
    }

    private void ApplyTrialConditions()
    {
        if (cursor == null) return;

        TrialConditions trial = CurrentTrial;
        typedWord = ""; // Reset the typed word
        nextCorrectLetter = trial.word[0].ToString();

        WordBeingTyped.text = "";
        WordToType.text = trial.word;

        keyboard.transform.localScale = trial.keyboardScale;

        Debug.Log($"Trial {currentTrialIndex + 1}/{blockSequence.Count}: " +
                  $"Cursor Mode = {trial.cursorMode}, Word = {trial.word}");
    }

    private void LogHeader()
    {
        CSVManager.AppendToCSV(header);
    }

    private void LogData()
    {
        string[] data =
        {
            participantID.ToString(),
            CurrentTrial.cursorMode.ToString(),
            CurrentTrial.word,
            (timer * 1000).ToString(),
            numOfTypos.ToString(),
            CurrentTrial.keyboardScale.ToString()
        };

        CSVManager.AppendToCSV(data);
        timer = 0f;
        numOfTypos = 0;
    }

    public void SetParticipantID(int ID)
    {
        participantID = ID;
    }

    private static List<T> YatesShuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = UnityEngine.Random.Range(0, i + 1);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
        return list;
    }
}
