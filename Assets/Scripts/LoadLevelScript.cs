using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevelScript : MonoBehaviour {

    public string loadToLevel;

    public void LoadLevel()
    {
        SceneManager.LoadScene(loadToLevel);
    }
}
