using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using TMPro;

public class WebsocketTest : MonoBehaviour
{
    private Subscription<WebTestEvent> webTestEvent;
    private TextMeshPro text;

    // Start is called before the first frame update
    void Start()
    {
        text = gameObject.GetComponent<TextMeshPro>();
        text.text = "Initial";
        webTestEvent = EventBus.Subscribe<WebTestEvent>(onWebTest);
    }

    private void onWebTest(WebTestEvent e)
    {
        text.text = e.testData.num.ToString();
    }
}
