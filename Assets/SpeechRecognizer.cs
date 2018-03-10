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
        StartCoroutine(DecodeSpeech(bytes));
        clip = null;
    }

    private WWW doPost(string path, string json)
    {
        string URL = path + "?key=" + apiKeyFile.text;

        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("Content-Type", "application/json");
        byte[] postData = System.Text.Encoding.UTF8.GetBytes(json);

        return new WWW(URL, postData, headers);
    }

    private IEnumerator DecodeSpeech(byte[] wavSpeech)
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

        using (WWW www = doPost("https://speech.googleapis.com/v1/speech:recognize", content))
        {
            yield return www;

            var speechResponse = JObject.Parse(www.text);
            var results = (JArray)speechResponse["results"];
            if (results != null && results.Count > 0)
            {
                string val = (string)results[0]["alternatives"][0]["transcript"];
                onSpeechRecognized.Invoke(val);
            }
        }
    }
}
