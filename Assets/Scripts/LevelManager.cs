using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour {

	GroundMaganger gm;

	public PanelCountdown panelCountdown;
	public GameObject panelMainMenu;
	public GameObject panelPauseGame;
	public GameObject panelHighScore;
	public GameObject imageFlashScreen;
	public GameObject textPressToContinue;

	public GameObject perfPlayer;
	public int maxPlayer = 2;
	public int maxPlayerLife = 3;
	GameObject[] players;
	int[] scorePlayers;
	PlayerController[] PCs;

	static LevelManager instLevelManager;

	float timer;
	public float timeToNewMatch = 4f;
	string strHighScore;
	static string strSaveKey = "Game History";

	enum GameState {
		FlashScreen,
		MainMenu,
		InGame,
		PauseGame,
		HighScore,
	};
	GameState m_gameState;

	void Awake()
	{
		instLevelManager = this;
		gm = GetComponent<GroundMaganger> ();
		players = new GameObject [maxPlayer];
		scorePlayers = new int[maxPlayer];
		PCs = new PlayerController [maxPlayer];
		strHighScore = PlayerPrefs.GetString (strSaveKey);
		if (strHighScore.Length == 0)
			strHighScore = "High Score\n";

		imageFlashScreen.SetActive (true);
		textPressToContinue.SetActive (true);
		timer = 0;
		m_gameState = GameState.FlashScreen;
	}

		
	// Update is called once per frame
	void Update () {
		switch (m_gameState) {
		case GameState.FlashScreen:
			UpdateFlashscreen ();
			break;
		case GameState.MainMenu:
			break;
		case GameState.InGame:
			UpdateInGame ();
			break;
		case GameState.PauseGame:
			UpdateIngameMenu ();
			break;
		case GameState.HighScore:
			UpdateHighScore ();
			break;
		default:
			break;
		}


	}

	void UpdatePauseGame()
	{
		if (Input.GetKeyDown (KeyCode.Escape)) {

		}
	}

	void UpdateInGame()
	{
		if (timer > 0) {
			timer -= Time.deltaTime;


			if (timer <= 0) {
				SpawnPlayer (0);
				SpawnPlayer (1);
				gm.NewMap ();
			}
		} else {
			if (Input.GetKey (KeyCode.Escape)) {
				PauseGame ();
			}
		}
	}

	public void NewGame()
	{
		m_gameState = GameState.InGame;
		panelMainMenu.SetActive (false);
		scorePlayers[0] = maxPlayerLife;
		scorePlayers[1] = maxPlayerLife;

		//TextHighScore.getInstance ().gameObject.SetActive (false);
		NewMatch ();
	}

	public void NewMatch()
	{
		panelCountdown.gameObject.SetActive (true);
		panelCountdown.StartCountDown (timeToNewMatch);
		gm.ClearMap ();
		SimpleText.getInstance().SetDisplayText("" + scorePlayers [0] + " : " + scorePlayers [1]);

		if (players [0] != null)
			Destroy (players [0]);
		if (players [1] != null)
			Destroy (players [1]);	

		timer = timeToNewMatch;

	}

	void SpawnPlayer(int index)
	{
		players [index] = Instantiate (perfPlayer, gm.GetSpawnPoint(index), Quaternion.identity);
		PCs [index] = players [index].GetComponent<PlayerController> ();
		PCs [index].SetPlayerIndex (index);
	}


	public static LevelManager GetInstance()
	{
		return instLevelManager;
	}

	public void PlayerDie(int index)
	{
		scorePlayers [index]--;
		if (scorePlayers [index] > 0) {
			NewMatch ();
		} else {
			GameOver ();
		}
	}

	void GameOver ()
	{
		string result = (scorePlayers [0] == scorePlayers [1] ? "Draw" : "Player " + (scorePlayers [0] > scorePlayers [1] ? 1 : 2) + " win");
		strHighScore = strHighScore + result + " at " + System.DateTime.Now + "\n";
		PlayerPrefs.SetString (strSaveKey, strHighScore);

		ShowHighScore ();
	}

	public void ShowHighScore()
	{
		panelMainMenu.SetActive (false);
		panelHighScore.SetActive (true);

		gm.ClearMap ();
		SimpleText.getInstance ().SetDisplayText (strHighScore);

		m_gameState = GameState.HighScore;
	}

	public void Exit()
	{
		Application.Quit ();
	}

	void PauseGame()
	{
		panelPauseGame.SetActive (true);
		textPressToContinue.SetActive (true);
		m_gameState = GameState.PauseGame;
	}

	void ResumeGame()
	{
		textPressToContinue.SetActive (false);
		panelPauseGame.SetActive (false);
		m_gameState = GameState.InGame;
		timer = 0;
	}

	public bool IsGamePause()
	{
		return m_gameState == GameState.PauseGame;
	}

	void UpdateFlashscreen()
	{
		if (Input.anyKey) {
			imageFlashScreen.SetActive (false);
			textPressToContinue.SetActive (false);

			panelMainMenu.SetActive (true);
			m_gameState = GameState.MainMenu;
		} else {
			
			timer += Time.deltaTime * 2;
			textPressToContinue.SetActive ((int)timer % 2 == 0);
		}
	}

	void UpdateIngameMenu()
	{
		if (Input.anyKeyDown) {
			ResumeGame ();
		}
		else {

			timer += Time.deltaTime * 2;
			textPressToContinue.SetActive ((int)timer % 2 == 0);
		}
	}

	void UpdateHighScore()
	{
		if (Input.anyKeyDown) {
			m_gameState = GameState.MainMenu;
			panelMainMenu.SetActive (true);

			panelHighScore.SetActive (false);
			textPressToContinue.SetActive (false);
		}
		else {

			timer += Time.deltaTime * 2;
			textPressToContinue.SetActive ((int)timer % 2 == 0);
		}
	}

	public bool IsCountingForNewMatch()
	{
		return (timer > 0);
	}
}
