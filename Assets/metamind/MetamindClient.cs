using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetamindClient : MonoBehaviour {

  public string basePath;
	public string username;
	public string password;
  public string story;
  public string timeZone;
  public string locale;
  public string visitor;

  private string sessionId = null;

  private List<System.Action<Message>> onMessageListeners;

  private WWW doPost(string path, string json) {
    string URL = basePath + path;
    string accessToken;

    Dictionary<string, string> headers = new Dictionary<string, string>();
    accessToken = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(username + ":" + password));
    headers.Add("Authorization", "Basic " + accessToken);
    headers.Add("Content-Type", "application/json");
    byte[] postData = System.Text.Encoding.UTF8.GetBytes(json);

    return new WWW(URL, postData, headers);
  }

  private IEnumerator WaitForSession(WWW res) {
    yield return res;
    if (res.error == null) {
      Session createdSession = JsonUtility.FromJson<Session>(res.text);
      sessionId = createdSession.id;
    } else {
      Debug.Log(res.error);
    }
  }

  private IEnumerator WaitForResponse(WWW res) {
    yield return res;
    if (res.error == null) {
      Message createdMessage = JsonUtility.FromJson<Message>(res.text);
      onMessageListeners.ForEach((System.Action<Message> listener) => { 
        listener.Invoke(createdMessage); 
      });
    } else {
      Debug.Log(res.error);
    }
  }

  public void createSession() {
    Session session = new Session();
    session.story = story;
    session.locale = locale;
    session.timeZone = timeZone;
    session.visitor = visitor;

    string json = JsonUtility.ToJson(session);
    StartCoroutine(WaitForSession(doPost("/sessions", json)));
  }

  public void postMessage(string content) {
    Message message = new Message();
    message.sessionId = sessionId;
    message.content = content;

    string json = JsonUtility.ToJson(message);
    StartCoroutine(WaitForResponse(doPost("/messages", json)));
  }

	public void Start () {
    onMessageListeners = new List<System.Action<Message>>();
    createSession();
	}

  public void OnMetamindMessage(System.Action<Message> listener) {
    onMessageListeners.Add(listener);
  }
}
