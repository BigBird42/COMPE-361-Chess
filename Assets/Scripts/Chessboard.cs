using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Chessboard : MonoBehaviour {
    public GameObject blackPawnPrefab, blackKingPrefab, blackQueenPrefab, blackBishopPrefab, blackKnightPrefab, blackRookPrefab;
    public GameObject whitePawnPrefab, whiteKingPrefab, whiteQueenPrefab, whiteBishopPrefab, whiteKnightPrefab, whiteRookPrefab;
    public GameObject selectedSquare;
    public GameObject targetSquare;
    public Text inCheckIndicator;
    public Text currentPlayerIndicator;
    public Text playerWonIndicator;
    public Button resetBoardButton, saveGameButton;
    string pieceToMove = "";
    static public string currentPlayer;
    static public Dictionary<string, GameObject> chessPieces;
    private List<GameObject> graveyard;
    private Vector3 whiteGrave;
    private Vector3 blackGrave;
    List<string> recordOfMoves;
    int recordOfMovesIdx;
    private bool stillPlaying;

    // Use this for initialization
    void Start () {
        chessPieces = new Dictionary<string, GameObject>();
        recordOfMoves = new List<string>();
        graveyard = new List<GameObject>();
        resetBoardButton.onClick.AddListener(ResetBoard);
        saveGameButton.onClick.AddListener(SaveGame);
        ResetBoard();
    }

    // Update is called once per frame
    void Update () {
		
	}

    private void ResetBoard()
    {
        // reset still playing boolean
        stillPlaying = true;

        // Reset starting player
        currentPlayer = "White";

        // Reset onscreen indicators
        inCheckIndicator.text = "";
        currentPlayerIndicator.text = "Current player is " + currentPlayer;
        playerWonIndicator.text = "";

        // Remove all chess pieces from board
        foreach (KeyValuePair<string, GameObject> pair in chessPieces)
        {
            Destroy(pair.Value);
        }
        chessPieces.Clear();

        // Clear record of moves
        recordOfMoves.Clear();
        recordOfMovesIdx = 0;

        // Reset piece that is being moved
        pieceToMove = "";
        // Remove location highlights
        highlightPiece("", false);
        highlightValidMoves(new LinkedList<string> { }, false);

        // Create black chess pieces in starting positions
        chessPieces.Add("e8", Instantiate(blackKingPrefab, FRToWorldPoint("e8"), Quaternion.identity));
        chessPieces.Add("d8", Instantiate(blackQueenPrefab, FRToWorldPoint("d8"), Quaternion.identity));
        chessPieces.Add("c8", Instantiate(blackBishopPrefab, FRToWorldPoint("c8"), Quaternion.identity));
        chessPieces.Add("f8", Instantiate(blackBishopPrefab, FRToWorldPoint("f8"), Quaternion.identity));
        chessPieces.Add("b8", Instantiate(blackKnightPrefab, FRToWorldPoint("b8"), Quaternion.identity));
        chessPieces.Add("g8", Instantiate(blackKnightPrefab, FRToWorldPoint("g8"), Quaternion.identity));
        chessPieces.Add("a8", Instantiate(blackRookPrefab, FRToWorldPoint("a8"), Quaternion.identity));
        chessPieces.Add("h8", Instantiate(blackRookPrefab, FRToWorldPoint("h8"), Quaternion.identity));
        chessPieces.Add("a7", Instantiate(blackPawnPrefab, FRToWorldPoint("a7"), Quaternion.identity));
        chessPieces.Add("b7", Instantiate(blackPawnPrefab, FRToWorldPoint("b7"), Quaternion.identity));
        chessPieces.Add("c7", Instantiate(blackPawnPrefab, FRToWorldPoint("c7"), Quaternion.identity));
        chessPieces.Add("d7", Instantiate(blackPawnPrefab, FRToWorldPoint("d7"), Quaternion.identity));
        chessPieces.Add("e7", Instantiate(blackPawnPrefab, FRToWorldPoint("e7"), Quaternion.identity));
        chessPieces.Add("f7", Instantiate(blackPawnPrefab, FRToWorldPoint("f7"), Quaternion.identity));
        chessPieces.Add("g7", Instantiate(blackPawnPrefab, FRToWorldPoint("g7"), Quaternion.identity));
        chessPieces.Add("h7", Instantiate(blackPawnPrefab, FRToWorldPoint("h7"), Quaternion.identity));

        // Create white chess pieces in starting positions
        chessPieces.Add("e1", Instantiate(whiteKingPrefab, FRToWorldPoint("e1"), Quaternion.identity));
        chessPieces.Add("d1", Instantiate(whiteQueenPrefab, FRToWorldPoint("d1"), Quaternion.identity));
        chessPieces.Add("c1", Instantiate(whiteBishopPrefab, FRToWorldPoint("c1"), Quaternion.identity));
        chessPieces.Add("f1", Instantiate(whiteBishopPrefab, FRToWorldPoint("f1"), Quaternion.identity));
        chessPieces.Add("b1", Instantiate(whiteKnightPrefab, FRToWorldPoint("b1"), Quaternion.identity));
        chessPieces.Add("g1", Instantiate(whiteKnightPrefab, FRToWorldPoint("g1"), Quaternion.identity));
        chessPieces.Add("a1", Instantiate(whiteRookPrefab, FRToWorldPoint("a1"), Quaternion.identity));
        chessPieces.Add("h1", Instantiate(whiteRookPrefab, FRToWorldPoint("h1"), Quaternion.identity));
        chessPieces.Add("a2", Instantiate(whitePawnPrefab, FRToWorldPoint("a2"), Quaternion.identity));
        chessPieces.Add("b2", Instantiate(whitePawnPrefab, FRToWorldPoint("b2"), Quaternion.identity));
        chessPieces.Add("c2", Instantiate(whitePawnPrefab, FRToWorldPoint("c2"), Quaternion.identity));
        chessPieces.Add("d2", Instantiate(whitePawnPrefab, FRToWorldPoint("d2"), Quaternion.identity));
        chessPieces.Add("e2", Instantiate(whitePawnPrefab, FRToWorldPoint("e2"), Quaternion.identity));
        chessPieces.Add("f2", Instantiate(whitePawnPrefab, FRToWorldPoint("f2"), Quaternion.identity));
        chessPieces.Add("g2", Instantiate(whitePawnPrefab, FRToWorldPoint("g2"), Quaternion.identity));
        chessPieces.Add("h2", Instantiate(whitePawnPrefab, FRToWorldPoint("h2"), Quaternion.identity));

        // Initialize the starting location of the white and black graveyard
        whiteGrave.x = -4.5f;
        whiteGrave.y = -3.5f;
        blackGrave.x = 4.5f;
        blackGrave.y = 3.5f;

        // Clear the graveyard
        foreach (GameObject piece in graveyard)
        {
            Destroy(piece);
        }
    }

    /// <summary>
    /// Save game as a string to be written to a file
    /// </summary>
    /// <returns></returns>
    public void SaveGame()
    {
        string saveState = "";
        foreach(string move in recordOfMoves)
        {
            saveState += move;
        }
        System.IO.File.WriteAllText(@"C:\Users\Public\Save.txt", saveState);
    }

    /// <summary>
    /// Load the game from a string
    /// </summary>`
    public void LoadGame()
    {
        ResetBoard();
        string loadState = System.IO.File.ReadAllText(@"C:\Users\Public\Save.txt");
        string move = "";
        for (int i = 0; i <= (loadState.Length-4); i += 4)
        {
            move = loadState.Substring(i, 4);
            // Skip records of move to grave, newPlayerMove will populate that for us
            if(move[1] == 'G') { continue; }
            try
            {
                newPlayerMove(FRToWorldPoint(move.Substring(0, 2)));
                newPlayerMove(FRToWorldPoint(move.Substring(2, 2)));
            }
            catch(System.ArgumentException)
            {
                throw new System.ArgumentException("Corrupted save file");
            }
        }
    }
    
    /// <summary>
    /// Determines actions to take on player clicks, all interaction from the players come through here
    /// </summary>
    private void OnMouseDown()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (mousePos.x < -4.0f || mousePos.x > 4.0f || mousePos.y < -4.0f || mousePos.y > 4.0f)
        {
            // Clicked outside of board
        }
        else
        {
            // if game is still in session
            if (stillPlaying)
            {
                newPlayerMove(mousePos);
            }

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

        // If no piece has been selected to move yet
        if (pieceToMove.Length == 0)
        {
            if (chessPieces.TryGetValue(location, out piece))
            {
                if (piece.tag.Contains(currentPlayer))
                {
                    pieceToMove += location;
                    highlightPiece(location, true);
                    highlightValidMoves(validMoveLocations(location), true);
                }
            }
        }
        else if (pieceToMove.Length == 2)
        {
            // See if move is valid
            if (isValidMove(pieceToMove, location))
            {
                // perform move
                if (chessPieces.TryGetValue(pieceToMove, out piece))
                {
                    moveToGrave(location);
                    piece.transform.position = FRToWorldPoint(location);
                    // Update key for piece that is moving
                    chessPieces.Remove(pieceToMove);
                    chessPieces.Add(location, piece);
                    // Promote pawns to Queens
                    if(piece.tag.Contains("Pawn") && (location[1] == '8' || location[1] == '1'))
                    {
                        pawnPromotion(location);
                    }
                }
                // record last move
                recordOfMoves.Add(pieceToMove + location);
                recordOfMovesIdx++;
                
                if (!stillPlaying)
                {
                    // Could have been changed by call to moveToGrave, we want to finish
                    // the killing blow and record the move, but now change player
                    // Reset current move
                    pieceToMove = "";
                    highlightPiece("", false);
                    highlightValidMoves(new LinkedList<string> { }, false);
                    return;
                }

                checkIfInCheck();

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
                // Indicate current player
                currentPlayerIndicator.text = "Current player is " + currentPlayer;

            }

            // Reset current move
            pieceToMove = "";
            highlightPiece("", false);
            highlightValidMoves(new LinkedList<string> { }, false);
        }
    }

    /// <summary>
    /// Moves a piece to the graveyard, does nothing if there is no piece at location
    /// </summary>
    /// <param name="location"></param>
    private void moveToGrave(string location)
    {
        GameObject piece;
        if(chessPieces.TryGetValue(location, out piece))
        {
            /*
            if (piece.tag.Contains(currentPlayer))
            {
                return;
            }
            */
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
                // Record the move to black grave
                recordOfMoves.Add("BG" + location);
                recordOfMovesIdx++;
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
                // Record the move to white grave
                recordOfMoves.Add("WG" + location);
                recordOfMovesIdx++;
            }
            if (piece.tag.Contains("King"))
            {
                playerWonIndicator.text = currentPlayer + " has won the game!";
                stillPlaying = false;
            }
            chessPieces.Remove(location);
            graveyard.Add(piece);
        }
    }

    /// <summary>
    /// Promotes a pawn at the location to a Queen, does not actually check if piece is a pawn
    /// </summary>
    /// <param name="location"></param>
    private void pawnPromotion(string location)
    {
        GameObject piece;
        if (chessPieces.TryGetValue(location, out piece))
        {
            Destroy(piece);
            chessPieces.Remove(location);
            // Assume Player wants a Queen
            if (currentPlayer == "White")
            {
                chessPieces.Add(location, Instantiate(whiteQueenPrefab, FRToWorldPoint(location), Quaternion.identity));
            }
            else if (currentPlayer == "Black")
            {
                chessPieces.Add(location, Instantiate(blackQueenPrefab, FRToWorldPoint(location), Quaternion.identity));
            }
            else
            {
                throw new System.Exception("Invalid player");
            }
        }
    }

    /// <summary>
    /// Displays warning on screen if the other player will be in check
    /// </summary>
    /// <returns></returns>
    private void checkIfInCheck()
    {
        string otherPlayer = "";
        if(currentPlayer == "White")
        {
            otherPlayer = "Black";
        }
        else if (currentPlayer == "Black")
        {
            otherPlayer = "White";
        }
        else
        {
            throw new System.Exception("currentPlayer is invalid somehow");
        }

        string inCheckBy = "";
        // Find the other players king
        GameObject checkKing = GameObject.FindGameObjectWithTag(otherPlayer + "King");
        // Iterate through all of the current players pieces
        foreach (KeyValuePair<string, GameObject> piece in chessPieces)
        {
            if (piece.Value.tag.Contains(currentPlayer))
            {
                // Add piece to string if it can move to the location of the enemy king
                if(isValidMove(piece.Key, WorldPointToFR(checkKing.transform.position)))
                {
                    inCheckBy += piece.Value.tag.Substring(5) + " " + piece.Key + ", ";
                }
            }
        }
        // If in  check list pieces checking king
        if (inCheckBy.Length != 0)
        {
            inCheckIndicator.text = otherPlayer + " is in check by " + currentPlayer + " " + inCheckBy.Substring(0, inCheckBy.Length - 2);
        }
        else
        {
            inCheckIndicator.text = "";
        }
        return;
    }

    /// <summary>
    /// Make the square under the selected piece golden, removes square instead if enable is false
    /// </summary>
    /// <param name="location"></param>
    /// <param name="enable"></param>
    private void highlightPiece(string location, bool enable)
    {
        if (enable)
        {
            Instantiate(selectedSquare, FRToWorldPoint(location), Quaternion.identity);
        }
        else
        {
            GameObject[] highlightedLocations = GameObject.FindGameObjectsWithTag("SelectedSquare");
            foreach (GameObject targetIndicator in highlightedLocations)
            {
                Destroy(targetIndicator);
            }
        }
    }

    /// <summary>
    /// Put a red square on locations of valid moves, removes squares instead if enable is false
    /// </summary>
    /// <param name="locations"></param>
    /// <param name="enable"></param>
    private void highlightValidMoves(LinkedList<string> locations, bool enable)
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
    /// <param name="pos"></param>
    /// <returns></returns>
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
    /// <param name="location"></param>
    /// <returns></returns>
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
    private bool isValidMove(string start, string move)
    {
        foreach (string location in validMoveLocations(start))
        {
            if (location == move)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Returns an array of strings containing valid locations for the given piece to move to
    /// </summary>
    /// <param name="location"></param>
    /// <returns></returns>
    private LinkedList<string> validMoveLocations(string location)
    {
        GameObject piece = chessPieces[location];
        if (piece.tag == "WhitePawn")
        {
            return validLocationsWhitePawn(location);
        }
        if (piece.tag == "BlackPawn")
        {
            return validLocationsBlackPawn(location);
        }
        if (piece.tag.Contains("Rook"))
        {
            return validLocationsRook(location);
        }
        if (piece.tag.Contains("Knight"))
        {
            return validLocationsKnight(location);
        }
        if (piece.tag.Contains("Bishop"))
        {
            return validLocationsBishop(location);
        }
        if (piece.tag.Contains("Queen"))
        {
            return validLocationsQueen(location);
        }
        if (piece.tag.Contains("King"))
        {
            return validLocationsKing(location);
        }
        return new LinkedList<string>(); // Just for testing, implement later
    }


    private LinkedList<string> validLocationsWhitePawn(string location)
    {
        LinkedList<string> possibleLocations = new LinkedList<string>();
        GameObject tempPiece;
        Vector3 pos1 = FRToWorldPoint(location);
        Vector3 pos2 = FRToWorldPoint(location);
        Vector3 pos3 = FRToWorldPoint(location);
        Vector3 pos4 = FRToWorldPoint(location);

        pos1.y++;
        // If first move
        if (pos2.y == -2.5f)
            pos2.y = pos2.y + 2;
        // If piece diagonal
        pos3.x++;
        pos3.y++;
        pos4.x--;
        pos4.y++;

        if (pos1.x > -4.0f && pos1.x < 4.0f && pos1.y > -4.0f && pos1.y < 4.0f)
        {
            if(!chessPieces.TryGetValue(WorldPointToFR(pos1), out tempPiece))
            {
                possibleLocations.AddLast(WorldPointToFR(pos1));
            }
        }

        if (pos2.x > -4.0f && pos2.x < 4.0f && pos2.y > -4.0f && pos2.y < 4.0f)
        {
            if (!chessPieces.TryGetValue(WorldPointToFR(pos2), out tempPiece))
            {
                possibleLocations.AddLast(WorldPointToFR(pos2));
            }
        }

        if (pos3.x > -4.0f && pos3.x < 4.0f && pos3.y > -4.0f && pos3.y < 4.0f)
        {
            if (chessPieces.TryGetValue(WorldPointToFR(pos3), out tempPiece))
            {
                if (tempPiece.tag.Contains(currentPlayer))
                {

                }
                else
                {
                    possibleLocations.AddLast(WorldPointToFR(pos3));
                }
            }
            
        }
        if (pos4.x > -4.0f && pos4.x < 4.0f && pos4.y > -4.0f && pos4.y < 4.0f)
        {
            if (chessPieces.TryGetValue(WorldPointToFR(pos4), out tempPiece))
            {
                if (tempPiece.tag.Contains(currentPlayer))
                {

                }
                else
                {
                    possibleLocations.AddLast(WorldPointToFR(pos4));
                }
            }
            
        }
        return possibleLocations;
    }
    private LinkedList<string> validLocationsBlackPawn(string location)
    {
        LinkedList<string> possibleLocations = new LinkedList<string>();
        GameObject tempPiece;
        Vector3 pos1 = FRToWorldPoint(location);
        Vector3 pos2 = FRToWorldPoint(location);
        Vector3 pos3 = FRToWorldPoint(location);
        Vector3 pos4 = FRToWorldPoint(location);

        pos1.y--;
        // If first move
        if (pos2.y == 2.5f)
            pos2.y = pos2.y - 2;
        // If piece diagonal
        pos3.x++;
        pos3.y--;
        pos4.x--;
        pos4.y--;

        if (pos1.x > -4.0f && pos1.x < 4.0f && pos1.y > -4.0f && pos1.y < 4.0f)
        {
            if (!chessPieces.TryGetValue(WorldPointToFR(pos1), out tempPiece))
            {
                possibleLocations.AddLast(WorldPointToFR(pos1));
            }
        }

        if (pos2.x > -4.0f && pos2.x < 4.0f && pos2.y > -4.0f && pos2.y < 4.0f)
        {
            if (!chessPieces.TryGetValue(WorldPointToFR(pos2), out tempPiece))
            {
                possibleLocations.AddLast(WorldPointToFR(pos2));
            }
        }

        if (pos3.x > -4.0f && pos3.x < 4.0f && pos3.y > -4.0f && pos3.y < 4.0f)
        {
            if (chessPieces.TryGetValue(WorldPointToFR(pos3), out tempPiece))
            {
                if (tempPiece.tag.Contains(currentPlayer))
                {

                }
                else
                {
                    possibleLocations.AddLast(WorldPointToFR(pos3));
                }
            }

        }
        if (pos4.x > -4.0f && pos4.x < 4.0f && pos4.y > -4.0f && pos4.y < 4.0f)
        {
            if (chessPieces.TryGetValue(WorldPointToFR(pos4), out tempPiece))
            {
                if (tempPiece.tag.Contains(currentPlayer))
                {

                }
                else
                {
                    possibleLocations.AddLast(WorldPointToFR(pos4));
                }
            }
        }
        return possibleLocations;
    }
    private LinkedList<string> validLocationsRook(string location)
    {
        LinkedList<string> possibleLocations = new LinkedList<string>();
        Vector3 pos = FRToWorldPoint(location);
        GameObject tempPiece; 

        for (float x = pos.x+1; x < 4.0f; x++)
        {
            if(chessPieces.TryGetValue(WorldPointToFR(new Vector3(x, pos.y)), out tempPiece))
                {
                    if (tempPiece.tag.Contains(currentPlayer))
                    {

                    }
                    else
                    {
                        possibleLocations.AddLast(WorldPointToFR(new Vector3(x, pos.y)));
                    }
                    break;
                }
                possibleLocations.AddLast(WorldPointToFR(new Vector3(x, pos.y)));
        }
        for (float x = pos.x-1; x > -4.0f; x--)
        {
            
            if (chessPieces.TryGetValue(WorldPointToFR(new Vector3(x, pos.y)), out tempPiece))
            {
                if (tempPiece.tag.Contains(currentPlayer))
                {

                }
                else
                {
                    possibleLocations.AddLast(WorldPointToFR(new Vector3(x, pos.y)));
                }
                break;
            }
            possibleLocations.AddLast(WorldPointToFR(new Vector3(x, pos.y)));
        }
        for (float y = pos.y+1; y < 4.0f; y++)
        {
            if (chessPieces.TryGetValue(WorldPointToFR(new Vector3(pos.x, y)), out tempPiece))
            {
                if (tempPiece.tag.Contains(currentPlayer))
                {

                }
                else
                {
                    possibleLocations.AddLast(WorldPointToFR(new Vector3(pos.x, y)));
                }
                break;
            }
            possibleLocations.AddLast(WorldPointToFR(new Vector3(pos.x, y)));
        }
        for (float y = pos.y-1; y > -4.0f; y--)
        {
            if (chessPieces.TryGetValue(WorldPointToFR(new Vector3(pos.x, y)), out tempPiece))
            {
                if (tempPiece.tag.Contains(currentPlayer))
                {

                }
                else
                {
                    possibleLocations.AddLast(WorldPointToFR(new Vector3(pos.x, y)));
                }
                break;
            }
            possibleLocations.AddLast(WorldPointToFR(new Vector3(pos.x, y)));
        }
        return possibleLocations;
    }
    private LinkedList<string> validLocationsKnight(string location)
    {
        LinkedList<string> possibleLocations = new LinkedList<string>();
        GameObject tempPiece;

        Vector3 pos1 = FRToWorldPoint(location);
        Vector3 pos2 = FRToWorldPoint(location);
        Vector3 pos3 = FRToWorldPoint(location);
        Vector3 pos4 = FRToWorldPoint(location);
        Vector3 pos5 = FRToWorldPoint(location);
        Vector3 pos6 = FRToWorldPoint(location);
        Vector3 pos7 = FRToWorldPoint(location);
        Vector3 pos8 = FRToWorldPoint(location);

        pos1.y++;
        pos1.x -= 2;
        pos2.y += 2;
        pos2.x--;
        pos3.y++;
        pos3.x += 2;
        pos4.y += 2;
        pos4.x++;
        pos5.x++;
        pos5.y -= 2;
        pos6.x += 2;
        pos6.y--;
        pos7.y--;
        pos7.x -= 2;
        pos8.y -= 2;
        pos8.x--;

        List<Vector3> pos = new List<Vector3>();
        pos.Add(pos1);
        pos.Add(pos2);
        pos.Add(pos3);
        pos.Add(pos4);
        pos.Add(pos5);
        pos.Add(pos6);
        pos.Add(pos7);
        pos.Add(pos8);

        foreach (Vector3 loc in pos)
        {
            if (loc.x > -4.0f && loc.x < 4.0f && loc.y > -4.0f && loc.y < 4.0f)
            {
                if (chessPieces.TryGetValue(WorldPointToFR(new Vector3(loc.x, loc.y)), out tempPiece))
                {
                    if (tempPiece.tag.Contains(currentPlayer))
                    {

                    }
                    else
                    {
                        possibleLocations.AddLast(WorldPointToFR(loc));
                    }
                    
                }
                else
                {
                    possibleLocations.AddLast(WorldPointToFR(loc));
                }
            }
        }
       
        return possibleLocations;
    }
    private LinkedList<string> validLocationsBishop(string location)
    {
        LinkedList<string> possibleLocations = new LinkedList<string>();
        Vector3 pos = FRToWorldPoint(location);
        GameObject tempPiece;

        for (float x = pos.x + 1, y = pos.y + 1; x < 4.0f && y < 4.0f; x++, y++)
        {
            if (chessPieces.TryGetValue(WorldPointToFR(new Vector3(x, y)), out tempPiece))
            {
                if (tempPiece.tag.Contains(currentPlayer))
                {

                }
                else
                {
                    possibleLocations.AddLast(WorldPointToFR(new Vector3(x, y)));
                }
                break;
            }
            possibleLocations.AddLast(WorldPointToFR(new Vector3(x, y)));
        }
        for (float x = pos.x - 1, y = pos.y + 1; x > -4.0f && y < 4.0f; x--, y++)
        {
            if (chessPieces.TryGetValue(WorldPointToFR(new Vector3(x, y)), out tempPiece))
            {
                if (tempPiece.tag.Contains(currentPlayer))
                {

                }
                else
                {
                    possibleLocations.AddLast(WorldPointToFR(new Vector3(x, y)));
                }
                break;
            }
            possibleLocations.AddLast(WorldPointToFR(new Vector3(x, y)));
        }
        for (float x = pos.x + 1, y = pos.y - 1; x < 4.0f && y > -4.0f; x++, y--)
        {
            if (chessPieces.TryGetValue(WorldPointToFR(new Vector3(x, y)), out tempPiece))
            {
                if (tempPiece.tag.Contains(currentPlayer))
                {

                }
                else
                {
                    possibleLocations.AddLast(WorldPointToFR(new Vector3(x, y)));
                }
                break;
            }
            possibleLocations.AddLast(WorldPointToFR(new Vector3(x, y)));
        }
        for (float x = pos.x - 1, y = pos.y - 1; x > -4.0f && y > -4.0f; x--, y--)
        {
            if (chessPieces.TryGetValue(WorldPointToFR(new Vector3(x, y)), out tempPiece))
            {
                if (tempPiece.tag.Contains(currentPlayer))
                {

                }
                else
                {
                    possibleLocations.AddLast(WorldPointToFR(new Vector3(x, y)));
                }
                break;
            }
            possibleLocations.AddLast(WorldPointToFR(new Vector3(x, y)));
        }

        return possibleLocations;
    }
    private LinkedList<string> validLocationsQueen(string location)
    {
        LinkedList<string> possibleLocations = new LinkedList<string>();

        foreach(string loc in validLocationsRook(location))
        {
            possibleLocations.AddLast(loc);
        }
        foreach (string loc in validLocationsBishop(location))
        {
            possibleLocations.AddLast(loc);
        }

        return possibleLocations;
    }
    private LinkedList<string> validLocationsKing(string location)
    {
        LinkedList<string> possibleLocations = new LinkedList<string>();
        GameObject tempPiece;
        Vector3 pos1 = FRToWorldPoint(location);
        Vector3 pos2 = FRToWorldPoint(location);
        Vector3 pos3 = FRToWorldPoint(location);
        Vector3 pos4 = FRToWorldPoint(location);
        Vector3 pos5 = FRToWorldPoint(location);
        Vector3 pos6 = FRToWorldPoint(location);
        Vector3 pos7 = FRToWorldPoint(location);
        Vector3 pos8 = FRToWorldPoint(location);

        pos1.y++;
        pos2.x++;
        pos2.y++;
        pos3.x++;
        pos4.x++;
        pos4.y--;
        pos5.y--;
        pos6.x--;
        pos6.y--;
        pos7.x--;
        pos8.x--;
        pos8.y++;

        List<Vector3> pos = new List<Vector3>();
        pos.Add(pos1);
        pos.Add(pos2);
        pos.Add(pos3);
        pos.Add(pos4);
        pos.Add(pos5);
        pos.Add(pos6);
        pos.Add(pos7);
        pos.Add(pos8);

        foreach (Vector3 loc in pos)
        {
            if (loc.x > -4.0f && loc.x < 4.0f && loc.y > -4.0f && loc.y < 4.0f)
            {
                if (chessPieces.TryGetValue(WorldPointToFR(new Vector3(loc.x, loc.y)), out tempPiece))
                {
                    if (tempPiece.tag.Contains(currentPlayer))
                    {

                    }
                    else
                    {
                        possibleLocations.AddLast(WorldPointToFR(loc));
                    }

                }
                else
                {
                    possibleLocations.AddLast(WorldPointToFR(loc));
                }
            }
        }

        return possibleLocations;
    }
}
