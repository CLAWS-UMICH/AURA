using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class progress_bar : MonoBehaviour
{
    private TextMeshPro fraction;
    private GameObject pbar;
    //private List<TaskObj> tasklist_bar;
    public float progress;
    //private Subscription<InitPopFinishedEvent> initEventPB;
    private Subscription<ProgressBarUpdateEvent> updatePBEvent;


    private void Start()
    {
        Debug.LogWarning("made it into start");
        fraction = transform.Find("Fraction").GetComponent<TextMeshPro>();
        pbar = transform.Find("pb_background").transform.Find("pb_bar").gameObject;
        Debug.LogWarning("did we find fraction ? " + (fraction == null) + " and did we find pbar ? " + (pbar == null));
        pbar.transform.localScale = new Vector3(0, 1, 1);
        //Debug.LogWarning("about to subscribe");
        //Debug.LogWarning("subscribed to events");
        //initEventPB = EventBus.Subscribe<InitPopFinishedEvent>(Init_Progress_bar);
        updatePBEvent = EventBus.Subscribe<ProgressBarUpdateEvent>(Update_caller);
    }

    private void Update_caller(ProgressBarUpdateEvent e)
    {
        Update_Progress_bar(e.comp, e.total);
    }
    public void Update_Progress_bar(int completed, int total)
    {
        progress = ((float)completed) / total;
        fraction.text = completed + "/" + total;
        pbar.transform.localPosition = new Vector3((1 - progress) * (-0.5f), pbar.transform.localPosition.y, pbar.transform.localPosition.z);
        pbar.transform.localScale = new Vector3(progress, 1, 1);
    }

    private void Init_Progress_bar(InitPopFinishedEvent e)
    {
        Debug.LogWarning("This subscription works progress bar");
        int comp = 0;
        //tasklist_bar = e.tl;
        int total = e.tl.Count;
        foreach(TaskObj t in e.tl)
        {
            if (t.status == 2)
            {
                comp += 1;
            }
            /*
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
            */
        }
        Debug.LogWarning("About to update");
        Update_Progress_bar(comp, total);
    }   
    
}
