using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Encounter : ScriptableObject
{
    public string Description;
    public List<Choice> Choices;
	public int []Visuals;

    public bool Encountered;
}
