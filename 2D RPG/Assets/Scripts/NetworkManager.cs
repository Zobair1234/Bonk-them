using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
    

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public int numPlayers;

    //instance
    public static NetworkManager instance;

    private void Awake()
    {
        if(instance != null && instance!=this)
        {
            gameObject.SetActive(false);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        //connect to master server

        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        //Debug.Log("you joined master server");

        
    }

  


    public void CreateRooms(string roomName)
    {
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = (byte)numPlayers;

        PhotonNetwork.CreateRoom(roomName, options);
    }

    public void Joinroom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
        Debug.Log("you joined master server");
    }

     
    //change the scene through photon system
    [PunRPC]
    public void ChangeScene(string sceneName)
    {
        PhotonNetwork.LoadLevel(sceneName);
    }

}
