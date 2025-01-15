using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TaskButtonPress : MonoBehaviour
{

    public GameObject backplate;
    private TextMeshPro id;
    private TextMeshPro pid;
    private int pid_val;
    private int id_val;
    // Start is called before the first frame update
    void Start()
    {
        id = transform.Find("ID").GetComponent<TextMeshPro>();
        pid = transform.Find("PID").GetComponent<TextMeshPro>();
        if (pid == null)
        {
            Debug.LogWarning("Main Task");
            id_val = int.Parse(id.text);
            pid_val = -1;
        }
        else
        {
            Debug.LogWarning("Main Task");
            id_val = int.Parse(id.text);
            pid_val = int.Parse(pid.text);
        }
    }

    public void OnPress()
    {
        EventBus.Publish(new TaskFinishedEvent(id_val, pid_val));
    }
}
