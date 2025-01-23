using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAssistantIcon : MonoBehaviour
{
    [SerializeField] private GameObject aiAssistantIcon;
    [SerializeField] private GameObject colorCircle;
    [SerializeField] private Material red;
    [SerializeField] private Material yellow;
    [SerializeField] private Material green;
    [SerializeField] private Material white;

    [SerializeField] private float scaleDuration = 0.5f; // Duration for scaling
    [SerializeField] private Vector3 smallScale = Vector3.zero; // Scale when invisible
    [SerializeField] private Vector3 regularScale = Vector3.one; // Scale when visible
    [SerializeField] private float normalSpeed = 50f;
    [SerializeField] private float listeningSpeed = 100f;

    private Coroutine currentCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        // TODO: Find aiAssistantIcon in scene
        aiAssistantIcon = GameObject.Find("IrregularCircleQuad").gameObject;
        colorCircle = aiAssistantIcon.transform.Find("QuadColor").gameObject;

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
        aiAssistantIcon.GetComponent<CircleAnimation>().speed = normalSpeed;
        colorCircle.GetComponent<MeshRenderer>().material = white;
    }

    // Changes icon to "Listening" state
    public void Listening()
    {
        // Change to faster green icon
        aiAssistantIcon.GetComponent<CircleAnimation>().speed = listeningSpeed;
        colorCircle.GetComponent<MeshRenderer>().material = green;
    }

    // Changes icon to "Processing" state
    public void Processing()
    {
        // Change to stagnant yellow icon
        aiAssistantIcon.GetComponent<CircleAnimation>().speed = normalSpeed;
        colorCircle.GetComponent<MeshRenderer>().material = yellow;
    }

    // Changes icon to "Fixing" state
    public void Fixing()
    {
        // Change to stagnant red icon
        aiAssistantIcon.GetComponent<CircleAnimation>().speed = normalSpeed;
        colorCircle.GetComponent<MeshRenderer>().material = red;
    }
}
