using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUIManager : MonoBehaviour
{
	public Canvas Menu, Pause, GamePlay, GameOver; 
	public Button buttonSelectUnit, buttonOpenFire, buttonRotate, buttonMoveforwards, buttonReset, tempTeamAWin;
	public Text textUnitSelected, textActionPointsRemaining, textUnitName, textActionPoints, currentTeam, winningTeam;

	public Toggle teamAAI, teamBAI, teamAPC, teamBPC;

	//public Team.TeamType TeamA, TeamB;
	public bool TeamA, TeamB;

	private GameObject selectedActor;

	private static GUIManager m_instance = null;
	public static GUIManager instance { get { return m_instance; } }
	
	void Awake()
	{
		//instantiate singleton
		m_instance = this;
	}

	void Update ()
	{
		switch (GameManager.instance.GameState)
		{
		case GameStates.debug:
			break;
		case GameStates.gamepause:
			break;
		case GameStates.gameplay:
			Tick();
			break;
		case GameStates.menu:
			break;		
		case GameStates.gameOver:
			GameOverGUI();
			break;
		}
	}

	/// <summary>
	/// Tick whilst in the menuState.
	/// </summary>
	private void GameOverGUI()
	{
		GamePlay.gameObject.SetActive(false);

		GameOver.gameObject.SetActive(true);
	}

	/// <summary>
	/// Tick whilst in the GamePlayState.
	/// </summary>
	private void Tick()
	{
		switch (Selection.instance.selectState)
		{
		case Selection.SelectionState.None:
			//set all selection/control logic to inactive
			buttonSelectUnit.gameObject.SetActive(false);
			buttonOpenFire.gameObject.SetActive(false);
			buttonRotate.gameObject.SetActive(false);
			buttonMoveforwards.gameObject.SetActive(false);
			//set text
			textUnitSelected.gameObject.SetActive(false);
			textActionPointsRemaining.gameObject.SetActive(false);
			textUnitName.gameObject.SetActive(false);
			textActionPoints.gameObject.SetActive(false);
			break;
		case Selection.SelectionState.Rotate:
			textUnitName.text = selectedActor.GetComponent<ActorBase>().actorName;
			
			textActionPoints.text = selectedActor.GetComponent<ActorBase>().actionPoints.ToString();
			break;

		case Selection.SelectionState.Shoot:
			
			break;

		case Selection.SelectionState.ActorSelected:
			buttonSelectUnit.gameObject.SetActive(false);
			buttonOpenFire.gameObject.SetActive(true);
			buttonRotate.gameObject.SetActive(true);
			buttonMoveforwards.gameObject.SetActive(true);

			textUnitSelected.gameObject.SetActive(true);
			textActionPointsRemaining.gameObject.SetActive(true);
			textUnitName.gameObject.SetActive(true);
			//feed name of actor
			textActionPoints.gameObject.SetActive(true);

			textUnitName.text = selectedActor.GetComponent<ActorBase>().actorName;

			textActionPoints.text = selectedActor.GetComponent<ActorBase>().actionPoints.ToString();


			//feed actionPoints remaining
			break;
		case Selection.SelectionState.TrOctSelected:
			//only select the actor if it is that team's turn
			if (Selection.instance.target.GetComponent<TruncOct>().containedActor &&
			    Selection.instance.target.GetComponent<TruncOct>().containedActor.GetComponent<ActorBase>().Team == GameManager.instance.turnTeam)
			{
				selectedActor = Selection.instance.target.GetComponent<TruncOct>().containedActor;

				//set all selection/control logic
				buttonSelectUnit.gameObject.SetActive(true);
				buttonOpenFire.gameObject.SetActive(false);
				buttonRotate.gameObject.SetActive(false);
				buttonMoveforwards.gameObject.SetActive(false);

				textUnitSelected.gameObject.SetActive(false);
				textActionPointsRemaining.gameObject.SetActive(false);
				textUnitName.gameObject.SetActive(false);
				textActionPoints.gameObject.SetActive(false);
			}
			else 
			{
				//set all selection/control logic
				buttonSelectUnit.gameObject.SetActive(false);
				buttonOpenFire.gameObject.SetActive(false);
				buttonRotate.gameObject.SetActive(false);
				buttonMoveforwards.gameObject.SetActive(false);
				
				textUnitSelected.gameObject.SetActive(false);
				textActionPointsRemaining.gameObject.SetActive(false);
				textUnitName.gameObject.SetActive(false);
				textActionPoints.gameObject.SetActive(false);

				selectedActor = null;
			}
			break;
		}


		//test for esc input
		if (Input.GetKeyUp(KeyCode.Escape))
		{
			PauseGame();
		}
	}

	/// <summary>
	/// Changes the team dialog.
	/// </summary>
	/// <param name="_team">_team.</param>
	public void TeamChange(int _team)
	{
		if (_team == 0)
		{
			currentTeam.text = "Team: A";
		}
		else
		{
			currentTeam.text = "Team: B";
		}
	}


	/// <summary>
	/// Pauses the game and switches canvas.
	/// </summary>
	private void PauseGame()
	{
		GameManager.instance.GameState = GameStates.gamepause;

		//switch canvas
		Pause.gameObject.SetActive(true);
		GamePlay.gameObject.SetActive(false);
	}


	#region Buttons

	/// <summary>
	/// Selects the unit, called when the button is pressed.
	/// </summary>
	public void SelectUnit()
	{
		selectedActor = Selection.instance.SelectActor ();
	}

	/// <summary>
	/// Proceeds to the next turn.
	/// </summary>
	public void NextTurn()
	{
		GameManager.instance.TurnEnd();
	}

	public void MoveForwards ()
	{
		if (selectedActor.GetComponent<ActorBase> ().TryMoveForwards ())
		{
			//tell the team manager to redo the FOW
			GameManager.instance.ResetFOW();
			TeamManager.instance.ClearFOW(selectedActor.GetComponent<ActorBase>().Team);

			//Reset the GUI to None
			Selection.instance.selectState = Selection.SelectionState.TrOctSelected;
		}
	}

	/// <summary>
	/// Sets the selectionMode to be planning route the route.
	/// </summary>
	public void OpenFire ()
	{
		selectedActor.GetComponent<ActorBase>().Shootforwards();
	}

	/// <summary>
	/// Allows for the rotation of the selected unit, called when the button is pressed.
	/// </summary>
	public void RotateUnit()
	{
		Selection.instance.selectState = Selection.SelectionState.Rotate;
	}

	/// <summary>
	/// Resume the game and change canvas.
	/// </summary>
	public void ResumeGame()
	{
		GameManager.instance.GameState = GameStates.gameplay;

		//switch from the pause canvas to the gameplay canvas
		Pause.gameObject.SetActive(false);
		GamePlay.gameObject.SetActive(true);
	}


/// <summary>
/// Switchs the type of the team.
/// </summary>
/// <param name="_toggle">_toggle.</param>
	public void SwitchTeamType(Toggle _toggle)
	{
		if (_toggle == teamAAI)
		{
			//what has it been switched to?
			if (_toggle.isOn)
			{
				teamAPC.isOn = false;
			}
			else
			{
				teamAPC.isOn = true;
			}
		}
		else if (_toggle == teamBAI)
		{
			//what has it been switched to?
			if (_toggle.isOn)
			{
				teamBPC.isOn = false;
			}
			else
			{
				teamBPC.isOn = true;
			}
		}
		else if (_toggle == teamAPC)
		{
			//what has it been switched to?
			if (_toggle.isOn)
			{
				teamAAI.isOn = false;
			}
			else
			{
				teamAAI.isOn = true;
			}
		}
		else if (_toggle == teamBPC)
		{
			//what has it been switched to?
			if (_toggle.isOn)
			{
				teamBAI.isOn = false;
			}
			else
			{
				teamBAI.isOn = true;
			}
		}
	}

	public void ResetGame()
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene("WorldGen");
	}

	public void TempTeamAWin()
	{
		GameManager.instance.TeamWin(0);
	}


	/// <summary>
	/// Tells the GameManager to commence the game.
	/// </summary>
	public void CommenceGame()
	{
		//feed the GameManager knowledge of whether each team is AI or not
		GameManager.instance.Commence(teamAAI.isOn, teamBAI.isOn);

		Menu.gameObject.SetActive(false);
		GamePlay.gameObject.SetActive(true);
	}

	#endregion
}
