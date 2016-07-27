using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SetupBoard : MonoBehaviour 
{

	// Use this for initialization

	public GameObject prefab;
	public GameObject border;

	public GameObject Pawn;
	public GameObject Rook;
	public GameObject Knight;
	public GameObject Bishop;
	public GameObject Queen;
	public GameObject King;

	GameObject cam;
	public static int boardLength = 8;
	public int Border = 1;

	public Case [,] board = new Case[boardLength,boardLength];
	public List<GameObject> ObjectsDelete = new List<GameObject>();

	public MouseCase mS;

	void Start () 
	{
		cam = this.gameObject;
		mS = GetComponent<MouseCase>();
		cam.transform.position = new Vector3((float)(boardLength + Border + 1) / 2, cam.transform.position.y, cam.transform.position.z);
		StartCoroutine(Board());
	}

	public IEnumerator Board ()
	{
		int n = (boardLength - 1) + (Border * 2);
		int r = (n + 1) / 2;

		//this is how many rectangle we need to make, since n is the side of one rectangle we can deduce the number of rectangle to make the piece
		for(int i = 0; i < r; i ++)
		{
			int cR = (2 - i);
			int cB = 1;
			int cL = 1;

			for(int j = 0; j < n * 4; j++)
			{
				//here we have our x and y value which are the coordinate that you can call X / Y
				int x = 0;
				int y = 0;

				if (j <= n) { x = j + i; y = n + i; }
				else if (j <= n * 2) { x = n + i; y = j - cR; cR += 2; }
				else if (j <= n * 3) { x = n + i - cB; y = i; cB++; }
				else { x = i; y = cL + i; cL ++; }


				//we check if the value of our coordinates x,y are pair or impair, pair = black, impair = white, so we can have a chess board
				if (i < Border)
				{
					GameObject obj = (GameObject)MonoBehaviour.Instantiate(border, new Vector3(x, -0.225f, y), Quaternion.identity);
					obj.name = "Board";
					obj.tag = "Untagged";

					if(Border > 1) if(i == 0){ obj.transform.localScale = new Vector3(1f, 0.25f, 1f); obj.transform.position = new Vector3(x, -0.6f, y);}

					ObjectsDelete.Add(obj);
				}
				else 
				{
					board[x - Border, y - Border] = new Case(x, y, board, prefab, ((x + 1) + (y + 1)) % 2 != 0); SetChessMan(x - Border, y - Border);
					ObjectsDelete.Add(board[x - Border, y - Border].thisCase);
				}

				yield return null;
			}
			//we decrement n value because we want to end up with a little cube inside a big cube and etc..
			n -= 2;
		}

//		classic way of setting the board
//		for(int i = 0; i <= 7; i++)
//		{
//			color = !color;
//			for(int j = 0; j <= 7; j++)
//			{
//				board[j,i] = new Case(j,i, board, prefab, color);
//				color = !color;
//
//				SetChessMan(j,i);
//
//				yield return new WaitForSeconds(0.03f);
//			}
//		}
//
		ObjectsDelete.Reverse();
		mS.SetList(board);
		StartCoroutine(mS.NextTurn());
	}

	public bool SetChessMan (int i, int j)
	{
		if(j == 1 || j == 6)
		{
			board[i,j].currPiece = new Pawn ();
			board[i,j].currPiece.Instantiate(Pawn, i, j, j == 1 ? true : false);
			return true;
		}
		else if(j == 0 || j == 7)
		{
			if(i == 0 || i == 7)
			{
				board[i,j].currPiece = new Rook();
				board[i,j].currPiece.Instantiate(Rook, i, j, j == 0 ? true : false);
				return true;
			}
			else if(i == 1 || i == 6)
			{
				board[i,j].currPiece = new Knight();
				board[i,j].currPiece.Instantiate(Knight, i, j, j == 0 ? true : false);
				return true;
			}
			else if(i == 2 || i == 5)
			{
				board[i,j].currPiece = new Bishop();
				board[i,j].currPiece.Instantiate(Bishop, i, j, j == 0 ? true : false);
				return true;
			}
			else if(i == 3)
			{
				board[i,j].currPiece = new Queen();
				board[i,j].currPiece.Instantiate(Queen, i, j, j == 0 ? true : false);
				return true;
			}
			else if(i == 4)
			{
				board[i,j].currPiece = new King();
				board[i,j].currPiece.Instantiate(King, i, j, j == 0 ? true : false);
				return true;
			}
		}

		return false;
	}
}

public class Case
{
	public bool col;
	public Material mat;

	public GameObject thisCase;
	public Vector2i position;
	public ChessMan currPiece;

	public Case (int x, int y, Case[,] board, GameObject prefab, bool color)
	{
		col = color;
		mat = col == true ? StaticClass.m_sTools.blackCase : StaticClass.m_sTools.whiteCase;
		position = new Vector2i(x - StaticClass.m_sCase.setupBoard.Border, y - StaticClass.m_sCase.setupBoard.Border);
		thisCase = (GameObject)MonoBehaviour.Instantiate(prefab, new Vector3(x, 0, y), Quaternion.identity);
		thisCase.name = "case : " + x + " : " + y;
		thisCase.GetComponent<Renderer>().material = mat;
	}

	public void SetSelector(bool set, Material toSet = null) { thisCase.GetComponent<Renderer>().material = set == true ? toSet : mat; }
}

public struct Vector2i 
{
	public int x;
	public int y;

	public Vector2i (int _x, int _y)
	{
		x = _x;
		y = _y;
	}

	public Vector2i (float _x, float _y)
	{
		x = (int)_x;
		y = (int)_y;
	}

	public override string ToString ()
	{
		return "x : " + x + "; y : " + y;
	}
}
