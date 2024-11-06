using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetGPSSpawn : MonoBehaviour
{
    GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Main Camera").gameObject;
    }

    public void Reset(Location loc)
    {
        player.transform.position = new Vector3(0f, 0f, 0f);
        GPSUtils.ChangeOriginGPSCoords(loc);
    }

}
