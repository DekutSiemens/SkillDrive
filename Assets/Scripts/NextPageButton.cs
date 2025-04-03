using System;
using System.Collections.Generic;
using UnityEngine;
namespace Unity.VRTemplate
{
    /// <summary>
    /// Controls the steps in the in coaching card.
    /// </summary>
    public class NextPageButton : MonoBehaviour
    {
        [Serializable]
        class Step
        {
            [SerializeField]
            public GameObject stepObject;
        }

        [SerializeField]
        List<Step> m_StepList = new List<Step>();

        int m_CurrentStepIndex = 0;

        public void Next()
        {
            m_StepList[m_CurrentStepIndex].stepObject.SetActive(false);
            m_CurrentStepIndex = (m_CurrentStepIndex + 1) % m_StepList.Count;
            m_StepList[m_CurrentStepIndex].stepObject.SetActive(true);
        }

        /// <summary>
        /// Shows a specific step by its index.
        /// </summary>
        /// <param name="stepIndex">The index of the step to show</param>
        public void ShowStep(int stepIndex)
        {
            // First, hide the current step
            if (m_CurrentStepIndex >= 0 && m_CurrentStepIndex < m_StepList.Count)
            {
                m_StepList[m_CurrentStepIndex].stepObject.SetActive(false);
            }

            // Check if the requested step index is valid
            if (stepIndex >= 0 && stepIndex < m_StepList.Count)
            {
                // Update the current step index
                m_CurrentStepIndex = stepIndex;

                // Show the new step
                m_StepList[m_CurrentStepIndex].stepObject.SetActive(true);
            }
            else
            {
                Debug.LogWarning($"Invalid step index: {stepIndex}. Step index must be between 0 and {m_StepList.Count - 1}.");
            }
        }
    }
}