using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chessboard : MonoBehaviour {

    public GameObject pawnPrefab, kingPrefab, queenPrefab, bishopPrefab, knightPrefab, rookPrefab;
    string nextMove = "";

	// Use this for initialization
	void Start () {
		
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
            if      (mousePos.x < -3.0f) { nextMove += 'a'; }
            else if (mousePos.x < -2.0f) { nextMove += 'b'; }
            else if (mousePos.x < -1.0f) { nextMove += 'c'; }
            else if (mousePos.x <  0.0f) { nextMove += 'd'; }
            else if (mousePos.x <  1.0f) { nextMove += 'e'; }
            else if (mousePos.x <  2.0f) { nextMove += 'f'; }
            else if (mousePos.x <  3.0f) { nextMove += 'g'; }
            else                         { nextMove += 'h'; }

            if      (mousePos.y < -3.0f) { nextMove += '1'; }
            else if (mousePos.y < -2.0f) { nextMove += '2'; }
            else if (mousePos.y < -1.0f) { nextMove += '3'; }
            else if (mousePos.y <  0.0f) { nextMove += '4'; }
            else if (mousePos.y <  1.0f) { nextMove += '5'; }
            else if (mousePos.y <  2.0f) { nextMove += '6'; }
            else if (mousePos.y <  3.0f) { nextMove += '7'; }
            else                         { nextMove += '8'; }

            if (nextMove.Length >= 4)
            {
                // interpret move
                nextMove = "";
            }
        }

        return;
    }
}
