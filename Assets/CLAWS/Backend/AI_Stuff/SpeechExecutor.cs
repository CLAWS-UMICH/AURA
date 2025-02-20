using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeechCommandExecutor : MonoBehaviour
{
    public void DoMoveDown()
    {
        Debug.Log("DoMoveDown");
        transform.position -= Vector3.up * 0.1f;
    }

    public void DoMoveUp()
    {
        Debug.Log("DoMoveUp");
        transform.position += Vector3.up * 10f;

    }
}
