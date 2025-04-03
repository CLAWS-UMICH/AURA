using Unity.Netcode;
using XRMultiplayer;

namespace MRTTT
{
    public class GameModeManager : NetworkBehaviour
    {
        // Added as a workaround for network variables not synchronizing when joining a new network session
        protected override void OnNetworkPreSpawn(ref NetworkManager networkManager)
        {
            // This handles resetting the NetworkList (NetworkVariableBase) for the scenario
            // where you have a NetworkVariable or NetworkList on an in-scene placed NetworkObject
            // and you can start and stop a network session without reloading the scene. Invoking
            // the Initialize method assures that the last time sent update is reset.
            m_CurrentGameMode.Initialize(this);
            base.OnNetworkPreSpawn(ref networkManager);
        }

        IGameMode[] m_GameModes;
        NetworkVariable<int> m_CurrentGameMode = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        void Awake()
        {
            GetGameModes();
        }

        void GetGameModes()
        {
            m_GameModes = transform.parent.GetComponentsInChildren<IGameMode>();
            System.Array.Sort(m_GameModes, (a, b) => a.gameModeID.CompareTo(b.gameModeID));
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            foreach (var gameMode in m_GameModes)
            {
                gameMode.HideGameMode();
            }

            if (IsOwner)
            {
                m_CurrentGameMode.Value = 0;
            }

            if (m_CurrentGameMode.Value >= 0)
                OnGameModeChanged(-1, m_CurrentGameMode.Value);

            m_CurrentGameMode.OnValueChanged += OnGameModeChanged;
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            foreach (var gameMode in m_GameModes)
            {
                gameMode.HideGameMode();
            }
            m_CurrentGameMode.OnValueChanged -= OnGameModeChanged;
        }

        void OnGameModeChanged(int old, int current)
        {
            Utils.Log($"Game Mode Changed from {old} to {current}");
            for (int i = 0; i < m_GameModes.Length; i++)
            {
                if (current == i)
                    m_GameModes[i].ShowGameMode();
                else
                    m_GameModes[i].HideGameMode();
            }
        }

        public void SetGameMode(int gameModeID)
        {
            if (IsOwner)
            {
                m_CurrentGameMode.Value = gameModeID;
            }
            else
                SetGameModeRpc(gameModeID);
        }

        [Rpc(SendTo.Owner)]
        void SetGameModeRpc(int gameModeID)
        {
            SetGameMode(gameModeID);
        }
    }

    public interface IGameMode
    {
        void HideGameMode();
        void ShowGameMode();
        void OnGameModeStart();
        void OnGameModeEnd();

        int gameModeID { get; }
    }
}
