using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAssistantIcon : MonoBehaviour
{
    [SerializeField] private GameObject aiAssistantIcon;
    [SerializeField] private GameObject redCircle;
    [SerializeField] private GameObject blueCircle;
    [SerializeField] private GameObject yellowCircle;
    [SerializeField] private GameObject greenCircle;
    [SerializeField] private GameObject whiteCircle;

    [SerializeField] private float scaleDuration = 0.5f; // Duration for scaling
    [SerializeField] private Vector3 smallScale = Vector3.zero; // Scale when invisible
    [SerializeField] private Vector3 regularScale = Vector3.one; // Scale when visible

    private Coroutine currentCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        // TODO: Find aiAssistantIcon in scene
        aiAssistantIcon = GameObject.Find("IrregularCircleQuad").gameObject;
        redCircle = aiAssistantIcon.transform.Find("QuadRed").gameObject;
        blueCircle = aiAssistantIcon.transform.Find("QuadBlue").gameObject;
        yellowCircle = aiAssistantIcon.transform.Find("QuadYellow").gameObject;
        greenCircle = aiAssistantIcon.transform.Find("QuadGreen").gameObject;
        whiteCircle = aiAssistantIcon.transform.Find("QuadWhite").gameObject;

        // Initialize the icon as invisible and at small scale
        aiAssistantIcon.transform.localScale = smallScale;
        aiAssistantIcon.SetActive(false);
    }

    public void ToggleVoiceAssistant(bool isOn)
    {
        // Stop any existing scaling coroutine
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        // Start scaling coroutine based on the state
        currentCoroutine = StartCoroutine(ScaleIcon(isOn));
    }

    private IEnumerator ScaleIcon(bool isVisible)
    {
        if (isVisible)
        {
            // Make the icon active before scaling up
            aiAssistantIcon.SetActive(true);
        }

        Vector3 startScale = aiAssistantIcon.transform.localScale;
        Vector3 targetScale = isVisible ? regularScale : smallScale;
        float elapsedTime = 0f;

        // Smoothly scale the icon
        while (elapsedTime < scaleDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / scaleDuration;

            // Apply scaling
            aiAssistantIcon.transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }

        // Ensure the final scale is set
        aiAssistantIcon.transform.localScale = targetScale;

        if (!isVisible)
        {
            // Disable the icon after scaling down
            aiAssistantIcon.SetActive(false);
        }
    }

    // Changes icon to "Default / VEGA Speaking" state
    public void Speaking()
    {
        // Change to white icon
        aiAssistantIcon.GetComponent<CircleAnimation>().speed = 50f;
        redCircle.SetActive(true);
        blueCircle.SetActive(true);
        yellowCircle.SetActive(true);
        greenCircle.SetActive(true);
        whiteCircle.SetActive(true);
    }

    // Changes icon to "Listening" state
    public void Listening()
    {
        // Change to faster green icon
        aiAssistantIcon.GetComponent<CircleAnimation>().speed = 100f;
        redCircle.SetActive(false);
        blueCircle.SetActive(false);
        yellowCircle.SetActive(false);
        greenCircle.SetActive(true);
        whiteCircle.SetActive(true);
    }

    // Changes icon to "Processing" state
    public void Processing()
    {
        // Change to stagnant yellow icon
        aiAssistantIcon.GetComponent<CircleAnimation>().speed = 50f;
        redCircle.SetActive(false);
        blueCircle.SetActive(false);
        yellowCircle.SetActive(true);
        greenCircle.SetActive(false);
        whiteCircle.SetActive(true);
    }

    // Changes icon to "Fixing" state
    public void Fixing()
    {
        // Change to stagnant red icon
        aiAssistantIcon.GetComponent<CircleAnimation>().speed = 50f;
        redCircle.SetActive(true);
        blueCircle.SetActive(false);
        yellowCircle.SetActive(false);
        greenCircle.SetActive(false);
        whiteCircle.SetActive(true);
    }
}
