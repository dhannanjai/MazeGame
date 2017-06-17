using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnSelectTwitch : MonoBehaviour {

    public void SelectTwitch()
    {
        GameManagerScript.gm.playerAbilitySelect = GameManagerScript.playerAbility.twitch;  
    }
}
