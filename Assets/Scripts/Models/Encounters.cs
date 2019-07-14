using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Encounters : ScriptableObject
{
    public List<Encounter> AvailableEncounters = new List<Encounter>();
}
