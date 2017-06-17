using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManagerScript : MonoBehaviour {

    public static GameManagerScript gm;
    private MazeGeneratorScript maze;

    private bool isTimerReset;
    public int numberOfMazes;

    public enum gameStates { Start, Playing, BeatLevel ,  ExtraTime , GameOver };
    public gameStates gameState = gameStates.Playing;

    public enum playerAbility { None , Recruit , Hammer , twitch };
    public playerAbility playerAbilitySelect = playerAbility.Recruit;

    public GameObject playerPrefab;
    public GameObject player;

    private Vector3[] playerEntryPoint;
    private Vector3[] playerExitPoint;
    public GameObject entryPointPrefab;
    public GameObject exitPointPrefab;

    public Canvas gameOverCanvas;
    public Canvas pauseMenuCanvas;
    public Canvas hUDCanvas;
    public Canvas playerSelectCanvas;
    public Canvas beatLevelCanvas;

    private float timeLeft;
    public float totalTimeInSeconds = 15f;
    public Slider timeSlider;

    private float extraTimeLeft;
    public float extraTotalTimeInSeconds = 15f;
    public Slider extraTimeSlider;

    private Transform playerGPoint;


    private void Start()
    {
        if (!gm)
            gm = GetComponent<GameManagerScript>();

        maze = GameObject.FindGameObjectWithTag("Maze").GetComponent<MazeGeneratorScript>();
        //identify the point where the player will be generated
        playerGPoint = maze.playerGenerationPoint;
            //gameObject.GetComponent<MazeGeneratorScript>().playerGenerationPoint;
        if (!playerGPoint)
            return;
        player = Instantiate(playerPrefab, playerGPoint.position + Vector3.left, Quaternion.identity) as GameObject;
        player.tag = "Player";

        player.GetComponent<AbilityHammer>().enabled = false;
        player.GetComponent<AbilityHammer>().enabled = false;
        //assign player prefab so generated to the target variable in CameraFollow.cs:7
        Camera.main.GetComponent<CameraFollow>().target = player;

        PlayerSelect();

        timeLeft = totalTimeInSeconds;
        extraTimeLeft = extraTotalTimeInSeconds;

        gameOverCanvas.gameObject.SetActive(false);
        hUDCanvas.gameObject.SetActive(true);

        CreateEntryandExitTrigger();

        gameState = gameStates.Start;
    }

    void CreateEntryandExitTrigger()
    {
        /*for(int i=0;i< gm.gameObject.GetComponent<MazeGeneratorScript>().entryPoint.Length;i++)
        entryPoint = new Vector3[gm.gameObject.GetComponent<MazeGeneratorScript>().entryPoint.Length];
        exitPoint = new Vector3[gm.gameObject.GetComponent<MazeGeneratorScript>().exitPoint.Length];
      
        foreach (Vector3 vector in )
*/
        playerEntryPoint = maze.entryPoint;
        playerExitPoint = maze.exitPoint;
        numberOfMazes = playerEntryPoint.Length;

        for (int i = 0; i < numberOfMazes; i++)
        {
            Instantiate(entryPointPrefab, playerEntryPoint[i], Quaternion.identity);
            Instantiate(exitPointPrefab, playerExitPoint[i], Quaternion.identity);
        }
    }

    private void Update () {
        if (!player)
            return;
        if (playerAbilitySelect == playerAbility.None)
        {
            PlayerSelect();
        }
        else
        {
            if(playerAbilitySelect == playerAbility.Hammer)
            {
                player.GetComponent<AbilityHammer>().enabled = true;
                player.GetComponent<AbilityTwitch>().enabled = false;
            }
            else if(playerAbilitySelect == playerAbility.twitch)
            {
                player.GetComponent<AbilityHammer>().enabled = false;
                player.GetComponent<AbilityTwitch>().enabled = true;
            }
            playerSelectCanvas.gameObject.SetActive(false);

                Debug.Log(playerAbilitySelect);
                switch (gameState)
                {
                    case gameStates.Start:
                        Debug.Log("Start");
                        if (isTimerReset == false)
                        {
                            isTimerReset = true;
                            TimerReset();
                        }
                        hUDCanvas.gameObject.SetActive(true);
                        gameOverCanvas.gameObject.SetActive(false);
                        beatLevelCanvas.gameObject.SetActive(false);

                        if (Input.GetKeyDown(KeyCode.Escape))
                            Pause();
                        break;

                    case gameStates.Playing:
                        Debug.Log("Playing");
                        timeLeft -= Time.deltaTime;
                        TimeCheck();
                        isTimerReset = false;

                        if (Input.GetKeyDown(KeyCode.Escape))
                            Pause();

                        break;

                    case gameStates.ExtraTime:
                        Debug.Log("ExtraTime");
                        extraTimeLeft -= Time.deltaTime;
                        ExtraTimeCheck();

                        break;

                    case gameStates.BeatLevel:
                        Debug.Log("BeatLevel");
                        beatLevelCanvas.gameObject.SetActive(true);
                        hUDCanvas.gameObject.SetActive(false);
                        break;

                    case gameStates.GameOver:
                        Debug.Log("GameOver");
                        player.GetComponent<PlayerMovement>().enabled = false;
                        gameOverCanvas.gameObject.SetActive(true);
                        hUDCanvas.gameObject.SetActive(false);
                        break;
            }
        }
	}

    public void Pause()
    {
       
            if (pauseMenuCanvas.gameObject.activeInHierarchy == false)
            {
                Time.timeScale = 0;
                pauseMenuCanvas.gameObject.SetActive(true);
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                Time.timeScale = 1;
                pauseMenuCanvas.gameObject.SetActive(false);
                Cursor.lockState = CursorLockMode.None;
            }
    }

    void TimeCheck()
    {
        if (timeLeft > 0)
        {
            timeSlider.value = (timeLeft / totalTimeInSeconds);
        }

        else
        {
            gameState = gameStates.GameOver;
            isTimerReset = false;
        }
    }

    public void ExtraTimeCheck()
    {
        if (extraTimeLeft > 0)
            extraTimeSlider.value = extraTimeLeft / extraTotalTimeInSeconds;

        else
        {
            gameState = gameStates.GameOver;
            isTimerReset = false;
        }
    }

    public void DecreaseNumberOfMazes()
    {
        numberOfMazes--;
        if (numberOfMazes == 0)
        {
            gameState = gameStates.BeatLevel;
        }
        else
            gameState = gameStates.ExtraTime;

        TimerReset();
    }

    void TimerReset()
    {
        timeSlider.value = 1;
        timeLeft = totalTimeInSeconds;
        Debug.Log("Time Reset");
    }

    void PlayerSelect()
    {
        playerSelectCanvas.gameObject.SetActive(true);
    }
}
