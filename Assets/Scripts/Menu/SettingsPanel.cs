using UnityEngine;

public class SettingsPanel : MonoBehaviour
{
#pragma warning disable 0649
	[SerializeField] private GameObject _mainPanel;
	private MenuStack _menuStack;

	private void Awake()
	{
		_menuStack = Resources.Load<MenuStack>("MenuStack");
		if (_menuStack == null)
			Debug.LogError("MenuStack not found");
	}
	private void OnEnable()
	{
		_menuStack.OpenMenu(_mainPanel, false);
	}
}
