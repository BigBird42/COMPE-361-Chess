﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chessboard : MonoBehaviour {

    //public GameObject pawnPrefab, kingPrefab, queenPrefab, bishopPrefab, knightPrefab, rookPrefab;
    public GameObject blackPawnPrefab, blackKingPrefab, blackQueenPrefab, blackBishopPrefab, blackKnightPrefab, blackRookPrefab;
    public GameObject whitePawnPrefab, whiteKingPrefab, whiteQueenPrefab, whiteBishopPrefab, whiteKnightPrefab, whiteRookPrefab;
    public GameObject selectedSquare;
    public GameObject targetSquare;
    string nextMove = "";
    static public string currentPlayer;
    static public Dictionary<string, GameObject> pieces;
    private Vector3 whiteGrave;
    private Vector3 blackGrave;

    // Use this for initialization
    void Start () {
        pieces = new Dictionary<string, GameObject>();
        ResetBoard();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void ResetBoard()
    {
        // Reset starting player
        currentPlayer = "White";

        // Remove all pieces from board
        foreach (KeyValuePair<string, GameObject> pair in pieces)
        {
            Destroy(pair.Value);
        }
        pieces.Clear();

        // Create black pieces in starting positions
        pieces.Add("e8", Instantiate(blackKingPrefab, FRToWorldPoint("e8"), Quaternion.identity));
        pieces.Add("d8", Instantiate(blackQueenPrefab, FRToWorldPoint("d8"), Quaternion.identity));
        pieces.Add("c8", Instantiate(blackBishopPrefab, FRToWorldPoint("c8"), Quaternion.identity));
        pieces.Add("f8", Instantiate(blackBishopPrefab, FRToWorldPoint("f8"), Quaternion.identity));
        pieces.Add("b8", Instantiate(blackKnightPrefab, FRToWorldPoint("b8"), Quaternion.identity));
        pieces.Add("g8", Instantiate(blackKnightPrefab, FRToWorldPoint("g8"), Quaternion.identity));
        pieces.Add("a8", Instantiate(blackRookPrefab, FRToWorldPoint("a8"), Quaternion.identity));
        pieces.Add("h8", Instantiate(blackRookPrefab, FRToWorldPoint("h8"), Quaternion.identity));
        pieces.Add("a7", Instantiate(blackPawnPrefab, FRToWorldPoint("a7"), Quaternion.identity));
        pieces.Add("b7", Instantiate(blackPawnPrefab, FRToWorldPoint("b7"), Quaternion.identity));
        pieces.Add("c7", Instantiate(blackPawnPrefab, FRToWorldPoint("c7"), Quaternion.identity));
        pieces.Add("d7", Instantiate(blackPawnPrefab, FRToWorldPoint("d7"), Quaternion.identity));
        pieces.Add("e7", Instantiate(blackPawnPrefab, FRToWorldPoint("e7"), Quaternion.identity));
        pieces.Add("f7", Instantiate(blackPawnPrefab, FRToWorldPoint("f7"), Quaternion.identity));
        pieces.Add("g7", Instantiate(blackPawnPrefab, FRToWorldPoint("g7"), Quaternion.identity));
        pieces.Add("h7", Instantiate(blackPawnPrefab, FRToWorldPoint("h7"), Quaternion.identity));

        // Create white pieces in starting positions
        pieces.Add("e1", Instantiate(whiteKingPrefab, FRToWorldPoint("e1"), Quaternion.identity));
        pieces.Add("d1", Instantiate(whiteQueenPrefab, FRToWorldPoint("d1"), Quaternion.identity));
        pieces.Add("c1", Instantiate(whiteBishopPrefab, FRToWorldPoint("c1"), Quaternion.identity));
        pieces.Add("f1", Instantiate(whiteBishopPrefab, FRToWorldPoint("f1"), Quaternion.identity));
        pieces.Add("b1", Instantiate(whiteKnightPrefab, FRToWorldPoint("b1"), Quaternion.identity));
        pieces.Add("g1", Instantiate(whiteKnightPrefab, FRToWorldPoint("g1"), Quaternion.identity));
        pieces.Add("a1", Instantiate(whiteRookPrefab, FRToWorldPoint("a1"), Quaternion.identity));
        pieces.Add("h1", Instantiate(whiteRookPrefab, FRToWorldPoint("h1"), Quaternion.identity));
        pieces.Add("a2", Instantiate(whitePawnPrefab, FRToWorldPoint("a2"), Quaternion.identity));
        pieces.Add("b2", Instantiate(whitePawnPrefab, FRToWorldPoint("b2"), Quaternion.identity));
        pieces.Add("c2", Instantiate(whitePawnPrefab, FRToWorldPoint("c2"), Quaternion.identity));
        pieces.Add("d2", Instantiate(whitePawnPrefab, FRToWorldPoint("d2"), Quaternion.identity));
        pieces.Add("e2", Instantiate(whitePawnPrefab, FRToWorldPoint("e2"), Quaternion.identity));
        pieces.Add("f2", Instantiate(whitePawnPrefab, FRToWorldPoint("f2"), Quaternion.identity));
        pieces.Add("g2", Instantiate(whitePawnPrefab, FRToWorldPoint("g2"), Quaternion.identity));
        pieces.Add("h2", Instantiate(whitePawnPrefab, FRToWorldPoint("h2"), Quaternion.identity));

        // Initialize the starting location of the white and black graveyard
        whiteGrave.x = -4.5f;
        whiteGrave.y = -3.5f;
        blackGrave.x = 4.5f;
        blackGrave.y = 3.5f;
    }

    private void OnMouseDown()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (mousePos.x < -4.0f || mousePos.x > 4.0f || mousePos.y < -4.0f || mousePos.y > 4.0f)
        {
            // Clicked outside of board
        }
        else
        {
            newPlayerMove(mousePos);
        }

        return;
    }
    
    /// <summary>
    /// Interprets a mouse click on the playing area of the chess board
    /// </summary>
    /// <param name="mousePos"></param>
    public void newPlayerMove(Vector3 mousePos)
    {
        string location = WorldPointToFR(mousePos);
        GameObject piece;

        // If this is the first selection on this turn
        if (nextMove.Length == 0)
        {
            if (pieces.TryGetValue(location, out piece))
            {
                if (piece.tag.Contains(currentPlayer))
                {
                    nextMove += location;
                    hightlightPiece(location, true);
                    highlightValidMoves(validMoveLocations(location), true);
                }
            }
        }
        else if (nextMove.Length == 2)
        {
            // See if move is valid
            if (isValidMove(nextMove + location))
            {
                // perform move
                if (pieces.TryGetValue(nextMove, out piece))
                {
                    moveToGrave(location);
                    piece.transform.position = FRToWorldPoint(location);
                    // Update new location of the piece
                    pieces.Remove(nextMove);
                    pieces.Add(location, piece);
                }
                // Change player
                if (currentPlayer == "White")
                {
                    currentPlayer = "Black";
                }
                else if (currentPlayer == "Black")
                {
                    currentPlayer = "White";
                }
                else
                {
                    throw new System.Exception("currentPlayer is invalid somehow");
                }
            }

            // Reset current move
            nextMove = "";
            hightlightPiece("", false);
            highlightValidMoves(new string[] { }, false);
        }
    }

    /// <summary>
    /// Moves a piece to the graveyard, does nothing if there is no piece at location
    /// </summary>
    /// <param name="location"></param>
    private void moveToGrave(string location)
    {
        GameObject piece;
        if(pieces.TryGetValue(location, out piece))
        {
            if (piece.tag.Contains("Black"))
            {
                piece.transform.position = blackGrave;
                if(blackGrave.y < -3.0f)
                {
                    blackGrave.x = 5.5f;
                    blackGrave.y = 3.5f;
                }
                else
                {
                    blackGrave.y -= 1.0f;
                }
            }
            else if (piece.tag.Contains("White"))
            {
                piece.transform.position = whiteGrave;
                if (whiteGrave.y > 3.0f)
                {
                    whiteGrave.x = -5.5f;
                    whiteGrave.y = -3.5f;
                }
                else
                {
                    whiteGrave.y += 1.0f;
                }
            }
            pieces.Remove(location);
        }
    }

    /// <summary>
    /// Make the square under the selected piece golden, removes square instead if enable is false
    /// </summary>
    /// <param name="location"></param>
    /// <param name="enable"></param>
    private void hightlightPiece(string location, bool enable)
    {
        if (enable)
        {
            Instantiate(selectedSquare, FRToWorldPoint(location), Quaternion.identity);
        }
        else
        {
            Destroy(GameObject.FindGameObjectWithTag("SelectedSquare"));
        }
    }

    /// <summary>
    /// Put a red square on locations of valid moves, removes squares instead if enable is false
    /// </summary>
    /// <param name="locations"></param>
    /// <param name="enable"></param>
    private void highlightValidMoves(string[] locations, bool enable)
    {
        if (enable)
        {
            foreach(string location in locations)
            {
                Instantiate(targetSquare, FRToWorldPoint(location), Quaternion.identity);
            }
        }
        else
        {
            GameObject[] highlightedLocations = GameObject.FindGameObjectsWithTag("TargetSquare");
            foreach (GameObject targetIndicator in highlightedLocations)
            {
                Destroy(targetIndicator);
            }
        }
    }

    /// <summary>
    /// Convert world point in unity to file and rank location on board
    /// </summary>
    private string WorldPointToFR(Vector3 pos)
    {
        string location = "";
        if (pos.x < -4.0f) { throw new System.ArgumentException("pos to the left of the board"); }
        else if (pos.x < -3.0f) { location += 'a'; }
        else if (pos.x < -2.0f) { location += 'b'; }
        else if (pos.x < -1.0f) { location += 'c'; }
        else if (pos.x < 0.0f) { location += 'd'; }
        else if (pos.x < 1.0f) { location += 'e'; }
        else if (pos.x < 2.0f) { location += 'f'; }
        else if (pos.x < 3.0f) { location += 'g'; }
        else if (pos.x < 4.0f) { location += 'h'; }
        else { throw new System.ArgumentException("pos to the right of the board"); }

        if (pos.y < -4.0f) { throw new System.ArgumentException("pos below the board"); }
        else if (pos.y < -3.0f) { location += '1'; }
        else if (pos.y < -2.0f) { location += '2'; }
        else if (pos.y < -1.0f) { location += '3'; }
        else if (pos.y < 0.0f) { location += '4'; }
        else if (pos.y < 1.0f) { location += '5'; }
        else if (pos.y < 2.0f) { location += '6'; }
        else if (pos.y < 3.0f) { location += '7'; }
        else if (pos.y < 4.0f) { location += '8'; }
        else { throw new System.ArgumentException("pos above the board"); }

        return location;
    }

    /// <summary>
    /// Convert file and rank location on board to world point in unity
    /// </summary>
    private Vector3 FRToWorldPoint(string location)
    {
        float x = 0, y = 0;
        if(location[0] == 'a') { x = -3.5f; }
        else if (location[0] == 'b') { x = -2.5f; }
        else if (location[0] == 'c') { x = -1.5f; }
        else if (location[0] == 'd') { x = -0.5f; }
        else if (location[0] == 'e') { x =  0.5f; }
        else if (location[0] == 'f') { x =  1.5f; }
        else if (location[0] == 'g') { x =  2.5f; }
        else if (location[0] == 'h') { x = 3.5f; }
        else { throw new System.ArgumentException("Invalid File"); }
        
        if (location[1] == '1') { y = -3.5f; }
        else if (location[1] == '2') { y = -2.5f; }
        else if (location[1] == '3') { y = -1.5f; }
        else if (location[1] == '4') { y = -0.5f; }
        else if (location[1] == '5') { y = 0.5f; }
        else if (location[1] == '6') { y = 1.5f; }
        else if (location[1] == '7') { y = 2.5f; }
        else if (location[1] == '8') { y = 3.5f; }
        else { throw new System.ArgumentException("Invalid Rank"); }

        return new Vector3(x, y, 0.0f);
    }

    /// <summary>
    /// Determine if a given move is valid
    /// </summary>
    /// <param name="move"></param>
    /// <returns></returns>
    private bool isValidMove(string move)
    {

        return true; // Just for testing, implement later
    }

    /// <summary>
    /// Returns an array of strings containing valid locations for the given piece to move to
    /// </summary>
    /// <param name="location"></param>
    /// <returns></returns>
    private string[] validMoveLocations(string location)
    {
        return new string[] { "a1", "a2", "b1", "b2" }; // Just for testing, implement later
    }
}
