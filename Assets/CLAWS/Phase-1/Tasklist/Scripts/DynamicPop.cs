using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DynamicPop : MonoBehaviour
{
    private Dictionary<(string, bool), string> prefabNames; // 
    private List<TaskObj> localTL;
    public GameObject[] prefabs; // types of messages (a1, lmcc a2)
    public Transform contentParent;
    
    // additions for new TL
    private Subscription<TaskFinishedEvent> finishedEvent;
    private List<Transform> Objects = new List<Transform>();

    void Start()
    {
        InitializePrefabTypes();
        PopulateContent();
        finishedEvent = EventBus.Subscribe<TaskFinishedEvent>(UpdateContent);
    }

    void InitializePrefabTypes()
    {

        localTL = new List<TaskObj>
        {
            new TaskObj(0, 0, "Drive the Rover", "Going for a drive in the rover innit", "ROVER", false, false, false, "BASE", new List<int>{}, new List<TaskObj> {
                new TaskObj(1, 0, "Do a donut", "rrrrrrr", "SUB_NORMAL", false, false, true, "BASE", new List<int>{}, new List<TaskObj> { }), 
            }),
            new TaskObj(2, 0, "Check the vitals", "Healthcare cuh", "VITALS", false, false, false, "BASE", new List<int>{}, new List<TaskObj> { }),
            new TaskObj(3, 0, "Fix the Rover", "Fixit felix", "REPAIR", false, false, false, "BASE", new List<int>{}, new List<TaskObj> { }),
            new TaskObj(4, 0, "Walk to Site A", "Strolling around", "NAVIGATION", false, false, false, "OUT", new List<int>{}, new List<TaskObj> { }),
            new TaskObj(5, 0, "Identify Sample", "Dwanye Johnson", "GEOSAMPLING", false, false, false, "OUT", new List<int>{}, new List<TaskObj> { }),
            new TaskObj(6, 0, "Rescue Steve", "Healthcare", "EMERGENCY", true, false, false, "BASE", new List<int>{}, new List<TaskObj> {
                new TaskObj(7, 0, "Do CPR", "Ba bump", "SUB_EMERGENCY", true, false, true, "BASE", new List<int>{}, new List<TaskObj> { })
            }),
        };


        // mapping types and shared status to names of prefabs for generation
        prefabNames = new Dictionary<(string, bool), string>();
        prefabNames[("ROVER", false)] = "TaskPrefab (rover solo)";
        prefabNames[("ROVER", true)] = "TaskPrefab (rover shared)";
        prefabNames[("VITALS", false)] = "TaskPrefab (vitals solo)";
        prefabNames[("VITALS", true)] = "TaskPrefab (vitals shared)";
        prefabNames[("REPAIR", false)] = "TaskPrefab (repair solo)";
        prefabNames[("REPAIR", true)] = "TaskPrefab (repair shared)";
        prefabNames[("NAVIGATION", false)] = "TaskPrefab (nav solo)";
        prefabNames[("NAVIGATION", true)] = "TaskPrefab (nav shared)";
        prefabNames[("GEOSAMPLING", false)] = "TaskPrefab (geo solo)";
        prefabNames[("GEOSAMPLING", true)] = "TaskPrefab (geo shared)";
        prefabNames[("EMERGENCY", false)] = "TaskPrefab (emergency solo)";
        prefabNames[("EMERGENCY", true)] = "TaskPrefab (emergency shared)";
        prefabNames[("SUB_EMERGENCY", false)] = "SubaskPrefab (emergency)";
        prefabNames[("SUB_NORMAL", false)] = "SubaskPrefab (normal)";
    }

    private void UpdateContent(TaskFinishedEvent e)
    {
        int id = e.id;
        int pid = e.pid;
        bool skip_sub = (pid == -1);
        int comp = 0;
        int total = localTL.Count;
        // change localTL to (AstronautInstance.User.tasklist.Tasklist) when running with web tests
        // backend update
        foreach (TaskObj task in localTL)
        {
            if (task.task_id == id && task.status != 2)
            {
                task.status = 2;
            }
            if (!skip_sub && task.subtasks.Count > 0)
            {
                foreach (TaskObj subtask in task.subtasks) 
                {
                    if (subtask.task_id == id && subtask.status != 2)
                    {
                        subtask.status = 2;
                        task.addCom();
                    }
                }
            }
            if (task.getComSub() == task.getNumSub() && task.getNumSub() != -1 )
            {
                task.status = 2;
            }
            if (task.status == 2)
            {
                Debug.Log("task name " + task.title);
                Debug.LogWarning(comp);
                comp += 1;
            }
        }
        //Debug.LogWarning("comp" + comp);
        EventBus.Publish(new ProgressBarUpdateEvent(comp, total));
    }

    void CollectAllButtons()
    {
        Transform parentTransform = transform;
        Objects.Clear();

        foreach (Transform child in parentTransform)
        {
            if (child.name != "Grid Layout")
            {
                Objects.Add(child);
            }
        }
    }

    void PopulateContent()
    {
        // change localTL to (AstronautInstance.User.tasklist.Tasklist) when running with web tests
        int comp = 0;
        int total = localTL.Count;
        foreach (TaskObj task in localTL)
        {
            // Find the corresponding prefab for the type
            GameObject prefab = GetPrefabByType(prefabNames[(task.taskType, task.isShared)]);
            if (prefab != null)
            {
                // Instantiate the prefab and add it to the Content area
                GameObject newTask = Instantiate(prefab, contentParent);
                newTask.transform.localPosition = new Vector3(contentParent.localPosition.x, contentParent.localPosition.y, -0.01f);
                newTask.transform.localScale = new Vector3(3.5f, 3.5f, 3.5f);
                newTask.transform.Find("UX.Slate.ContentBackplate").transform.Find("Title").GetComponent<TextMeshPro>().text = task.title;
                newTask.transform.Find("UX.Slate.ContentBackplate").transform.Find("Description").GetComponent<TextMeshPro>().text = task.description;
                newTask.transform.Find("UX.Slate.ContentBackplate").transform.Find("ID").GetComponent<TextMeshPro>().text = task.task_id.ToString();
                newTask.transform.Find("UX.Slate.ContentBackplate").transform.Find("NumSubs").GetComponent<TextMeshPro>().text = task.subtasks.Count.ToString();
                if (task.subtasks.Count > 0)
                {
                    newTask.transform.Find("UX.Slate.ContentBackplate").transform.Find("ComSubs").GetComponent<TextMeshPro>().text = 0.ToString();
                }
                if (task.isShared)
                {
                    Debug.LogWarning("Shared, logic later!");
                }
                if (task.subtasks.Count > 0)
                {
                    foreach(TaskObj subtask in task.subtasks)
                    {
                        GameObject sub_prefab = GetPrefabByType(prefabNames[(subtask.taskType, false)]);
                        if (sub_prefab != null)
                        {
                            GameObject newSub = Instantiate(sub_prefab, contentParent);
                            newSub.transform.localPosition = new Vector3(contentParent.localPosition.x, contentParent.localPosition.y, -0.01f);
                            newSub.transform.localScale = new Vector3(3.5f, 3.5f, 3.5f);
                            newSub.transform.Find("UX.Slate.ContentBackplate").transform.Find("Title").GetComponent<TextMeshPro>().text = subtask.title;
                            newSub.transform.Find("UX.Slate.ContentBackplate").transform.Find("Description").GetComponent<TextMeshPro>().text = subtask.description;
                            newSub.transform.Find("UX.Slate.ContentBackplate").transform.Find("ID").GetComponent<TextMeshPro>().text = subtask.task_id.ToString();
                            newSub.transform.Find("UX.Slate.ContentBackplate").transform.Find("PID").GetComponent<TextMeshPro>().text = task.task_id.ToString();
                        }
                        else
                        {
                            Debug.LogWarning($"Sub_Prefab not found!");
                        }
                    }
                }
            }
            else
            {
                Debug.LogWarning($"Prefab not found!");
            }
        }
        EventBus.Publish(new ProgressBarUpdateEvent(comp, total));
        EventBus.Publish(new InitPopFinishedEvent(localTL));
        Debug.LogWarning("Finished Init Population");
    }

    GameObject GetPrefabByType(string type)
    {
        // Find the prefab that matches the type
        foreach (GameObject prefab in prefabs)
        {
            if (prefab.name == type)
            {
                return prefab;
            }
        }
        // Return null if no matching prefab is found
        return null; 
    }
}
