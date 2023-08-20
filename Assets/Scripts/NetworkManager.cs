using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

/// <summary>
/// This is the networkManager Script. That uses the PhotonAPI
/// 
/// Photon is a simple multiplayer API. All of the functions names and what they do
/// are self-explanatory. 
/// </summary>

public class NetworkManager : MonoBehaviourPunCallbacks
{

    public TMP_InputField ServerName;
    public TMP_InputField PlayerName;
    public Button JoinButton;
    public Button CreateButton;


    private void Start()
    {
        ServerName.interactable = false;
        PlayerName.interactable = false;
        JoinButton.interactable = false;
        CreateButton.interactable = false;

        PhotonNetwork.AutomaticallySyncScene = true;
        Connect();
    }

    public void Update()
    {
        PhotonNetwork.LocalPlayer.NickName = PlayerName.text;
    }

    public override void OnConnectedToMaster()
    {

        //Enable buttons to join and create

        ServerName.interactable = true;
        PlayerName.interactable = true;
        JoinButton.interactable = true;
        CreateButton.interactable = true;

        print("Connected");

        base.OnConnectedToMaster();
    }

    void Connect()
    {
        PhotonNetwork.GameVersion = "0.0.1";
        PhotonNetwork.ConnectUsingSettings();

        print("Connecting");
    }

    public override void OnJoinedRoom()
    {
        StartGame();

        base.OnJoinedRoom();
    }

    public void Join()
    {
        PhotonNetwork.JoinRoom(ServerName.text);
    }

    public void Create()
    {
        PhotonNetwork.CreateRoom(PhotonNetwork.LocalPlayer.NickName);
    }

    public void StartGame()
    {
        if(PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            PhotonNetwork.LoadLevel(1);
        }
    }

}
