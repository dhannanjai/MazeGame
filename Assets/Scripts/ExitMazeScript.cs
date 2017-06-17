using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitMazeScript : MonoBehaviour {

    public bool called = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !called)
        {
            called = true;
            Debug.Log("Exit");
            GameManagerScript.gm.DecreaseNumberOfMazes();
        }
            
    }
}
