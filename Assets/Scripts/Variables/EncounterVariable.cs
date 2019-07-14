using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EncounterVariable : TVariable<Encounter>
{
}

[System.Serializable]
public class EncounterReference: TReference<Encounter, EncounterVariable>
{
    public EncounterReference(Encounter initial) : base(initial)
    {
    }
}