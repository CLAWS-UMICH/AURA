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
    void Update()
    {
        id = transform.Find("ID").GetComponent<TextMeshPro>();
        if (transform.Find("PID") == null)
        {
            // Debug.LogWarning("Main Task");
            id_val = int.Parse(id.text);
            pid_val = -1;
        }
        else
        {
            // Debug.LogWarning("Sub Task");
            id_val = int.Parse(id.text);
            pid = transform.Find("PID").GetComponent<TextMeshPro>();
            pid_val = int.Parse(pid.text);
        }
    }

    public void OnPress()
    {
        Debug.LogWarning("ID " + id_val + " PID " + pid_val);
        EventBus.Publish(new TaskFinishedEvent(id_val, pid_val));
        Debug.LogWarning("New Task Finished");
    }
}
