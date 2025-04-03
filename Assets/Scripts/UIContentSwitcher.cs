using UnityEngine;

public class UIContentSwitcher : MonoBehaviour
{
    public GameObject[] contentPanels; // Array of UI content panels
    private int currentContentIndex = 0;

    // Function to show the selected content and hide others
    public void ShowContent(int index)
    {
        // Disable all content panels first
        foreach (GameObject panel in contentPanels)
        {
            panel.SetActive(false);
        }

        // Enable the selected one
        if (index >= 0 && index < contentPanels.Length)
        {
            currentContentIndex = index;
            contentPanels[index].SetActive(true);
        }
        else
        {
            Debug.LogWarning($"Invalid content index: {index}. Index must be between 0 and {contentPanels.Length - 1}.");
        }
    }

    // Function to move to the next content panel
    public void Next()
    {
        // Disable current panel
        contentPanels[currentContentIndex].SetActive(false);

        // Move to next panel, wrapping around to the start if at the end
        currentContentIndex = (currentContentIndex + 1) % contentPanels.Length;

        // Show the new current panel
        contentPanels[currentContentIndex].SetActive(true);
    }
}