using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HierarchicalTabManager : MonoBehaviour
{
    [System.Serializable]
    public class TabNode
    {
        [Tooltip("The button for this tab")]
        public Button button;

        [Tooltip("The associated content panel for this tab")]
        public GameObject contentPanel;

        [Tooltip("Child tabs nested under this tab")]
        public List<TabNode> childTabs = new List<TabNode>();

        [Tooltip("Parent tab of this node")]
        public TabNode parentTab;
    }

    [Tooltip("Root-level tabs in the UI hierarchy")]
    public List<TabNode> rootTabs = new List<TabNode>();

    private void Start()
    {
        // Recursively set up hierarchy and listeners
        SetupTabHierarchy(rootTabs, null);
    }

    private void SetupTabHierarchy(List<TabNode> tabs, TabNode parentNode)
    {
        foreach (TabNode tabNode in tabs)
        {
            // Set parent reference
            tabNode.parentTab = parentNode;

            if (tabNode.button != null)
            {
                // Add listener to this tab's button
                tabNode.button.onClick.AddListener(() => OnTabSelected(tabNode));

                // Recursively set up child tabs
                if (tabNode.childTabs.Count > 0)
                {
                    SetupTabHierarchy(tabNode.childTabs, tabNode);
                }
            }
        }
    }

    private void OnTabSelected(TabNode selectedTab)
    {
        // Deactivate all content panels first
        DeactivateAllContentPanels(rootTabs);

        // Activate the selected tab and its content panel
        ActivateTabPath(selectedTab);
    }

    private void DeactivateAllContentPanels(List<TabNode> tabs)
    {
        foreach (TabNode tab in tabs)
        {
            // Deactivate this tab's content panel
            if (tab.contentPanel != null)
            {
                tab.contentPanel.SetActive(false);
            }

            // Recursively deactivate child tabs' content panels
            if (tab.childTabs.Count > 0)
            {
                DeactivateAllContentPanels(tab.childTabs);
            }
        }
    }

    private void ActivateTabPath(TabNode selectedTab)
    {
        TabNode currentTab = selectedTab;
        while (currentTab != null)
        {
            // Activate content panel
            if (currentTab.contentPanel != null)
            {
                currentTab.contentPanel.SetActive(true);
            }

            // Visually select the button (assuming Toggle or custom selection method)
            if (currentTab.button != null)
            {
                // You might need to customize this based on your specific UI setup
                Toggle toggleComponent = currentTab.button.GetComponent<Toggle>();
                if (toggleComponent != null)
                {
                    toggleComponent.isOn = true;
                }
            }

            // Move to parent
            currentTab = currentTab.parentTab;
        }
    }

    // Method to add a new tab node dynamically
    public TabNode AddTabNode(TabNode parentNode, Button newButton, GameObject contentPanel = null)
    {
        TabNode newTabNode = new TabNode
        {
            button = newButton,
            contentPanel = contentPanel,
            parentTab = parentNode
        };

        if (parentNode != null)
        {
            parentNode.childTabs.Add(newTabNode);
        }
        else
        {
            // If no parent, add to root tabs
            rootTabs.Add(newTabNode);
        }

        // Set up listener for the new button
        if (newButton != null)
        {
            newButton.onClick.AddListener(() => OnTabSelected(newTabNode));
        }

        return newTabNode;
    }

    // Debugging method to print tab hierarchy
    public void PrintTabHierarchy()
    {
        Debug.Log("Tab Hierarchy:");
        PrintTabNodes(rootTabs, 0);
    }

    private void PrintTabNodes(List<TabNode> tabs, int depth)
    {
        foreach (TabNode tab in tabs)
        {
            string indent = new string('-', depth);
            Debug.Log($"{indent} {tab.button.name}");

            if (tab.childTabs.Count > 0)
            {
                PrintTabNodes(tab.childTabs, depth + 1);
            }
        }
    }
}