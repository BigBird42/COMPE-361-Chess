using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chessboard : MonoBehaviour {

    public GameObject pawnPrefab, kingPrefab, queenPrefab, bishopPrefab, knightPrefab, rookPrefab;
    string nextMove = "";
    static public string currentPlayer;
    static public Dictionary<string, GameObject> pieces;
    private Vector3 whiteGrave;
    private Vector3 blackGrave;
    private GameObject selectedSquare;
    private GameObject targetSquare;

    // Use this for initialization
    void Start () {
        // Initialize starting player
        currentPlayer = "White";

        // Fill pieces dictionary with all pieces on the board
        GameObject[] piecesWhite = GameObject.FindGameObjectsWithTag("White");
        GameObject[] piecesBlack = GameObject.FindGameObjectsWithTag("Black");

        pieces = new Dictionary<string, GameObject>();
        foreach (GameObject piece in piecesWhite)
        {
            pieces.Add(WorldPointToRF(piece.transform.position), piece);
        }
        foreach (GameObject piece in piecesBlack)
        {
            pieces.Add(WorldPointToRF(piece.transform.position), piece);
        }

        // Initialize the indicator GameObjects
        selectedSquare = GameObject.FindGameObjectWithTag("SelectedSquare");
        targetSquare = GameObject.FindGameObjectWithTag("TargetSquare");

        // Initialize the starting location of the white and black graveyard
        whiteGrave.x = -4.5f;
        whiteGrave.y = -3.5f;
        blackGrave.x = 4.5f;
        blackGrave.y = 3.5f;
    }
	
	// Update is called once per frame
	void Update () {
		
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
        string location = WorldPointToRF(mousePos);
        GameObject piece;

        // If this is the first selection on this turn
        if (nextMove.Length == 0)
        {
            if (pieces.TryGetValue(location, out piece))
            {
                if (piece.tag == currentPlayer)
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
                    piece.transform.position = RFToWorldPoint(location);
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
            if (piece.name[1] == 'B')
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
            else if (piece.name[1] == 'W')
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
            selectedSquare.transform.position = RFToWorldPoint(location);
        }
        else
        {
            selectedSquare.transform.position = new Vector3(15, 0 , 0);
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
                Instantiate(targetSquare, RFToWorldPoint(location), Quaternion.identity);
            }
        }
        else
        {
            GameObject[] highlightedLocations = GameObject.FindGameObjectsWithTag("TargetSquare");
            foreach (GameObject location in highlightedLocations)
            {
                if(location != targetSquare)
                {
                    Destroy(location);
                }
            }
        }
    }

    /// <summary>
    /// Convert world point in unity to rank and file location on board
    /// </summary>
    private string WorldPointToRF(Vector3 pos)
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
    /// Convert rank and file location on board to world point in unity
    /// </summary>
    private Vector3 RFToWorldPoint(string location)
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
