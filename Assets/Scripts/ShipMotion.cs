using UnityEngine;

public class ShipMotion : MonoBehaviour
{
	public FloatReference Progress;
	public float MoveScaling = 1;
	public float MoveSpeed = 1;
	Vector3 startPos;
	float offset;
	private void Start()
	{
		startPos = transform.position;
		offset = Random.value;
	}
	void Update()
	{
		float xPos = Mathf.PerlinNoise(0, (offset + Progress.Value) * MoveSpeed) * MoveScaling;
		float yPos = Mathf.PerlinNoise(0.25f, (offset + Progress.Value) * MoveSpeed) * MoveScaling;
		float zPos = Mathf.PerlinNoise(0.5f, (offset + Progress.Value) * MoveSpeed) * MoveScaling;

		transform.position = startPos + new Vector3(xPos, yPos, zPos);
	}
}
