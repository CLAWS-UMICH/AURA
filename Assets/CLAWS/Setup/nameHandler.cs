using UnityEngine;
using System.Collections;
using MixedReality.Toolkit.Examples.Demos;


public class nameHandler : MonoBehaviour
{
    public DictationHandler dictationHandler;
    public GameObject targetGameObject;
    void Start()
    {
        dictationHandler.OnRecognitionFinished.AddListener(HandleRecognitionFinished);
    } 


    private void HandleRecognitionFinished(string reason)
    {
        // Deactivate the target GameObject
        if (targetGameObject != null)
        {
            targetGameObject.SetActive(false);
            Debug.Log("Target GameObject has been deactivated.");
        }
    }
};