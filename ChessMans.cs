using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class ChessMan
{
	public bool color;
	public bool KeyChess;
	public GameObject obj;
	public float height;

	public int BasePriority;
	public int Priority;

	public Vector2i position;

	public virtual void Instantiate (GameObject o, int x, int y, bool col)
	{
		color = col;
		position = new Vector2i(x,y);

		obj = (GameObject) MonoBehaviour.Instantiate(o, 
		                                             new Vector3(StaticClass.m_sCase.setupBoard.board[x, y].thisCase.transform.position.x, 
		            											 height, 
		            											 StaticClass.m_sCase.setupBoard.board[x, y].thisCase.transform.position.z),
		                                             o.transform.rotation);

		obj.transform.localEulerAngles = new Vector3(obj.transform.localEulerAngles.x, color == true ? 270.0f : 90.0f, obj.transform.localEulerAngles.z);
		obj.GetComponent<Renderer>().material.color = color == true ? StaticClass.m_sTools.black : StaticClass.m_sTools.white;
	}

	public abstract List<Case> GetPossibleMovement (Case [,] board, bool recursive = true);

	public virtual IEnumerator DoMovement(Case [,] board, int x, int y)
	{
		Case selected = board[position.x, position.y];
		selected.currPiece = null;

		float t = 0.0f;
		while(t < 1.0f)
		{
			obj.transform.position = Vector3.Lerp(obj.transform.position, new Vector3(board[x,y].thisCase.transform.position.x, obj.transform.position.y, board[x,y].thisCase.transform.position.z), t);
			t += Time.deltaTime;
			
			if(t > 0.3f) { if(board[x, y].currPiece != null){ StaticClass.m_sCase.Pieces.Remove(board[x, y].currPiece); MonoBehaviour.Destroy(board[x, y].currPiece.obj); }}
			
			yield return null;
		}

		board[x,y].currPiece = this;
		position.x = x;
		position.y = y;
	}
}

public class Pawn : ChessMan
{
	bool firstMove = true;
	public Pawn ()
	{
		height = 0.63f;
		BasePriority = 5;
		Priority = BasePriority;
	}
	
	public override List<Case> GetPossibleMovement (Case[,] board, bool recursive = true)
	{
		List<Case> moves = new List<Case>();
		
		int moveSet = color == true ? 1 : -1;
		
		if (position.y + moveSet <= 7 && position.y + moveSet >= 0)
		{
			if (board[position.x, position.y + moveSet].currPiece == null)
			{
				moves.Add(board[position.x, position.y + moveSet]);
				if(position.y + moveSet + moveSet <= 7 && position.y + moveSet + moveSet >= 0)
				{
					if (firstMove) { if (board[position.x, position.y + moveSet + moveSet].currPiece == null) moves.Add(board[position.x, position.y + moveSet + moveSet]); }
				}
			}
		}
		
		//if we found an ennemy piece in diagonal, u can shot him down
		if(position.x < 7) if(board[position.x + 1, position.y + moveSet].currPiece != null) moves.Add(board[position.x + 1, position.y + moveSet]); 
		if(position.x > 0) if(board[position.x - 1, position.y + moveSet].currPiece != null) moves.Add(board[position.x - 1, position.y + moveSet]);


		if(recursive) StaticClass.m_sTools.RemoveEchecCase(board[position.x, position.y], moves, color, board);

		return moves;
	}
	
	public override IEnumerator DoMovement(Case [,] board, int x, int y)
	{
		Case selected = board[position.x, position.y];
		selected.currPiece = null;

		firstMove = false;
		float t = 0.0f;
		while(t < 1.0f)
		{
			obj.transform.position = Vector3.Lerp(obj.transform.position, new Vector3(board[x,y].thisCase.transform.position.x, obj.transform.position.y, board[x,y].thisCase.transform.position.z), t);
			t += Time.deltaTime;
			
			if(t > 0.3f) { if(board[x, y].currPiece != null){ StaticClass.m_sCase.Pieces.Remove(board[x,y].currPiece); MonoBehaviour.Destroy(board[x, y].currPiece.obj); }}
			
			yield return null;
		}

		if(y >= 7 || y <= 0)
		{
			MonoBehaviour.Destroy(obj);
			Queen newQueen = new Queen();
			newQueen.Instantiate(StaticClass.m_sCase.setupBoard.Queen, x, y, y >= 7 ? true : false);
			StaticClass.m_sCase.Pieces[StaticClass.m_sCase.Pieces.LastIndexOf(this)] = newQueen;

			board[x,y].currPiece = newQueen;
		}
		else
		{
			board[x,y].currPiece = this;
			position.x = x;
			position.y = y;
		}
	}

	public override string ToString ()
	{
		return "Pawn" + (color == true ? "Black" : "White");
	}
}
public class Rook : ChessMan
{
	public Rook ()
	{
		height = 0.73f;
		BasePriority = 15;
		Priority = BasePriority;
	}
	
	public override List<Case> GetPossibleMovement (Case[,] board, bool recursive = true)
	{
		List<Case> moves = StaticClass.m_sTools.RookMoveSet(board,position.x, position.y, color, false);

		if(recursive) StaticClass.m_sTools.RemoveEchecCase(board[position.x,position.y], moves, color, board);

		return moves;
	}

	public override string ToString ()
	{
		return "Rook " + (color == true ? "Black" : "White");
	}
}
public class Knight : ChessMan
{
	public Knight ()
	{
		height = 0.61f;
		BasePriority = 18;
		Priority = BasePriority;
	}
	
	public override List<Case> GetPossibleMovement (Case[,] board, bool recursive = true)
	{
		List<Case> moves = StaticClass.m_sTools.KnightMoveSet(board,position.x, position.y, color);

		if(recursive) StaticClass.m_sTools.RemoveEchecCase(board[position.x, position.y], moves, color, board);
		
		return moves;
	}

	public override string ToString ()
	{
		return "Knight " + (color == true ? "Black" : "White");
	}
}
public class Bishop : ChessMan
{
	public Bishop ()
	{
		height = 0.85f;
		BasePriority = 22;
		Priority = BasePriority;
	}
	
	public override List<Case> GetPossibleMovement (Case[,] board, bool recursive = true)
	{
		List<Case> moves = StaticClass.m_sTools.BishopMoveSet(board,position.x, position.y, color, false);

		if(recursive) StaticClass.m_sTools.RemoveEchecCase(board[position.x,position.y], moves, color, board);
		
		return moves;
	}

	public override string ToString ()
	{
		return "Bishop " + (color == true ? "Black" : "White");
	}
}
public class Queen : ChessMan
{
	public Queen ()
	{
		height = 0.92f;

		BasePriority = 48;
		Priority = BasePriority;
	}
	
	public override List<Case> GetPossibleMovement (Case[,] board, bool recursive = true)
	{
		List<Case> moves = StaticClass.m_sTools.RookMoveSet(board,position.x, position.y, color, false);
		moves.AddRange(StaticClass.m_sTools.BishopMoveSet(board,position.x, position.y, color, false));

		if(recursive) StaticClass.m_sTools.RemoveEchecCase(board[position.x, position.y], moves, color, board);

		return moves;
	}

	public override string ToString ()
	{
		return "Queen " + (color == true ? "Black" : "White");
	}
}
public class King : ChessMan
{
	public King ()
	{
		height = 0.97f;
		BasePriority = 60;
		Priority = BasePriority;

		KeyChess = true;
	}
	
	public override List<Case> GetPossibleMovement (Case[,] board, bool recursive = true)
	{
		List<Case> moves = StaticClass.m_sTools.RookMoveSet(board,position.x, position.y, color, true);
		moves.AddRange(StaticClass.m_sTools.BishopMoveSet(board,position.x, position.y, color, true));

		if(recursive) StaticClass.m_sTools.RemoveEchecCase(board[position.x, position.y], moves, color, board);

		return moves;

	}

	public override string ToString ()
	{
		return "King " + (color == true ? "Black" : "White");
	}
}

