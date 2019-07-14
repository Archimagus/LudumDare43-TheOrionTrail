using UnityEngine;
using UnityEngine.SceneManagement;

public class QuitButton : MonoBehaviour
{
	public void QuitToMenu()
	{
		SceneManager.LoadScene("MainMenu");
	}
}
