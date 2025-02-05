using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TaskManager : MonoBehaviour
{
    public GameObject[] taskPrompts; // Array of GameObjects representing task prompts
    public GameObject moreTasksText; // GameObject for "+X more tasks"
    public GameObject loadingIcon; // Loading icon GameObject

    private Queue<string> tasks = new Queue<string>();
    private bool isProcessing = false;

    void Start()
    {
        // Validate components
        if (taskPrompts == null || taskPrompts.Length == 0)
        {
            Debug.LogError("Task prompts array not set!");
            return;
        }

        if (moreTasksText == null)
        {
            Debug.LogError("More tasks text reference not set!");
            return;
        }

        if (loadingIcon == null)
        {
            Debug.LogError("Loading icon reference not set!");
            return;
        }

        // Initialize tasks
        AddTask("Finding Waypoint");
        AddTask("Open Navigation");
        AddTask("Start Navigation");
        AddTask("Kill Astronaut");
    }

    public void AddTask(string task)
    {
        tasks.Enqueue(task);
        UpdateUI();
        if (!isProcessing)
        {
            StartCoroutine(ProcessTasks());
        }
    }

    private void UpdateUI()
    {
        for (int i = 0; i < taskPrompts.Length; i++)
        {
            if (tasks.Count > i)
            {
                string taskText = tasks.ToArray()[i];
                if (i == 0 && tasks.Count > 1)
                {
                    taskText += " ...";
                }

                // Get the TaskText component within the current taskPrompt
                TextMeshPro taskTextComponent = taskPrompts[i].transform
                    .Find("TaskText")
                    .GetComponent<TextMeshPro>();
                    
                if (taskTextComponent != null)
                {
                    taskTextComponent.text = taskText;
                }

                taskPrompts[i].SetActive(true);
            }
            else
            {
                taskPrompts[i].SetActive(false);
            }
        }

        // Update more tasks counter
        if (tasks.Count > taskPrompts.Length)
        {
            moreTasksText.SetActive(true);
            TextMeshProUGUI moreTasksTextComponent = moreTasksText
                .GetComponent<TextMeshProUGUI>();
            if (moreTasksTextComponent != null)
            {
                moreTasksTextComponent.text = $"+{tasks.Count - taskPrompts.Length} more tasks";
            }
        }
        else
        {
            moreTasksText.SetActive(false);
        }
    }

    private IEnumerator ProcessTasks()
    {
        isProcessing = true;
        while (tasks.Count > 0)
        {
            yield return new WaitForSeconds(5); // Wait for 5 seconds before completing the task
            tasks.Dequeue();
            UpdateUI();
            StartCoroutine(ShowLoadingIcon());
        }
        isProcessing = false;
    }

    private IEnumerator ShowLoadingIcon()
    {
        loadingIcon.SetActive(true);
        yield return new WaitForSeconds(1); // Adjust the delay as needed
        UpdateUI();
    }
}