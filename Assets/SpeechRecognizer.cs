﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using RestSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class SpeechRecognizer : MonoBehaviour {

    public string apiKey = "";
    public TextAsset base64Audio;

	// Use this for initialization
	void Start ()
    {
        DecodeSpeech();
	}
	
	// Update is called once per frame
	void Update ()
    {
	}

    private string DecodeSpeech()
    {
        JContainer speechRequest = new JObject(
            new JProperty("audio", new JObject(
                new JProperty("content", base64Audio.text)
            )),
            new JProperty("config", new JObject(
                new JProperty("languageCode", new JValue("fi"))
            ))
        );

        Debug.Log(speechRequest.ToString());

        var client = new RestClient("https://speech.googleapis.com/");
        var req = new RestRequest("v1/speech:recognize", Method.POST);
        req.AddParameter("key", apiKey);
        req.AddBody(speechRequest.ToString());

        var resp = client.Execute(req);
        Debug.Log(resp.ErrorMessage);
        Debug.Log(resp.ResponseStatus);
        Debug.Log(resp.Content);
        return resp.Content;
    }
}