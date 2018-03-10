using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Networking;
using RestSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Text;

[System.Serializable]
public class SpeechRecognizedEvent : UnityEvent<String>
{

}

public class SpeechRecognizer : MonoBehaviour {

    public TextAsset apiKeyFile;
    public int recordLength;
    [SerializeField]
    public SpeechRecognizedEvent onSpeechRecognized;
    private bool hasMic;
    private int minFreq;
    private int maxFreq;
    private AudioClip clip;

    // Use this for initialization
    void Start()
    {
        if (Microphone.devices.Length > 0)
        {
            hasMic = true;
            Microphone.GetDeviceCaps(null, out minFreq, out maxFreq);
            if (minFreq == 0 && maxFreq == 0)
            {
                maxFreq = 44100;
            }
        }
    }

    public void Record()
    {
        if (!hasMic)
        {
            Debug.Log("No microphone present");
            return;
        }

        clip = Microphone.Start(null, false, recordLength, maxFreq);
    }

    public void Finish()
    {
        if (!hasMic)
        {
            Debug.Log("No microphone present");
            return;
        }

        if (clip == null)
        {
            Debug.Log("No clip");
            return;
        }

        Microphone.End(null);

        var trimmed = SavWav.TrimSilence(clip, 0.001f);

        var stream = new MemoryStream();
        SavWav.ConvertAndWrite(stream, trimmed);
        SavWav.WriteHeader(stream, trimmed);
        var bytes = stream.ToArray();
        var result = DecodeSpeech(bytes);
        Debug.Log(result);
        onSpeechRecognized.Invoke(result);

        clip = null;
    }

    private string DecodeSpeech(byte[] wavSpeech)
    {
        var speechRequest = new JObject(
            new JProperty("audio", new JObject(
                new JProperty("content", Convert.ToBase64String(wavSpeech))
            )),
            new JProperty("config", new JObject(
                new JProperty("languageCode", new JValue("fi"))
            ))
        );

        var content = speechRequest.ToString();

        var client = new RestClient("https://speech.googleapis.com/");
        var req = new RestRequest("v1/speech:recognize", Method.POST);
        req.AddParameter("key", apiKeyFile.text, ParameterType.QueryString);
        req.AddParameter("application/json", Encoding.UTF8.GetBytes(content), ParameterType.RequestBody);

        var resp = client.Execute(req);
        Debug.Log(resp.ErrorMessage);
        Debug.Log(resp.ResponseStatus);
        Debug.Log(resp.Content);

        var speechResponse = JObject.Parse(resp.Content);
        var results = (JArray)speechResponse["results"];
        if (results != null && results.Count > 0)
        {
            return (string)results[0]["alternatives"][0]["transcript"];
        }

        return "";
    }
}
