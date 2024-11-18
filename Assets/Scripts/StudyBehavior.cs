using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class TrialConditions
{
    public Cursor.CursorMode cursorMode; // Cursor mode for the trial
    public string word;                 // Word to be typed
}

[Serializable]
public class StudySettings
{
    public List<string> wordsToType;            // Words for the trials
    public List<Cursor.CursorMode> cursorModes; // Cursor modes to test
}

public class StudyBehavior : MonoBehaviour
{
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
    private string typedWord = ""; // Tracks the word being typed
    private Cursor cursor;

    private string[] header =
    {
        "PID",
        "CursorMode",
        "Word",
        "TypedWord",
        "CompletionTime",
        "MissedKeys"
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

        // Check if the typed word matches the trial's word
        if (typedWord.Equals(CurrentTrial.word, StringComparison.OrdinalIgnoreCase))
        {
            Debug.Log($"Word completed: {typedWord}");
            NextTrial();
        }
    }

    public void HandleMissedKey()
    {
        Debug.Log("Missed key!");
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
        foreach (Cursor.CursorMode mode in studySettings.cursorModes)
        {
            for (int i = 0; i < repetitions; i++)
            {
                foreach (string word in studySettings.wordsToType)
                {
                    blockSequence.Add(new TrialConditions()
                    {
                        cursorMode = mode,
                        word = word
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
        cursor.SetCursorMode(trial.cursorMode);
        typedWord = ""; // Reset the typed word

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
            typedWord,
            (timer * 1000).ToString(),
            (CurrentTrial.word.Length - typedWord.Length).ToString() // Missed keys
        };

        CSVManager.AppendToCSV(data);
        timer = 0f;
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
