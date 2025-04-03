using UnityEngine;

namespace MRTTT
{
    public class GameModeEmpty : MonoBehaviour, IGameMode
    {
        public int gameModeID => m_GameModeID;

        [SerializeField]
        int m_GameModeID = 0;


        public void OnGameModeEnd() { }

        public void OnGameModeStart() { }

        public void HideGameMode()
        {
        }
        public void ShowGameMode()
        {
        }
    }
}
