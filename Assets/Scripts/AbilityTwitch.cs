using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityTwitch : MonoBehaviour {

    private bool flied = false;
    public Camera droneCamprefab;
    private Camera droneCam;
    private  Transform player;
    public float totaltime = 3f;
    private bool start = false;

    private void Start()
    {
        player = GameManagerScript.gm.player.transform;
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            start = true;
        }

        if (flied != true && start == true)
        {
            if (totaltime >= 0)
            {
                totaltime -= Time.deltaTime;
                droneToWork();
            }
            else
            {
                Debug.Log("Out of loop");
                flied = true;
                droneCam.depth = -1;
                Camera.main.depth = 2;
            }
        }
    }
    private void droneToWork()
    {
        Debug.Log("Called droneToWork()");

            Debug.Log("Twitch is applied");
            droneCam = Camera.Instantiate<Camera>(droneCamprefab, player);
            droneCam.transform.position = new Vector3(player.position.x, 15, player.position.z);
            droneCam.transform.rotation = Quaternion.identity;
            droneCam.transform.Rotate(Vector3.right, 90f);
            droneCam.depth = 1; 

    }
}
