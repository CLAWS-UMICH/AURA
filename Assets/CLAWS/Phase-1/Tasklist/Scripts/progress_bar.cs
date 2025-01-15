using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class progress_bar : MonoBehaviour
{
    private TextMeshPro fraction;
    private GameObject pbar;
    private List<TaskObj> tasklist_bar;
    public float progress;
    public bool isSubtask = true;
    private Subscription<InitPopFinishedEvent> initEvent;

    void Start()
    {
        Debug.LogWarning("made it into start");
        fraction = transform.Find("Fraction").GetComponent<TextMeshPro>();
        //Debug.LogWarning("made it here 1");
        pbar = transform.Find("pb_background").transform.Find("pb_bar").gameObject;
        //Debug.LogWarning("made it here 2");
        pbar.transform.localScale = new Vector3(0, 1, 1);
        //Debug.LogWarning("made it here 3");
        initEvent = EventBus.Subscribe<InitPopFinishedEvent>(Init_Progress_bar);
        //Debug.LogWarning("subscribed");
    }

    public void Update_Progress_bar(int completed, int total)
    {
        progress = ((float)completed) / total;
        fraction.text = completed + "/" + total;
        pbar.transform.localPosition = new Vector3((1 - progress) * (-0.5f), pbar.transform.localPosition.y, pbar.transform.localPosition.z);
        pbar.transform.localScale = new Vector3(progress, 1, 1);
    }

    public void Init_Progress_bar(InitPopFinishedEvent e)
    {
        Debug.LogWarning("made it into Init PB");
        int comp = 0;
        int total = 0;
        tasklist_bar = e.tl;
        foreach(TaskObj t in tasklist_bar)
        {
            total += 1;
            if (t.status == 2)
            {
                comp += 1;
            }
            if (t.subtasks.Count > 0)
            {
                foreach (TaskObj subT in t.subtasks)
                {
                    total += 1;
                    if (subT.status == 2)
                    {
                        comp += 1;
                    }
                }
            }
        }
        Update_Progress_bar(comp, total);
    }   
    
}
