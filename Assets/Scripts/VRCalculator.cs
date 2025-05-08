using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VRCalculator : MonoBehaviour
{
    public TMP_InputField displayField;
    private string currentInput = "";
    private float currentValue = 0f;
    private string pendingOperation = "";
    private bool newInput = true;

    void Start()
    {
        ClearCalculator();
    }

    public void AppendNumber(string number)
    {
        if (newInput)
        {
            currentInput = number;
            newInput = false;
        }
        else
        {
            currentInput += number;
        }

        displayField.text = currentInput;
    }

    public void SetOperation(string operation)
    {
        if (!newInput)
        {
            CalculateResult();
        }

        pendingOperation = operation;
        currentValue = float.Parse(currentInput);
        newInput = true;
    }

    public void CalculateResult()
    {
        if (string.IsNullOrEmpty(pendingOperation) || newInput) return;

        float newValue = float.Parse(currentInput);

        switch (pendingOperation)
        {
            case "+":
                currentValue += newValue;
                break;
            case "-":
                currentValue -= newValue;
                break;
            case "*":
                currentValue *= newValue;
                break;
            case "/":
                if (newValue != 0)
                    currentValue /= newValue;
                else
                    Debug.LogError("Division by zero!");
                break;
        }

        displayField.text = currentValue.ToString();
        currentInput = currentValue.ToString();
        pendingOperation = "";
        newInput = true;
    }

    public void ClearCalculator()
    {
        currentInput = "";
        currentValue = 0f;
        pendingOperation = "";
        displayField.text = "0";
        newInput = true;
    }

    public void AddDecimalPoint()
    {
        if (newInput)
        {
            currentInput = "0.";
            newInput = false;
        }
        else if (!currentInput.Contains("."))
        {
            currentInput += ".";
        }

        displayField.text = currentInput;
    }
}