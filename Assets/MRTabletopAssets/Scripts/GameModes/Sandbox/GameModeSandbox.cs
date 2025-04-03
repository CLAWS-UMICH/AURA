using MRTTT;
using UnityEngine;

public class GameModeSandbox : MonoBehaviour, IGameMode
{
    /// <summary>
    /// The ID of the game mode. Used to order the game modes in the game mode manager.
    /// </summary>
    public int gameModeID => m_GameModeID;

    [SerializeField]
    int m_GameModeID = 1;

    [SerializeField]
    NetworkObjectDispenser m_ObjectDispenser;

    [SerializeField]
    GameObject[] m_ObjectsToToggle;

    void Start()
    {
        HideGameMode();
    }

    public void HideGameMode()
    {
        m_ObjectDispenser.Hide();
        foreach (var obj in m_ObjectsToToggle)
        {
            obj.SetActive(false);
        }
    }

    public void ShowGameMode()
    {
        m_ObjectDispenser.Show();
        foreach (var obj in m_ObjectsToToggle)
        {
            obj.SetActive(true);
        }
    }

    public void OnGameModeEnd() { }

    public void OnGameModeStart() { }
}
