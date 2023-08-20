using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetonFall : MonoBehaviour
{
    
    Transform Spawn_pos;
    
    //sets the Spawn_Pos Transform.
    private void Start()
    {
        Spawn_pos = GameObject.Find("NetworkSpawner/SpawnPos").transform;
    }
    
    //Checks if it collided with the player.
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            ResetPos(other.gameObject.transform);
        }
    }
    
    //Resets the players the player position to spawnpos.
    public void ResetPos(Transform plr)
    {
        plr.position = Spawn_pos.position;
    }

}
