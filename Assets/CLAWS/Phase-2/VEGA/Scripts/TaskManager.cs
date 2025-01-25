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
        // Initialize with some tasks
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
                Debug.Log("Task Text is: " + taskText);
                if (i == 0 && tasks.Count > 1) // Check if it's the first task and there are more tasks
                {
                    taskText += " ..."; // Append "..." to the first task
                }

                // Get the TextMeshPro component from the task prompt GameObject
                TextMeshPro taskTextComponent = GameObject.Find("TaskText").GetComponent<TextMeshPro>();
                taskTextComponent.text = taskText;


                taskPrompts[i].SetActive(true);
            }
            else
            {
                taskPrompts[i].SetActive(false);
            }
        }

        if (tasks.Count > taskPrompts.Length)
        {
            TextMeshProUGUI moreTasksTextComponent = moreTasksText.GetComponentInChildren<TextMeshProUGUI>();
            if (moreTasksTextComponent != null)
            {
                moreTasksTextComponent.text = $"+{tasks.Count - taskPrompts.Length} more tasks";
            }
            moreTasksText.SetActive(true);
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