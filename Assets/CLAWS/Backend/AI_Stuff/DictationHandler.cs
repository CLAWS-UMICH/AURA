// Copyright (c) Mixed Reality Toolkit Contributors
// Licensed under the BSD 3-Clause

// Disable "missing XML comment" warning for samples. While nice to have, this XML documentation is not required for samples.
#pragma warning disable CS1591

using MixedReality.Toolkit.Subsystems;
using UnityEngine;
using UnityEngine.Events;
using SocketIOClient;
using System;
using System.Collections.Generic;
using System.Collections;
using Newtonsoft.Json;
using TMPro;

namespace MixedReality.Toolkit.Examples.Demos
{
    /// <summary>
    /// Demonstration script showing how to subscribe to and handle
    /// events fired by <see cref="DictationSubsystem"/>.
    /// </summary>
    
    [System.Serializable]
    public class ToolResponse
    {
        public string tool_name;
        public override string ToString()
        {
            return $"ToolResponse(tool_name: {tool_name})";
        }
    }
    [System.Serializable]
    public class AuraResponse
    {
        public string data;
    }
    [System.Serializable]
    public class ToolResponseWrapper
    {
        public ToolResponse[] responses;
    }

    [AddComponentMenu("MRTK/Examples/Dictation Handler")]
    public class DictationHandler : MonoBehaviour
    {
        /// <summary>
        /// Wrapper of UnityEvent&lt;string&gt; for serialization.
        /// </summary>
        [System.Serializable]
        public class StringUnityEvent : UnityEvent<string> { }

        /// <summary>
        /// Event raised while the user is talking. As the recognizer listens, it provides text of what it's heard so far.
        /// </summary>
        [field: SerializeField]
        public StringUnityEvent OnSpeechRecognizing { get; private set; }

        /// <summary>
        /// Event raised after the user pauses, typically at the end of a sentence. Contains the full recognized string so far.
        /// </summary>
        [field: SerializeField]
        public StringUnityEvent OnSpeechRecognized { get; private set; }

        /// <summary>
        /// Event raised when the recognizer stops. Contains the final recognized string.
        /// </summary>
        [field: SerializeField]
        public StringUnityEvent OnRecognitionFinished { get; private set; }

        /// <summary>
        /// Event raised when an error occurs. Contains the string representation of the error reason.
        /// </summary>
        [field: SerializeField]
        public StringUnityEvent OnRecognitionFaulted { get; private set; }

        private IDictationSubsystem dictationSubsystem = null;
        private IKeywordRecognitionSubsystem keywordRecognitionSubsystem = null;
        private string completeDictationResult = string.Empty;
        private float lastRecognizingTime;
        private bool isDictation = false;
        public SocketIOUnity socket;
        public float stopDictationTime = 1f;
        public AgentHandler agentHandler;
        public GameObject vitalsPanel;
        public TMP_Text aiStatusText;

        /// <summary>
        /// Start dictation on a DictationSubsystem.
        /// </summary>

        
        public void Awake(){
            var uri = new Uri("http://127.0.0.1:5001");
            socket = new SocketIOUnity(uri, new SocketIOOptions
            {
                Query = new Dictionary<string, string>
                    {
                        {"token", "UNITY" }
                    },
                EIO = 4,
                Transport = SocketIOClient.Transport.TransportProtocol.WebSocket
            });

            socket.OnConnected += (sender, e) =>
            {
                Debug.Log("socket.OnConnected");
            };
            socket.OnDisconnected += (sender, e) =>
            {
                Debug.Log("disconnect: " + e);
            };
            socket.Connect();

            Debug.Log("socket: " + socket.ToString());      
            socket.On("response", (data) => {
                var jsonObject = JsonConvert.DeserializeObject<AuraResponse[]>(data.ToString());
                UnityThread.executeInUpdate(() => {
                    aiStatusText.text = "AURA RESPONSE:\n" + jsonObject[0].data;
                });
                Debug.Log("AURA RESPONSE: " + jsonObject[0].data);
            });

            socket.On("tool_response", (response) => {
                // Extract the first item (index 0) and convert it to a string.
                string jsonResponse = response.ToString();
                var jsonObject = JsonConvert.DeserializeObject<ToolResponse[]>(jsonResponse);
                Debug.Log("toolresponse: " + jsonObject[0].tool_name);
                // var jsonObject = JsonSerializer.Serialize(jsonResponse);
                // Debug.Log("response: " + jsonObject);
                // Debug.Log("response type: " + response.GetValue(0));
                


                
                // Deserialize the json into your ToolResponse class.
                // ToolResponse[] toolResponseWrapper = response.GetValue<ToolResponse[]>(0);
                // Debug.Log("toolResponseWrapper: " + toolResponseWrapper);
                // try {
                //     Debug.Log("toolName: " + toolResponseWrapper[0].tool_name);
                // } catch (Exception e) {
                //     Debug.Log("Error: " + e.Message);
                // }
                
                if (jsonObject[0].tool_name == "ClickVitals")
                {
                    Debug.Log("ClickVitals");
                    UnityThread.executeInUpdate(() => {
                        aiStatusText.text = "Pulling Up Vitals";
                        vitalsPanel.SetActive(true);
                    });
                    socket.Emit("client_clicked_vitals", "ClickVitals");

                }
            });


            dictationSubsystem = XRSubsystemHelpers.DictationSubsystem;
            dictationSubsystem.Recognizing += DictationSubsystem_Recognizing;
            dictationSubsystem.Recognized += DictationSubsystem_Recognized;
            dictationSubsystem.RecognitionFinished += DictationSubsystem_RecognitionFinished;
            dictationSubsystem.RecognitionFaulted += DictationSubsystem_RecognitionFaulted;
        }

        // Update is called once per frame
        void OnDestroy()
        {
            socket.Disconnect();
        }

        public void StartRecognition()
        {
            // Make sure there isn't an ongoing recognition session
            StopRecognition();
            lastRecognizingTime = Time.time;
            aiStatusText.text = "Transcribing";
            print("dictationSubsystem is not null: " + (dictationSubsystem != null));

            if (dictationSubsystem == null)
            {
                dictationSubsystem = XRSubsystemHelpers.DictationSubsystem;
            }
            Debug.Log("starting dictation");
            keywordRecognitionSubsystem = XRSubsystemHelpers.KeywordRecognitionSubsystem;
            if (keywordRecognitionSubsystem != null)
            {
                keywordRecognitionSubsystem.Stop();
            }
            dictationSubsystem.StartDictation();
            isDictation = true;
            
        }

        private void DictationSubsystem_RecognitionFaulted(DictationSessionEventArgs obj)
        {
            OnRecognitionFaulted.Invoke("Recognition faulted. Reason: " + obj.ReasonString);
            HandleDictationShutdown();
        }

        private void DictationSubsystem_RecognitionFinished(DictationSessionEventArgs obj)
        {
            OnRecognitionFinished.Invoke("Recognition finished. Reason: " + obj.ReasonString);
        }

        private void DictationSubsystem_Recognized(DictationResultEventArgs obj)
        {
            
            completeDictationResult += obj.Result + " ";
            Debug.Log("Recognized:" + obj.Result);
            
            OnSpeechRecognized.Invoke("Recognized:" + obj.Result);
        }

        private void DictationSubsystem_Recognizing(DictationResultEventArgs obj)
        {
            lastRecognizingTime = Time.time;
            Debug.Log("Recognizing:" + lastRecognizingTime);
            OnSpeechRecognizing.Invoke("Recognizing:" + obj.Result);
        }

        IEnumerator RestartKeywordRecognizer() {
            // Wait one frame or a small duration to let the dictation subsystem shut down.
            yield return new WaitForSeconds(0.5f);
            if (keywordRecognitionSubsystem == null)
            {
                keywordRecognitionSubsystem = XRSubsystemHelpers.KeywordRecognitionSubsystem;
            }
            keywordRecognitionSubsystem.Start();
        }

        void Update() {
            if (isDictation && Time.time - lastRecognizingTime > stopDictationTime) {
                Debug.Log("AI STOP DICTATION");
                StopRecognition();
                SendMessageToAI();
                StartCoroutine(RestartKeywordRecognizer());
            }
        }

        /// <summary>
        /// Stop dictation on the current DictationSubsystem.
        /// </summary>
        public void SendMessageToAI(){
            Debug.Log("Complete dictation result: " + completeDictationResult);
            if (completeDictationResult != string.Empty)
            {
                aiStatusText.text = "Asking AI";
                socket.Emit("message", completeDictationResult);
            }
            completeDictationResult = string.Empty;
        }
        public void StopRecognition()
        {
            if (dictationSubsystem != null)
            {
                dictationSubsystem.StopDictation();
            }
            isDictation = false;
        }

        /// <summary>
        /// Stop dictation on the current DictationSubsystem.
        /// </summary>
        public void HandleDictationShutdown()
        {            

            if (dictationSubsystem != null)
            {
                dictationSubsystem.Recognizing -= DictationSubsystem_Recognizing;
                dictationSubsystem.Recognized -= DictationSubsystem_Recognized;
                dictationSubsystem.RecognitionFinished -= DictationSubsystem_RecognitionFinished;
                dictationSubsystem.RecognitionFaulted -= DictationSubsystem_RecognitionFaulted;
                dictationSubsystem = null;
            }

            if (keywordRecognitionSubsystem != null)
            {
                keywordRecognitionSubsystem.Start();
                keywordRecognitionSubsystem = null;
            }

           
        }
    }
}
#pragma warning restore CS1591