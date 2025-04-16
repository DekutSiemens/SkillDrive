using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MultipleChoiceQuizController : MonoBehaviour
{
    [System.Serializable]
    public class QuizQuestion
    {
        public string questionName;
        public GameObject questionObject;
        public int optionCount;
        public List<bool> correctAnswers = new List<bool>();
    }

    [Header("Question Setup")]
    public List<QuizQuestion> questions = new List<QuizQuestion>();

    [Header("UI Elements")]
    public GameObject buttonPrefab;
    public Transform buttonContainer;
    public Button submitButton;
    public TextMeshProUGUI feedbackText;
    public TextMeshProUGUI questionNumberText;
    public TextMeshProUGUI finalScoreText;

    [Header("Panels")]
    public GameObject introPanel;
    public GameObject outroPanel;

    private int currentQuestionIndex = -1;
    private List<Button> optionButtons = new List<Button>();
    private List<bool> selectedOptions = new List<bool>();
    private Queue<int> selectionQueue = new Queue<int>();
    private int correctAnswersRequired = 0;
    private List<bool> answerResults = new List<bool>();

    private void Start()
    {
        if (submitButton != null)
        {
            submitButton.onClick.AddListener(CheckAnswer);
        }

        foreach (var q in questions)
        {
            if (q.questionObject != null)
                q.questionObject.SetActive(false);
        }

        if (introPanel != null)
            introPanel.SetActive(true);

        if (outroPanel != null)
            outroPanel.SetActive(false);
    }

    public void StartQuiz()
    {
        if (introPanel != null)
            introPanel.SetActive(false);

        if (questions.Count > 0)
            ShowQuestion(0);
    }

    public void ShowQuestion(int index)
    {
        if (index < 0 || index >= questions.Count)
        {
            Debug.LogError("Question index out of range!");
            return;
        }

        if (currentQuestionIndex >= 0 && currentQuestionIndex < questions.Count)
        {
            questions[currentQuestionIndex].questionObject?.SetActive(false);
        }

        currentQuestionIndex = index;
        QuizQuestion currentQ = questions[currentQuestionIndex];
        currentQ.questionObject?.SetActive(true);

        if (questionNumberText != null)
        {
            questionNumberText.text = $"Question {currentQuestionIndex + 1} of {questions.Count}";
        }

        correctAnswersRequired = 0;
        foreach (bool b in currentQ.correctAnswers)
            if (b) correctAnswersRequired++;

        ClearOptionButtons();
        selectedOptions.Clear();

        for (int i = 0; i < currentQ.optionCount; i++)
        {
            if (i >= currentQ.correctAnswers.Count)
                currentQ.correctAnswers.Add(false);

            GameObject buttonObj = Instantiate(buttonPrefab, buttonContainer);
            Button button = buttonObj.GetComponent<Button>();
            TextMeshProUGUI text = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            char optionLetter = (char)('A' + i);
            text.text = optionLetter.ToString();

            int optionIndex = i;
            button.onClick.AddListener(() => ToggleOption(optionIndex));

            optionButtons.Add(button);
            selectedOptions.Add(false);
        }

        selectionQueue.Clear();
        if (feedbackText != null)
            feedbackText.text = $"Select {correctAnswersRequired} option{(correctAnswersRequired > 1 ? "s" : "")}";
    }

    private void ToggleOption(int index)
    {
        if (selectedOptions[index])
        {
            selectedOptions[index] = false;
            Queue<int> newQueue = new Queue<int>();
            foreach (int i in selectionQueue)
                if (i != index)
                    newQueue.Enqueue(i);
            selectionQueue = newQueue;
            UpdateButtonVisual(optionButtons[index], false);
            return;
        }

        if (selectionQueue.Count >= correctAnswersRequired)
        {
            int oldest = selectionQueue.Dequeue();
            selectedOptions[oldest] = false;
            UpdateButtonVisual(optionButtons[oldest], false);
        }

        selectedOptions[index] = true;
        selectionQueue.Enqueue(index);
        UpdateButtonVisual(optionButtons[index], true);
    }

    private void UpdateButtonVisual(Button button, bool selected)
    {
        ColorBlock cb = button.colors;

        if (selected)
        {
            Color32 selectedColor = new Color32(0x00, 0x36, 0x6D, 255);
            cb.normalColor = selectedColor;
            cb.highlightedColor = selectedColor;
            cb.pressedColor = selectedColor;
            cb.selectedColor = selectedColor;
        }
        else
        {
            Color32 defaultColor = new Color32(255, 255, 255, 255);
            cb.normalColor = defaultColor;
            cb.highlightedColor = defaultColor;
            cb.pressedColor = defaultColor;
            cb.selectedColor = defaultColor;
        }

        button.colors = cb;
        button.transition = Selectable.Transition.ColorTint;
    }

    private void ClearOptionButtons()
    {
        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }

        optionButtons.Clear();
        selectedOptions.Clear();
        selectionQueue.Clear();
    }

    public void CheckAnswer()
    {
        if (currentQuestionIndex < 0 || currentQuestionIndex >= questions.Count)
            return;

        QuizQuestion currentQ = questions[currentQuestionIndex];

        bool isCorrect = true;
        for (int i = 0; i < currentQ.correctAnswers.Count && i < selectedOptions.Count; i++)
        {
            if (currentQ.correctAnswers[i] != selectedOptions[i])
            {
                isCorrect = false;
                break;
            }
        }

        answerResults.Add(isCorrect);

        if (feedbackText != null)
        {
            feedbackText.text = isCorrect ? "Correct" : "Incorrect";
        }

        StartCoroutine(NextQuestionAfterDelay(1.5f));
    }

    private System.Collections.IEnumerator NextQuestionAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        int nextIndex = currentQuestionIndex + 1;

        if (nextIndex < questions.Count)
        {
            ShowQuestion(nextIndex);
        }
        else
        {
            submitButton.interactable = false;

            if (questions[currentQuestionIndex].questionObject != null)
            {
                questions[currentQuestionIndex].questionObject.SetActive(false);
            }

            int correctCount = 0;
            for (int i = 0; i < answerResults.Count; i++)
            {
                if (answerResults[i]) correctCount++;
            }

            if (finalScoreText != null)
            {
                string summary = $"Final Score: {correctCount} / {questions.Count}\n\n";
                for (int i = 0; i < answerResults.Count; i++)
                {
                    string resultText = answerResults[i] ? "Correct" : "Incorrect";
                    summary += $"Question {i + 1}: {resultText}\n";
                }

                finalScoreText.text = summary;
            }

            if (feedbackText != null)
            {
                feedbackText.text = "";
            }

            if (outroPanel != null)
            {
                outroPanel.SetActive(true);
            }
        }
    }

    public void ResetQuiz()
    {
        if (currentQuestionIndex >= 0 && currentQuestionIndex < questions.Count)
        {
            questions[currentQuestionIndex].questionObject?.SetActive(false);
        }

        ClearOptionButtons();
        currentQuestionIndex = -1;
        correctAnswersRequired = 0;
        selectionQueue.Clear();
        answerResults.Clear();

        if (feedbackText != null)
        {
            feedbackText.text = "";
        }

        if (finalScoreText != null)
        {
            finalScoreText.text = "";
        }

        submitButton.interactable = true;

        if (introPanel != null)
            introPanel.SetActive(true);

        if (outroPanel != null)
            outroPanel.SetActive(false);
    }
}
