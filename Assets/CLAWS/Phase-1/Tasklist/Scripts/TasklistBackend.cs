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
        taskListEvent = EventBus.Subscribe<TasklistEvent>(UpdateTasklistWeb);
    }

    public void UpdateTasklistWeb(TasklistEvent e)
    {
        tasklist = e.taskdata;
        if (e.use == "POST")
        {
            foreach (TaskObj t in tasklist.Tasklist)
            {
                // puts the emergency task in the front of the tasklist
                if (t.isEmergency)
                {
                    AstronautInstance.User.tasklist.insert(0, t);
                }
                else
                {
                    AstronautInstance.User.tasklist.add(t);
                }
            }
        }
        if (e.use == "PUT")
        {
            foreach(TaskObj t in tasklist.Tasklist)
            {
                AstronautInstance.User.tasklist.update(t);
            }
        }
        if (e.use == "DELETE")
        {
            foreach (TaskObj target in tasklist.Tasklist)
            {
                AstronautInstance.User.tasklist.Tasklist.Remove(target); 
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
