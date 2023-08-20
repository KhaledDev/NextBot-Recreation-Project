using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.AI;

/// <summary>
/// This is the NextBot AI. It's nothing complicated and very simple.
/// </summary>

public class NextBot : MonoBehaviour
{
    NavMeshAgent agent;

    GameObject[] plrs;
    
    
    /// <summary>
    /// Sets the agent veriable and Disables the gameobject for the player to activate later.
    /// </summary>
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        gameObject.SetActive(false);
    }
    
    
    /// <summary>
    /// The player_index veriable will be used to count the amount of player and who the bot will follow.
    /// </summary>
    int player_index;

    /// <summary>
    /// Using the onEnable function. We take an array of the players and choose a random player
    /// for the bot to follow.
    /// </summary>
    private void OnEnable()
    {
        plrs = GameObject.FindGameObjectsWithTag("Player");

        player_index = Random.Range(0, plrs.Length);
    }

    /// <summary>
    /// In the update function we check if the current player is dead or not
    /// If he isn't dead we will make the bot follow him.
    /// else we call ChangePlayer function.
    /// </summary>
    private void Update()
    {

        if (!plrs[player_index].GetComponent<CollisionDetection>().Dead)
        {
            agent.SetDestination(plrs[player_index].transform.position);
        }
        else
        {
            ChangePlayer();
        }

    }
    /// <summary>
    /// Using the random.range we check the amount of players
    /// </summary>
    void ChangePlayer() => player_index = Random.Range(0, plrs.Length);

}
