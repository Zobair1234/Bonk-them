using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;

public class GameManager : MonoBehaviourPun
{
    [Header("Players")]
    public string PlayerPrefabpath;

    public PlayerController[] players;

    public Transform[] spawnPoint;
    public float respawnTime;

    private int playersInGame;

    //instance
    public static GameManager instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        // set the length of array
        players = new PlayerController[PhotonNetwork.PlayerList.Length];

        photonView.RPC("ImInGame", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void ImInGame()
    {
        playersInGame++;

        //is the amount of players in the game as same as in the room?
        if(playersInGame == PhotonNetwork.PlayerList.Length)
        {
            SpawnPLayer();
        }
    }

    void SpawnPLayer()
    {
        GameObject playerObject = PhotonNetwork.Instantiate(PlayerPrefabpath, spawnPoint[Random.Range(0,spawnPoint.Length)].position, Quaternion.identity);

        //initialize player

        playerObject.GetComponent<PhotonView>().RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);
    }
}
