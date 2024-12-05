using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TasklistBackend : MonoBehaviour
{
    // Start is called before the first frame update
    public static TasklistObj tasklist;

    public static TaskObj currentTask;

    private Subscription<TasklistEvent> taskListEvent;
    void Start()
    {
        taskListEvent = EventBus.Subscribe<TasklistEvent>(SetCurrentTask);
    }

    public void SetCurrentTask(TasklistEvent e)
    {
        if (e.use == "POST")
        {

        }
        if (e.use == "PUT")
        {

        }
        if (e.use == "DELETE")
        {

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
