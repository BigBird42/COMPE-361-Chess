using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessPiece : MonoBehaviour {
    public int weight;

    public virtual string GetLoc()
    {
        throw new System.NotImplementedException();
    }

    public virtual bool IsValidMove()
    {
        throw new System.NotImplementedException();
    }

    public virtual void Move()
    {
        throw new System.NotImplementedException();
    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
