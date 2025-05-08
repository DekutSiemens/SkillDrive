using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.VRTemplate
{
    /// <summary>
    /// Enhanced Callout that supports multiple curves (e.g., multiple start points -> one tooltip).
    /// </summary>
    public class MultiCurveCallout : MonoBehaviour
    {
        [SerializeField, Tooltip("Whether Gaze Callout is used.")]
        bool m_UseGazeCallout = true;

        [SerializeField]
        [Tooltip("The tooltip Transform associated with this Callout.")]
        Transform m_LazyTooltip;

        [SerializeField]
        [Tooltip("List of curve GameObjects (one per interactable).")]
        List<GameObject> m_Curves = new List<GameObject>();

        [SerializeField]
        [Tooltip("The required time to dwell on this callout before the tooltip and curves are enabled.")]
        float m_DwellTime = 1f;

        [SerializeField]
        [Tooltip("Whether the associated tooltip will be unparented on Start.")]
        bool m_Unparent = true;

        [SerializeField]
        [Tooltip("Whether the associated tooltip and curves will be disabled on Start.")]
        bool m_TurnOffAtStart = true;

        bool m_Gazing = false;
        Coroutine m_StartCo;
        Coroutine m_EndCo;

        void Start()
        {
            if (!m_UseGazeCallout)
            {
                DisableCallout();
                return;
            }

            if (m_Unparent && m_LazyTooltip != null)
                m_LazyTooltip.SetParent(null);

            if (m_TurnOffAtStart)
            {
                if (m_LazyTooltip != null)
                    m_LazyTooltip.gameObject.SetActive(false);
                foreach (var curve in m_Curves)
                    curve.SetActive(false);
            }
        }

        public void GazeHoverStart(int curveIndex = 0)
        {
            if (!m_UseGazeCallout)
            {
                DisableCallout();
                return;
            }

            m_Gazing = true;
            if (m_StartCo != null)
                StopCoroutine(m_StartCo);
            if (m_EndCo != null)
                StopCoroutine(m_EndCo);
            m_StartCo = StartCoroutine(StartDelay(curveIndex));
        }

        public void GazeHoverEnd()
        {
            if (!m_UseGazeCallout)
            {
                DisableCallout();
                return;
            }

            m_Gazing = false;
            m_EndCo = StartCoroutine(EndDelay());
        }

        IEnumerator StartDelay(int curveIndex)
        {
            yield return new WaitForSeconds(m_DwellTime);
            if (m_Gazing)
                TurnOnStuff(curveIndex);
        }

        IEnumerator EndDelay()
        {
            if (!m_Gazing)
                TurnOffStuff();
            yield return null;
        }

        void TurnOnStuff(int curveIndex)
        {
            if (m_LazyTooltip != null)
                m_LazyTooltip.gameObject.SetActive(true);

            // Activate only the specified curve (or all if you prefer)
            if (curveIndex >= 0 && curveIndex < m_Curves.Count && m_Curves[curveIndex] != null)
                m_Curves[curveIndex].SetActive(true);
        }

        void TurnOffStuff()
        {
            if (m_LazyTooltip != null)
                m_LazyTooltip.gameObject.SetActive(false);
            foreach (var curve in m_Curves)
                curve.SetActive(false);
        }

        void DisableCallout()
        {
            if (m_StartCo != null)
                StopCoroutine(m_StartCo);
            if (m_EndCo != null)
                StopCoroutine(m_EndCo);
            TurnOffStuff();
        }
    }
}