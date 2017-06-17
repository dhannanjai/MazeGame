using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityHammer : MonoBehaviour {

    public int numberOfMoves = 3;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Wall" && numberOfMoves != 0 && Input.GetKey(KeyCode.LeftShift))
        {
            Debug.Log("Hammer is used");
            Destroy(collision.gameObject);
            numberOfMoves--;
        }
        
    }


}
