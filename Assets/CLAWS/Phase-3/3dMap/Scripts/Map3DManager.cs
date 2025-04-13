using UnityEngine;
using XRMultiplayer;

public class Map3DManager : MonoBehaviour
{
    [SerializeField] GameObject map;
    [SerializeField] GameObject navigation;
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject minimap;
    [SerializeField] LobbyUI lobbyUI;
    [SerializeField] PlayerOptions playerOptions;

    public void Open3DMap()
    {
        lobbyUI.QuickJoinLobby();
        map.SetActive(true);
        navigation.SetActive(false);
        mainMenu.SetActive(false);
        minimap.SetActive(false);
    }

    public void Close3DMap()
    {
        //playerOptions.LeaveTableConfirmationPanel();
        playerOptions.LogOut();
        map.SetActive(false);
        navigation.SetActive(true);
        mainMenu.SetActive(true);
        minimap.SetActive(true);
    }
}
