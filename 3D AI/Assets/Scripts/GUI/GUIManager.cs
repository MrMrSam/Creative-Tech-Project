using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUIManager : MonoBehaviour
{
	public Button buttonSelectUnit, buttonPlanRoute, buttonRotate, buttonMoveforwards;
	public Text textUnitSelected, textActionPointsRemaining, textUnitName, textActionPoints;

	private GameObject selectedActor;

	private static GUIManager m_instance = null;
	public static GUIManager instance { get { return m_instance; } }
	
	void Awake()
	{
		//instantiate singleton
		m_instance = this;
	}

	void Start ()
	{
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
		}
	}

	void Tick()
	{
		switch (Selection.instance.selectState)
		{
		case Selection.SelectionState.None:
			//set all selection/control logic to inactive
			buttonSelectUnit.gameObject.SetActive(false);
			buttonPlanRoute.gameObject.SetActive(false);
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
			buttonPlanRoute.gameObject.SetActive(true);
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
			if (Selection.instance.target.GetComponent<TruncOct>().containedActor /*&& Selection.instance.target.GetComponent<TruncOct>().containedActor*/)
			{
				selectedActor = Selection.instance.target.GetComponent<TruncOct>().containedActor;

				//set all selection/control logic
				buttonSelectUnit.gameObject.SetActive(true);
				buttonPlanRoute.gameObject.SetActive(false);
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
				buttonPlanRoute.gameObject.SetActive(false);
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
			//Reset the GUI to None
			Selection.instance.selectState = Selection.SelectionState.TrOctSelected;
		}
	}

	/// <summary>
	/// Sets the selectionMode to be planning route the route.
	/// </summary>
	public void PlanRoute ()
	{
		Selection.instance.selectState = Selection.SelectionState.PlanRoute;
	}

	/// <summary>
	/// Allows for the rotation of the selected unit, called when the button is pressed.
	/// </summary>
	public void RotateUnit()
	{
		Selection.instance.selectState = Selection.SelectionState.Rotate;
	}

	/// <summary>
	/// Moves the unit forwards, called when the button is pressed;
	/// </summary>
	public void MoveUnitForwards()
	{
		//selectedActor.GetComponent
	}
	#endregion
}
