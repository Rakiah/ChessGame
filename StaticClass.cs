using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StaticClass : MonoBehaviour 
{
	public static StaticClass m_sTools;
	public static MouseCase m_sCase;

	public Material blackCase;
	public Material whiteCase;
	public Material selectorCase;
	public Material attackCase;

	public Color white;
	public Color black;
	

	void Awake ()
	{
		m_sTools = this;
		m_sCase = GetComponent<MouseCase>();
	}
	
	public List<Case> RookMoveSet (Case [,] board, int x, int y, bool color, bool limited)
	{
		List<Case> possibleCase = new List<Case>();

		int right = 1;
		while(true)
		{
			if(x + right <= 7) 
			{ 
				if(board[x + right, y].currPiece != null) { possibleCase.Add(board[x + right, y]); break; }
				else if(board[x + right, y].currPiece == null) possibleCase.Add(board[x + right, y]);
				else break;
			}
			else break;

		
			if(limited) break;
			right++;
		}
		
		int left = -1;
		while (true)
		{
			if(x + left >= 0) 
			{ 
				if(board[x + left, y].currPiece != null) { possibleCase.Add(board[x + left, y]); break; }
				else if(board[x + left, y].currPiece == null) possibleCase.Add(board[x + left, y]); 
				else break;
			}
			else break;


			if(limited) break;
			left--;
		}
		
		int top = 1;
		while(true)
		{
			if(y + top <= 7)
			{
				if(board[x, y + top].currPiece != null) { possibleCase.Add(board[x, y + top]); break; }
				else if(board[x, y + top].currPiece == null) possibleCase.Add(board[x, y + top]);
				else break;
			}
			else break;


			if(limited) break;
			top++;
		}
		
		int bot = -1;
		while(true)
		{
			if(y + bot >= 0)
			{
				if(board[x, y + bot].currPiece != null) { possibleCase.Add(board[x, y + bot]); break; }
				else if(board[x, y + bot].currPiece == null) possibleCase.Add(board[x, y + bot]);
				else break;
			}
			else break;


			if(limited) break;
			bot--;
		}

		return possibleCase;
	}

	public List<Case> BishopMoveSet (Case [,] board, int x, int y, bool color, bool limited)
	{
		List<Case> possibleCase = new List<Case>();
		//top right as black
		int right = 1;
		while(true)
		{
			if(x + right <= 7 && y + right <= 7) 
			{
				if (board[x + right, y + right].currPiece != null) { possibleCase.Add(board[x + right, y + right]); break; }
				else if (board[x + right, y + right].currPiece == null) possibleCase.Add(board[x + right, y + right]);
				else break;
			}
			else break;

			if(limited) break;
			right++;
		}
		//top left as black
		int left = -1;
		while (true)
		{
			if(x + left >= 0 && y - left <= 7) 
			{
				if (board[x + left, y - left].currPiece != null) { possibleCase.Add(board[x + left, y - left]); break; }
				else if (board[x + left, y - left].currPiece == null) possibleCase.Add(board[x + left, y - left]); 
				else break;
			}
			else break;

			if(limited) break;
			left--;
		}
		//bot right as black
		int top = 1;
		while(true)
		{
			if(x + top <= 7 && y - top >= 0)
			{
				if (board[x + top, y - top].currPiece != null) { possibleCase.Add(board[x + top, y - top]); break; }
				else if (board[x + top, y - top].currPiece == null) possibleCase.Add(board[x + top, y - top]);
				else break;
			}
			else break;

			if(limited) break;
			top++;
		}
		//bot left as black
		int bot = -1;
		while(true)
		{
			if(x + bot >= 0 && y + bot >= 0)
			{
				if (board[x + bot, y + bot].currPiece != null) { possibleCase.Add(board[x + bot, y + bot]); break; }
				else if (board[x + bot, y + bot].currPiece == null) possibleCase.Add(board[x + bot, y + bot]);
				else break;
			}
			else break;

			if(limited) break;
			bot--;
		}

		return possibleCase;
	}

	public List<Case> KnightMoveSet (Case [,] board, int x, int y, bool color)
	{
		List<Case> possibleCase = new List<Case>();

		if (x + 2 <= 7 && y + 1 <= 7) possibleCase.Add(board[x + 2, y + 1]);
		if (x - 2 >= 0 && y + 1 <= 7) possibleCase.Add(board[x - 2, y + 1]);
		if (x + 2 <= 7 && y - 1 >= 0) possibleCase.Add(board[x + 2, y - 1]);
		if (x - 2 >= 0 && y - 1 >= 0) possibleCase.Add(board[x - 2, y - 1]);
		if (x + 1 <= 7 && y + 2 <= 7) possibleCase.Add(board[x + 1, y + 2]);
		if (x - 1 >= 0 && y + 2 <= 7) possibleCase.Add(board[x - 1, y + 2]);
		if (x + 1 <= 7 && y - 2 >= 0) possibleCase.Add(board[x + 1, y - 2]);
		if (x - 1 >= 0 && y - 2 >= 0) possibleCase.Add(board[x - 1, y - 2]);

		return possibleCase;
	}
	
	public void RemoveEchecCase(Case toMove, List<Case> possibleCase, bool color, Case [,] board)
	{
		//do the plays for each case, check each one out of this, if there is a bad case out of this, delete it
		for (int i = possibleCase.Count - 1; i >= 0; i--) 
		{
			if(possibleCase[i].currPiece != null && possibleCase[i].currPiece.color == color) continue;
			if(PieceEchec(board, new Simulation(toMove, possibleCase[i]), (color == true ? m_sCase.BlackKing : m_sCase.WhiteKing), true) != null) possibleCase.RemoveAt(i); 
		}
	}

	public int GetPossibleMove (bool color)
	{
		int possible = 0;
		foreach(ChessMan chess in m_sCase.Pieces)
		{
			if(chess.color == color)
			{
				foreach(Case c in chess.GetPossibleMovement(m_sCase.setupBoard.board))
				{
					if(c.currPiece == null) possible++;
					else if(c.currPiece.color != chess.color) possible++;
				}
			}
		}

		return possible;
	}

	public List<Case> GetListOfPossibleMove (bool color)
	{
		List<Case> moves = new List<Case>();
		foreach(ChessMan chess in m_sCase.Pieces){ if(chess.color == color) { moves.AddRange(chess.GetPossibleMovement(m_sCase.setupBoard.board)); } }

		return moves;
	}


	public EchecThreat PieceEchec(Case [,] board, ChessMan Target, ChessMan deleted = null)
	{
		if(Target == null) return null; 
		EchecThreat threat = new EchecThreat(Target);

		foreach(ChessMan chess in m_sCase.Pieces)
		{
			if(deleted != null && deleted == chess) continue;

			if(chess.color != Target.color)
			{
				List<Case> moves = chess.GetPossibleMovement(board, false);
				foreach(Case c in moves)
				{
					//here it means if we have a possible movement case non null with the ennemy king on it
					if(c.currPiece != null && c.currPiece == Target)
					{					
						threat.Add(chess);
					}
				}
			}
		}

		threat.SortThreats();

		return threat.threater.Count > 0 ? threat : null;
	}

	public EchecThreat PieceEchec (Case [,] board, Simulation sim, bool OneShotSimulation = false)
	{
		sim.Simulate();

		EchecThreat threat = new EchecThreat(sim.targetCase.currPiece);

		foreach(ChessMan chess in m_sCase.Pieces)
		{
			if(sim.deletedBackup != null && sim.deletedBackup == chess) continue;
			
			if(chess.color != sim.targetCase.currPiece.color)
			{
				List<Case> moves = chess.GetPossibleMovement(board, false);
				foreach(Case c in moves)
				{
					//here it means if we have a possible movement case non null with the target on it
					if(c.currPiece != null && c.currPiece == sim.targetCase.currPiece)
					{
						threat.Add(chess);
					}
				}
			}
		}

		threat.SortThreats();
		if(OneShotSimulation) sim.Delete();

		return threat.threater.Count > 0 ? threat : null;
	}

	public EchecThreat PieceEchec (Case [,] board, Simulation sim, ChessMan Target, bool OneShotSimulation = false)
	{
		sim.Simulate();
		
		EchecThreat threat = new EchecThreat(sim.targetCase.currPiece);
		
		foreach(ChessMan chess in m_sCase.Pieces)
		{
			if(sim.deletedBackup != null && sim.deletedBackup == chess) continue;
			
			if(chess.color != Target.color)
			{
				List<Case> moves = chess.GetPossibleMovement(board, false);
				foreach(Case c in moves)
				{
					//here it means if we have a possible movement case non null with the target on it
					if(c.currPiece != null && c.currPiece == Target) threat.Add(chess);
				}
			}
		}
		
		threat.SortThreats();
		if(OneShotSimulation) sim.Delete();
		
		return threat.threater.Count > 0 ? threat : null;
	}
		
	public Decision Evaluate (ChessMan chess, Case [,] board)
	{
		int x = chess.position.x;
		int y = chess.position.y;

		List<Case> possibleCase = chess.GetPossibleMovement(board);
		 
		List<underDecision> underDecs = new List<underDecision>();
		int initialsMoves = GetListOfPossibleMove(chess.color).Count;

		EchecThreat isThreatened = PieceEchec(board, chess);

		foreach(Case c in possibleCase)
		{
			underDecision d = new underDecision(0, c);

			//here we found out that we can get a special piece with a priority, but take care, it might put us in a bad situation
			if(c.currPiece != null && c.currPiece.color != chess.color){ d.value += c.currPiece.Priority; d.SetReason("getting piece with priority : " + c.currPiece.Priority); }
			//that's a bad decision and even more, an unaceptable move since its our own piece, dont even listen to this
			else if(c.currPiece != null) continue;


			//create a simulation that we will simulate later on
			Simulation s = new Simulation(board[x,y], c);

			//i am already in echec

			if(isThreatened != null)
			{
				//simulate, so we can check if i can find some good situations
				//this move can send me out of echec, we should consider it
				if(PieceEchec(board, s) == null){ d.value += chess.Priority; d.SetReason("getting out of echec add : " + chess.Priority); Debug.Log(chess.ToString() + " not getting out of echec ??" + d.value); }
				//this move cant get me ouf of echec, we delete some priority to it, but it might be usefull on a certain sutaiton
				else { d.value -= chess.Priority; d.SetReason("cant get out, del : " + chess.Priority); }
			}
			else
			{
				//i was not in an echec move but im going into it? remove some value to this move
				if(PieceEchec(board, s) != null) { d.value -= chess.Priority; d.SetReason("going in echec intentionnaly del : " + chess.Priority); }
			}

			//get all of the move that come from our team after the simulation
			List<Case> moves = GetListOfPossibleMove(chess.color);
			bool covered = false;

			foreach(Case cover in moves) { if(cover == c) covered = true; }
			// calculate how many moves we opened up by doing this (because it means we have more possible movements out of this
			int difference = (moves.Count - initialsMoves) / 2;
			d.value += difference; d.SetReason("add some cases to play" + difference);

			if(StaticClass.m_sTools.GetPossibleMove(!chess.color) <= 0){ d.value += 1000; d.SetReason("put ennemy in mat"); }


			//now found out if we can put ennemy pieces in echec out of this move
			List<Case> SimulatedMove = chess.GetPossibleMovement(m_sCase.setupBoard.board, false);

			foreach(Case simulatedCase in SimulatedMove)
			{
				EchecThreat simulatedThreat = PieceEchec(board,simulatedCase.currPiece);
				if(simulatedCase.currPiece != null && simulatedCase.currPiece.color != chess.color) 
				{
					//we can actually put a piece in echec out of this, that's pretty good, even more if we are covered by another piece
					d.value += simulatedCase.currPiece.Priority / (covered == true ? 2 : 10);
					d.SetReason("put an piece in echec : " + (simulatedCase.currPiece.Priority / (covered == true ? 2 : 10))); 
				}

				//if we can cover an ally piece that is in echec do it
				else if(simulatedCase.currPiece != null && simulatedThreat != null)
				{
					d.value += simulatedCase.currPiece.Priority / 2;
					d.SetReason("helped a piece in echec : " + simulatedCase.currPiece.Priority / 2);
				}
			}

			s.Delete();
			underDecs.Add(d);
		}

		underDecision bestUdec = null;
		foreach(underDecision d in underDecs) 
		{
			if(bestUdec == null) bestUdec = d; 
			else if(bestUdec.value < d.value) bestUdec = d; 
			else if(bestUdec.value == d.value)
			{
				if(Random.Range(0,2) == 1) bestUdec = d;
			}
		}

		return new Decision(bestUdec, chess);
	}
}

public class Decision
{
	public ChessMan chess;
	public underDecision decision;
	
	public Decision () {}
	public Decision(underDecision dec, ChessMan piece)
	{
		chess = piece;
		decision = dec;
	}

	public Decision (int val, Case deci, ChessMan piece)
	{
		decision.value = val;
		decision.choice = deci;
		chess = piece;
	}
}
public class underDecision
{
	public int value;
	public string apparentReason;
	public Case choice;

	public underDecision(int val, Case c)
	{
		value = val;
		choice = c;
	}

	public void SetReason (string reason)
	{
		apparentReason += " and : " + reason;
	}
}
public class Simulation
{
    public ChessMan deletedBackup;

	public Case baseCase;
	public Case targetCase;


	public Simulation (Case bC, Case tC)
    {
        targetCase = tC;
        deletedBackup = tC.currPiece;

        baseCase = bC;
    }

	public void Simulate ()
	{
		targetCase.currPiece = baseCase.currPiece;
		targetCase.currPiece.position = targetCase.position;
		baseCase.currPiece = null;
	}

    public void Delete ()
    {
		baseCase.currPiece = targetCase.currPiece;
		baseCase.currPiece.position = baseCase.position;
		targetCase.currPiece = deletedBackup;
    }
}
public class EchecThreat
{
	public List<ChessMan> threater = new List<ChessMan>();
	public ChessMan threatened;


	public EchecThreat (ChessMan _threatened)
	{
		threatened = _threatened;
	}

	public void Add(ChessMan _threater)
	{
		threater.Add(_threater);
	}

	public void SortThreats ()
	{
		int ndebut = 0;
		int nfin = threater.Count - 1;
		while (ndebut < nfin)
		{
			int debut = ndebut;
			int fin = nfin;
			ndebut = threater.Count;
			for (int i = debut; i < fin; i++)
			{
				if (threater[i].Priority > threater[i + 1].Priority)
				{
					ChessMan chess = threater[i];
					threater[i] = threater[i + 1];
					threater[i + 1] = chess;
					if (i < ndebut)
					{
						ndebut = i - 1;
						if (ndebut < 0)
						{
							ndebut = 0;
						}
					}
					else if (i > nfin)
					{
						nfin = i + 1;
					}
				}
			}
		}
	}
}
