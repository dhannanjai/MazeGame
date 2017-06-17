using UnityEngine.SceneManagement;
using UnityEngine;

public class QuitGameScript : MonoBehaviour {
    
    public void OnQuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}
