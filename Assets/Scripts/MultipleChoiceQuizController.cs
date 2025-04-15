using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MultipleChoiceQuizController : MonoBehaviour
{
    [System.Serializable]
    public class QuestionData
    {
        public GameObject questionObject;        // The image GameObject for the question
        public int numberOfOptions = 4;          // Number of answer choices (e.g., 4 = A-D)
        public List<bool> correctAnswers;        // Boolean list to define correct options (A = 0, B = 1, etc.)
    }

    [Header("Question Setup")]
    public List<QuestionData> questions = new List<QuestionData>(); // All question data
    public Transform answerPanel;               // Vertical layout group for answer buttons
    public Button answerButtonPrefab;           // Prefab with TMP or Text for label

    [Header("Instruction and Outro UI")]
    public TextMeshProUGUI instructionText;     // Instructions ("Select X options")
    public TextMeshProUGUI outroTextPanel;      // Final result text ("Your Score: X")

    private int currentQuestionIndex = 0;
    private List<ToggleButton> activeButtons = new List<ToggleButton>();
    private List<int> selectedIndices = new List<int>();
    private int maxSelectionsAllowed = 1;

    private int score = 0;
    private List<bool> resultsPerQuestion = new List<bool>(); // true = correct, false = incorrect

    private readonly string[] optionLabels = { "A", "B", "C", "D", "E", "F", "G" };

    void Start()
    {
        ResetQuiz(); // Automatically starts quiz on play
    }

    public void LoadQuestion(int index)
    {
        if (index < 0 || index >= questions.Count) return;

        // Deactivate all questions
        foreach (var q in questions)
            q.questionObject.SetActive(false);

        // Set question index and activate current
        currentQuestionIndex = index;
        var question = questions[currentQuestionIndex];
        question.questionObject.SetActive(true);

        // Clear previous buttons
        ClearAnswerPanel();

        // Count number of correct answers
        maxSelectionsAllowed = 0;
        foreach (bool isCorrect in question.correctAnswers)
            if (isCorrect) maxSelectionsAllowed++;

        maxSelectionsAllowed = Mathf.Max(1, maxSelectionsAllowed);
        instructionText.text = $"Select {maxSelectionsAllowed} option{(maxSelectionsAllowed > 1 ? "s" : "")}";

        // Generate new buttons
        for (int i = 0; i < question.numberOfOptions; i++)
        {
            Button newBtn = Instantiate(answerButtonPrefab, answerPanel);
            string label = (i < optionLabels.Length) ? optionLabels[i] : $"Option {i + 1}";

            var text = newBtn.GetComponentInChildren<TextMeshProUGUI>();
            if (text != null) text.text = label;

            var toggle = newBtn.gameObject.AddComponent<ToggleButton>();
            toggle.Init(i, OnOptionSelected);
            activeButtons.Add(toggle);
        }
    }

    private void ClearAnswerPanel()
    {
        foreach (Transform child in answerPanel)
            Destroy(child.gameObject);

        activeButtons.Clear();
        selectedIndices.Clear();
    }

    private void OnOptionSelected(int index, bool isSelected)
    {
        if (isSelected)
        {
            if (selectedIndices.Contains(index)) return;

            if (selectedIndices.Count < maxSelectionsAllowed)
            {
                selectedIndices.Add(index);
            }
            else
            {
                int oldest = selectedIndices[0];
                selectedIndices.RemoveAt(0);
                activeButtons[oldest].SetSelected(false);
                selectedIndices.Add(index);
            }
        }
        else
        {
            selectedIndices.Remove(index);
        }
    }

    public void SubmitAnswer()
    {
        var question = questions[currentQuestionIndex];

        // Check if user selected exactly the correct answers
        bool isCorrect = true;
        for (int i = 0; i < question.correctAnswers.Count; i++)
        {
            bool shouldBeSelected = question.correctAnswers[i];
            bool actuallySelected = selectedIndices.Contains(i);

            if (shouldBeSelected != actuallySelected)
            {
                isCorrect = false;
                break;
            }
        }

        resultsPerQuestion.Add(isCorrect);
        if (isCorrect) score++;

        // Load next question or show result
        if (currentQuestionIndex + 1 < questions.Count)
        {
            LoadNextQuestion();
        }
        else
        {
            ShowFinalResults();
        }
    }

    public void LoadNextQuestion()
    {
        int nextIndex = currentQuestionIndex + 1;

        if (nextIndex < questions.Count)
        {
            LoadQuestion(nextIndex);
        }
        else
        {
            ShowFinalResults();
        }
    }

    private void ShowFinalResults()
    {
        ClearAnswerPanel();
        instructionText.text = "All questions completed!";

        if (outroTextPanel != null)
        {
            string summary = $"Your Score: {score}/{questions.Count}\n\n";
            for (int i = 0; i < resultsPerQuestion.Count; i++)
            {
                summary += $"Question {i + 1}: {(resultsPerQuestion[i] ? "<color=green>Correct</color>" : "<color=red>Wrong</color>")}\n";
            }
            outroTextPanel.text = summary;
        }
    }

    public void ResetQuiz()
    {
        score = 0;
        resultsPerQuestion.Clear();
        outroTextPanel.text = "";
        LoadQuestion(0);
    }
}
