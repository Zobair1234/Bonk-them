using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class MainMenu : MonoBehaviourPunCallbacks, ILobbyCallbacks
{
    //Screen
    [Header("Screen")]
    public GameObject mainScreen;
    public GameObject createRoomScreen;
    public GameObject lobbyScreen;
    public GameObject lobbyBrowserScreen;

    [Header("Main screen")]
    //main screen
    public Button createRoomButton;
    public Button findRoomButton;

    [Header("Lobby")]
    //lobby
    public TextMeshProUGUI playerListtext;
    public TextMeshProUGUI roomInfoText;
    public Button startGameButton;

    [Header("Lobby Browser")]
    //lobby browser
    public RectTransform roomListContainer;
    public GameObject roomButtonPrefab;

    private List<GameObject> roomButtons = new List<GameObject>();
    private List<RoomInfo> roomList = new List<RoomInfo>();

    private void Start()
    {
        //disable the menu button at the start
        createRoomButton.interactable = false;
        findRoomButton.interactable = false;

        //enable the cursor 
        Cursor.lockState = CursorLockMode.None;

        //if we are in game
        if (PhotonNetwork.InRoom)
        {
            //go to the lobby

            //make the game visible
            PhotonNetwork.CurrentRoom.IsVisible = true;
            PhotonNetwork.CurrentRoom.IsOpen = true;

        }
    }

    void Setscreen(GameObject screen)
    {
        //disable all other screen
        mainScreen.SetActive(false);
        createRoomScreen.SetActive(false);
        lobbyScreen.SetActive(false);
        lobbyBrowserScreen.SetActive(false);

        //activate the requested screen
        screen.SetActive(true);

        if (screen == lobbyBrowserScreen)
            UpdateLobbyBrowserUI();


    }


    //
    public void OnBackButton()
    {
        Setscreen(mainScreen);
    }


    public void OnPlayerNameValueChanged(TMP_InputField playerNameInput)
    {
        PhotonNetwork.NickName = playerNameInput.text;
    }

    public override void OnConnectedToMaster()
    {
        //enable the menu buttons upon connection to the master server
        createRoomButton.interactable = true;
        findRoomButton.interactable = true;

    }

    public void OnCreateRoomButton()
    {
        Setscreen(createRoomScreen);
    }

    public void OnfindRoomButton()
    {
        Setscreen(lobbyBrowserScreen);
    }

    public void OnCreateButton(TMP_InputField roomNameInput)
    {
        NetworkManager.instance.CreateRooms(roomNameInput.text);
    }

    //lobby screen

    public override void OnJoinedRoom()
    {
        Setscreen(lobbyScreen);

        // when join lobby update lobby UI for all
        photonView.RPC("UpdateLobbyUI", RpcTarget.All);

    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdateLobbyUI();
    }

    //RPC = call function in other player's PC
    [PunRPC]
    private void UpdateLobbyUI()
    {
        //enable or disable button if and only if HOST

        startGameButton.interactable = PhotonNetwork.IsMasterClient;

        //display all players
        playerListtext.text = "";

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            playerListtext.text += player.NickName + "\n";
        }

        // room info text
        roomInfoText.text = "<b>Room Name</b>\n" + PhotonNetwork.CurrentRoom.Name;
    }

    public void OnStartGameButton()
    {
        //close the room
        PhotonNetwork.CurrentRoom.IsOpen = false;
        //hide the room
        PhotonNetwork.CurrentRoom.IsVisible = false;

        //tell everyone to load ingame
        NetworkManager.instance.photonView.RPC("ChangeScene", RpcTarget.All, "Game");

    }

    public void OnLeaveLobbyButton()
    {
        // leave the room
        PhotonNetwork.LeaveRoom();

        //set screen back to main screen
        Setscreen(mainScreen);
    }

    //lobby browser script

    GameObject CreateRoomButtons()
    {
        GameObject buttonObj = Instantiate(roomButtonPrefab, roomListContainer.transform);
        roomButtons.Add(buttonObj);
        return buttonObj;
    }
    void UpdateLobbyBrowserUI()
    {
        // disable all room buttons
        foreach(GameObject button in roomButtons)
        {
            button.SetActive(false);
        }

        //display all current rooms in the master server
        for(int i=0; i<roomList.Count;i++)
        {
            //get or create button obj
            GameObject button = i >= roomButtons.Count ? CreateRoomButtons() : roomButtons[i];
            button.SetActive(true);
            //set room name player count
            button.transform.Find("Room Name Text").GetComponent<TextMeshProUGUI>().text = roomList[i].Name;
            button.transform.Find("Player Count Text").GetComponent<TextMeshProUGUI>().text = roomList[i].PlayerCount + "/" + roomList[i].MaxPlayers;

            //set the button onclick
            Button buttonComp = button.GetComponent<Button>();

            //save the room name
            string roomName = roomList[i].Name;

            buttonComp.onClick.RemoveAllListeners();
            buttonComp.onClick.AddListener(() => { NetworkManager.instance.Joinroom(roomName); });


        }
    }

    /*public void OnJoinRoomButton(string roomName)
    {
        NetworkManager.instance.Joinroom(roomName);
    }*/

    public void OnrefreshButton()
    {
        UpdateLobbyBrowserUI();
    }
    public override void OnRoomListUpdate(List<RoomInfo> allRooms)
    {
        roomList = allRooms;
    }
}
