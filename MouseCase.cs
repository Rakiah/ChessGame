using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class MouseCase : MonoBehaviour 
{
	public SetupBoard setupBoard;
	public List<ChessMan> Pieces = new List<ChessMan>();

	public ChessMan BlackKing;
	public ChessMan WhiteKing;

	GameObject SelectorCase;

	Case activeCase;

	public bool CanPlay = false;

	public bool Black = true;
	public int currentPossibleMove = 0;

	public bool ShowEchec = false;
	public bool ShowMat = false;

	public bool isShowingMovements = false;
	public bool isReseting = false;

	public Transform posBlack;
	public Transform posWhite;

	public new Transform camera;

	public GameObject CheckMateLabel;
	public Text CheckMateText;

	public void TriggerShowMovement()
	{
		if (!isShowingMovements)
			StartCoroutine(showMovement());
	}

	public void TriggerReset()
	{
		if (!isReseting)
			StartCoroutine(Reset());
	}

	void Start () 
	{
		SelectorCase = GameObject.Find("Selector");
		setupBoard = GetComponent<SetupBoard>();
	}

	void DeselectCase (int x, int y)
	{
		setupBoard.board[x,y].SetSelector(false);
	}

	void DeselectCase(Case c)
	{
		c.SetSelector(false);
	}

	void DeselectCase ()
	{
		foreach(Case c in setupBoard.board)
		{
			c.SetSelector(false);
		}
	}

	void Update () 
	{
		if(CanPlay)
		{
			RaycastHit ray;

			if (Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out ray))
			{
				if(ray.collider.tag == "case")
				{
					SelectorCase.SetActive(true);
					Vector2i pos = new Vector2i(ray.collider.transform.position.x - setupBoard.Border, ray.collider.transform.position.z - setupBoard.Border);
					SelectorCase.transform.position = new Vector3(ray.collider.transform.position.x, 0.05f, ray.collider.transform.position.z);

					Case selectedCase = setupBoard.board[pos.x, pos.y];

					if(Input.GetMouseButtonDown(0))
					{
						//if we already have a locked piece, move it
						if(activeCase != null)
						{
							//if we picked another piece that is our own deselect last piece and select the new one
							if(selectedCase.currPiece != null && selectedCase.currPiece.color == Black)
							{
								DeselectCase();
								activeCase = selectedCase;

								List<Case> moves = activeCase.currPiece.GetPossibleMovement(setupBoard.board);

								foreach(Case c in moves)
								{
									if(c.currPiece != null && c.currPiece.color != activeCase.currPiece.color) c.SetSelector(true, StaticClass.m_sTools.attackCase);
									else if(c.currPiece == null) c.SetSelector(true, StaticClass.m_sTools.selectorCase);
								}
							}
							else
							{
								List<Case> moves = activeCase.currPiece.GetPossibleMovement(setupBoard.board);

								foreach(Case c in moves)
								{
									//if we are in a valid case move in
									if(c == selectedCase)
									{
										CanPlay = false;
										StartCoroutine(Played(selectedCase));
									}
								}
							}
						}
						else
						{
							if(selectedCase.currPiece != null && selectedCase.currPiece.color == Black)
							{
								activeCase = selectedCase;

								List<Case> moves = selectedCase.currPiece.GetPossibleMovement(setupBoard.board);
								foreach(Case c in moves)
								{
									if(c.currPiece != null && c.currPiece.color != activeCase.currPiece.color) c.SetSelector(true, StaticClass.m_sTools.attackCase);
									else if(c.currPiece == null) c.SetSelector(true, StaticClass.m_sTools.selectorCase);
								}
							}
						}
					}
				}
				else SelectorCase.SetActive(false);
			}
			else SelectorCase.SetActive(false);
		}
	}

	public void SetList (Case [,] board)
	{
		foreach(Case c in board)
		{
			if(c.currPiece != null)
			{
				if(c.currPiece.KeyChess)
				{
					if(c.currPiece.color) BlackKing = c.currPiece;
					else WhiteKing = c.currPiece;
				}
				else
				{
					Pieces.Add(c.currPiece);
				}
			}
		}

		Pieces.Add(BlackKing);
		Pieces.Add(WhiteKing);
	}

	public ChessMan GetKing () { return (Black == true ? BlackKing : WhiteKing); }

	IEnumerator Played (Case selected)
	{
		StopCoroutine(showMovement());
		Black = !Black;
		DeselectCase();
		yield return StartCoroutine(activeCase.currPiece.DoMovement(setupBoard.board, selected.position.x, selected.position.y));
		activeCase.currPiece = null;
		activeCase = null;
		DeselectCase();
		StartCoroutine(NextTurn());
	}

	public IEnumerator NextTurn ()
	{
		CheckMateText.text = "";
		if (StaticClass.m_sTools.PieceEchec(setupBoard.board, GetKing()) != null) CheckMateText.text = "Check";
		if (StaticClass.m_sTools.GetPossibleMove(Black) <= 0) CheckMateText.text += "Mate";

		if (CheckMateText.text != "")
		{
			CheckMateLabel.SetActive(true);
			yield return new WaitForSeconds(3.5f);
			CheckMateLabel.SetActive(false);
		}

		if(CheckMateText.text == "CheckMate") StartCoroutine(Reset());
		
		if (Black) CanPlay = true;
		else  StartCoroutine(getDecisions(Black));
	
	}
	public IEnumerator MoveTo (Transform pos)
	{
		float timeTo = 2.0f;
		float t = 0.0f;
		while(t < timeTo)
		{
			camera.position = Vector3.Lerp(camera.position, pos.position, t / (timeTo*2));
			camera.rotation = Quaternion.Lerp(camera.rotation, pos.rotation, t / (timeTo*2));

			t += Time.deltaTime;

			yield return null;
		}
	}

	IEnumerator getDecisions (bool color)
	{
		List<Decision> choices = new List<Decision>();
		foreach(ChessMan c in Pieces)
		{
			if(c.color == color)
			{
				Decision choice = StaticClass.m_sTools.Evaluate(c, setupBoard.board);

				if(choice == null || choice.decision == null) continue;

				choices.Add(choice);
			}
		}

		Decision bestDecision = null;
		foreach(Decision d in choices)
		{
			if(bestDecision == null) bestDecision = d;
			else if(d.decision.value > bestDecision.decision.value) bestDecision = d;
			else if(d.decision.value == bestDecision.decision.value) if(Random.Range(0,2) == 1) bestDecision = d;
			
		}

		if(bestDecision != null)
		{
			Debug.Log("decided to move on " + bestDecision.decision.value.ToString() + " as value, the case" + bestDecision.chess.position.ToString() + bestDecision.decision.choice.position.ToString());
			Debug.Log("decision : " + bestDecision.decision.apparentReason);

			activeCase = setupBoard.board[bestDecision.chess.position.x, bestDecision.chess.position.y];
			activeCase.SetSelector(true, StaticClass.m_sTools.selectorCase);
			StartCoroutine(Played(bestDecision.decision.choice));
			yield return null;
		}
	}

	IEnumerator showMovement ()
	{
		isShowingMovements = true;
		foreach(ChessMan chess in Pieces)
		{
			setupBoard.board[chess.position.x, chess.position.y].SetSelector(true, StaticClass.m_sTools.selectorCase);

			List<Case> moves = chess.GetPossibleMovement(setupBoard.board);
			foreach(Case c in moves)
			{
				if (isReseting)
				{
					isShowingMovements = false;
					DeselectCase();
					yield break;
				}
				if(c.currPiece != null && c.currPiece.color != chess.color)
				{
					c.SetSelector(true,  StaticClass.m_sTools.attackCase);
					yield return new WaitForSeconds(0.05f);
				}
				else if(c.currPiece == null)
				{
					c.SetSelector(true, StaticClass.m_sTools.selectorCase);
					yield return new WaitForSeconds(0.05f);
				}
			}

			yield return new WaitForSeconds(0.5f);

			DeselectCase();
		}
		isShowingMovements = false;
	}


	IEnumerator Reset ()
	{
		isReseting = true;
		foreach(ChessMan chess in Pieces) { Destroy(chess.obj); yield return null; }
		Pieces.Clear();

		foreach(Case c in setupBoard.board) c.currPiece = null;
		foreach(Case c in setupBoard.board) { if(setupBoard.SetChessMan(c.position.x, c.position.y)) yield return null; }
		SetList(setupBoard.board);
		StartCoroutine(NextTurn());
		isReseting = false;
	}
}