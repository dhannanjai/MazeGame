using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntryMazeScript : MonoBehaviour {

    public bool called = false;

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player" && !called)
        {
            called = true;
            Debug.Log("enter");
            GameManagerScript.gm.gameState = GameManagerScript.gameStates.Playing;
        }
    }
}
