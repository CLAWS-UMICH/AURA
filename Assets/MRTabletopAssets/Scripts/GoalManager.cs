using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class GoalManager : MonoBehaviour
{
    [Serializable]
    struct Step
    {
        public GameObject stepObject;
        public string buttonText;
        public bool includeSkipButton;
        public float continueButtonDelayTime;
    }

    [SerializeField]
    List<Step> m_StepList = new List<Step>();

    [SerializeField]
    TextMeshProUGUI m_StepButtonTextField;

    [SerializeField]
    Button m_SkipButton;

    [SerializeField]
    GameObject m_CoachingUIParent;

    [SerializeField]
    UnityEngine.XR.Interaction.Toolkit.UI.LazyFollow m_GoalPanelLazyFollow;

    [SerializeField]
    Button m_StepButton;

    [SerializeField] UnityEvent m_OnGoalCompleted;

    private int currentStepIndex = 0;

    void Start()
    {
        m_StepButton.onClick.AddListener(OnStepButtonPressed);
        m_SkipButton.onClick.AddListener(CompleteGoal);
        UpdateStepUI();
    }

    public void OnStepButtonPressed()
    {
        if (currentStepIndex < m_StepList.Count - 1)
        {
            m_StepList[currentStepIndex].stepObject.SetActive(false);
            currentStepIndex++;
            UpdateStepUI();
        }
        else
        {
            CompleteGoal();
        }
    }

    public void OnSkipButtonPressed()
    {
        CompleteGoal();
    }

    private void UpdateStepUI()
    {
        if (currentStepIndex < m_StepList.Count)
        {
            m_StepList[currentStepIndex].stepObject.SetActive(true);
            m_StepButtonTextField.text = m_StepList[currentStepIndex].buttonText;
            m_SkipButton.gameObject.SetActive(m_StepList[currentStepIndex].includeSkipButton);
            m_StepButton.interactable = false;
            StartCoroutine(DisableContinueButtonForTime(m_StepList[currentStepIndex].continueButtonDelayTime));
        }
    }

    IEnumerator DisableContinueButtonForTime(float time)
    {
        yield return new WaitForSeconds(time);
        m_StepButton.interactable = true;
    }

    void CompleteGoal()
    {
        m_OnGoalCompleted.Invoke();
        m_CoachingUIParent.SetActive(false);
        m_SkipButton.gameObject.SetActive(false);
    }
}
