using System.Net;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
	[SerializeField, Scene] private int sceneToLoad;
	[SerializeField] private Button startButton;
	[SerializeField] private Button quitButton;
	[SerializeField] private InputField ipInputField;

	private void Awake()
	{
		Application.runInBackground = true;
		
		startButton.onClick.AddListener(RunGame);
		quitButton.onClick.AddListener(Application.Quit);

		ipInputField.text = Utils.GetIP4Address();
	}

	private void RunGame()
	{
		string ipAddress = ipInputField.text.Trim();
		Settings.SERVER_IP = IPAddress.Parse(ipAddress);
		
		SceneManager.LoadScene(sceneToLoad);
	}
}