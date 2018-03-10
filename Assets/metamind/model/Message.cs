using System;
using System.Collections.Generic;

[Serializable]
public class Message {
  public string id;
  public string sessionId;
  public string content;
  public string response;
  public string hint;
  public List<string> quickResponses;
  public string created;
}
