using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// When you either create a room or enter a room this script will just spawn you.
/// </summary>

public class NetworkSpawner : MonoBehaviour
{

    public string player_name;
    public Transform SpawnPos;

    // Start is called before the first frame update
    void Start()
    {
        Spawn();
    }

    void Spawn()
    {
        PhotonNetwork.Instantiate(player_name, SpawnPos.position, SpawnPos.rotation);
    }

}
