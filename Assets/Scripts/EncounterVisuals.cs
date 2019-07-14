using UnityEngine;

public class EncounterVisuals : MonoBehaviour
{
	public GameEvent ShowEncounterWindowEvent;
	public Transform ParentObject;
	public GameObject[] EncounterVisualObjects;
	public EncounterVariable CurrentEncounter;
	public void SetVisualObject()
	{
		ClearVisualObject();
		var visuals = CurrentEncounter?.CurrentValue?.Visuals;
		int index = 0;
		if (visuals != null && visuals.Length > 0)
			index = visuals[Random.Range(0, visuals.Length)];
		
		var go = Instantiate(EncounterVisualObjects[index]);
		go.transform.SetParent(ParentObject, true);
		go.transform.localPosition = Vector3.zero;
		go.transform.localRotation = Quaternion.identity;
		go.transform.localScale = Vector3.one;

	}
	public void ClearVisualObject()
	{
		while(ParentObject.childCount > 0)
		{
			var c = ParentObject.GetChild(0);
			Destroy(c.gameObject);
			c.SetParent(null);
		}
	}
	public void ShowEncounterWindow()
	{
		ShowEncounterWindowEvent.Raise();
	}
}
