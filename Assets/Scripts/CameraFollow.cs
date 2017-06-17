using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    public GameObject target;
    Vector3 offset;
    float smoothing = 5.0f;

    private void Start()
    {
        offset = transform.position - Vector3.zero;
        if(GameManagerScript.gm)
            target = GameManagerScript.gm.player;
        
    }

    private void Update()
    {
        if (target == null)
            Debug.Log("Noplayer");
        if (target)
        {
            Vector3 targetCamPos = target.transform.position + offset;
            transform.position= Vector3.Lerp(transform.position, targetCamPos, smoothing * Time.deltaTime);
        }
    }

}
