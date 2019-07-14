using UnityEngine;

public class ModifyIntVariable : MonoBehaviour
{
	public IntVariable Variable;
	public void Multiply(float ammount)
	{
		Variable.CurrentValue = Mathf.FloorToInt(Variable.CurrentValue * ammount);
	}
	public void Add(int ammount)
	{
		Variable.CurrentValue = Variable.CurrentValue + ammount;
	}
}
