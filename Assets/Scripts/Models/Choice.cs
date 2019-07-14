using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Choice
{
    public string Description;
    public GameEvent Event;
    public EventData Data;
    public bool IsDefault;

    public void Activate()
    {
        if(Event != null)
        {
            Event.Raise(Data);
        }
    }
}
