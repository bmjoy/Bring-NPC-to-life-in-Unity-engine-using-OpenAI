using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using TMPro;

public class NPC_CreateWebRequest : MonoBehaviour
{
    public string txtPromt;
    public TextMeshProUGUI InputSpeakText; //drag and drop element

    public void getValue()
    {
       txtPromt = InputSpeakText.text; //here the value is "a"
    }

    void Start()
    {
       
    }

    public void Talk()
    {
        getValue();
        StartCoroutine(SendPrompt());
    }
    
    IEnumerator SendPrompt()
    {
        Debug.Log(txtPromt);
        WWWForm form = new WWWForm();
        form.AddField("myField", "myData");

        using (UnityWebRequest www = UnityWebRequest.Post("https://www.my-server.com/myform", form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Form upload complete!");
            }
        }
    }
}