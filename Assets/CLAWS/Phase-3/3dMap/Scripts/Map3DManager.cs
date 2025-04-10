using UnityEngine;
using XRMultiplayer;

public class Map3DManager : MonoBehaviour
{
    [SerializeField] GameObject map;
    [SerializeField] LobbyUI lobbyUI;
    [SerializeField] PlayerOptions playerOptions;

    public void Open3DMap()
    {
        map.SetActive(true);
        lobbyUI.QuickJoinLobby();
    }

    public void Close3DMap()
    {
        //playerOptions.LeaveTableConfirmationPanel();
        playerOptions.LogOut();
        map.SetActive(false);
    }
}
