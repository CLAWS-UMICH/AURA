using UnityEngine;

namespace XRMultiplayer
{
    public class PlayerListInitializer : MonoBehaviour
    {
        [SerializeField] PlayerListUI[] m_PlayerListUIs;

        void Start()
        {
            foreach (var l in m_PlayerListUIs)
            {
                l.InitializeCallbacks();
            }
        }

        [ContextMenu("Find Player List UIs")]
        void FindPlayerListUIs()
        {
            m_PlayerListUIs = FindObjectsByType<PlayerListUI>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        }
    }
}
