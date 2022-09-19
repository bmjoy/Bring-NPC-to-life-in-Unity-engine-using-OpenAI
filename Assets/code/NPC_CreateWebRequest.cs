using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
using System.Text;

public class NPC_CreateWebRequest : MonoBehaviour
{
    public TextMeshProUGUI InputSpeakText; //drag and drop element
    public readonly string UnityCompletionPost_URL = "https://api.openai.com/v1/completions";
    public string openaiKey;
    public Prompt initialPrompt;

    public string getInputTxt()
    {
       return InputSpeakText.text; //here the value is "a"
    }

    void Start()
    {
        var keys = ImportJson<Keys>("local.keys");
        openaiKey = keys.openaikey;

        initialPrompt = ImportJson<Prompt>("initial.prompt");
    }

    public void Talk()
    {
        StartCoroutine(SendPrompt(getInputTxt()));
    }

    private WWWForm CreatPromptCompletionForm(Prompt prompt)
    {
        WWWForm form = new WWWForm();
        form.AddField("model", prompt.model);
        form.AddField("prompt", prompt.prompt);
        form.AddField("temperature", prompt.temperature);
        form.AddField("max_tokens", prompt.max_tokens);
        form.AddField("top_p", prompt.top_p);
        form.AddField("frequency_penalty", prompt.frequency_penalty);
        form.AddField("presence_penalty", prompt.presence_penalty);
        //form.AddField("stop", JsonUtility.ToJson(prompt.stop));

        return form;
    }

    IEnumerator SendPrompt(string txtPromt)
    {
        Debug.Log(txtPromt);

        Debug.Log(UnityCompletionPost_URL);

        // convert our class to json
        string JsonData = JsonUtility.ToJson(initialPrompt);
        Debug.Log(JsonData);

        using (UnityWebRequest request = new UnityWebRequest(UnityCompletionPost_URL, "POST")) //form))
        {
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(JsonData);
            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

            // set your headers
            request.SetRequestHeader("Authorization", "Bearer " + openaiKey);
            request.SetRequestHeader("Content-Type", "application/json");


            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(request.error);
            }
            else
            {
                Debug.Log("Form upload complete!");
                Debug.Log(JsonUtility.ToJson(request.result));
            }
        }
    }

    public static T ImportJson<T>(string path)
    {
        TextAsset textAsset = Resources.Load<TextAsset>(path);
        Debug.Log(textAsset.text);
        return JsonUtility.FromJson<T>(textAsset.text);
    }

    [Serializable]
    public class Keys
    {
        public string openaikey;
    }

    [Serializable]
    public class Prompt
    {
        public string model;
        public string prompt;
        public int temperature;
        public int max_tokens;
        public int top_p;
        public int frequency_penalty;
        public int presence_penalty;
        //public string[] stop;
    }
}