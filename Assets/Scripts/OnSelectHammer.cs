using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnSelectHammer : MonoBehaviour {

    public void SelectHammer()
    {
        GameManagerScript.gm.playerAbilitySelect = GameManagerScript.playerAbility.Hammer;
    }
}
