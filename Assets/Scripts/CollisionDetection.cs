using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CollisionDetection : MonoBehaviour
{

    public bool Dead;
    /// <summary>
    /// Checks if the player collided with the bot and starts the Coroutine.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Bot" && !Dead)
        {
            StartCoroutine(ResetPlayer());
        }
    }
    
    /// <summary>
    /// Sets dead boolean to true. Get's the playermovement and disables it. then after 5 seconds 
    /// It renable the playermovement, And sets his position to the spawnPosition and sets dead to false
    /// </summary>
    IEnumerator ResetPlayer()
    {

        Dead = true;
        GetComponent<PlayerMovement>().enabled = false;

        yield return new WaitForSeconds(5);

        GetComponent<PlayerMovement>().enabled = true;
        transform.position = GameObject.Find("NetworkSpawner/SpawnPos").transform.position;
        Dead = false;
    }

}
