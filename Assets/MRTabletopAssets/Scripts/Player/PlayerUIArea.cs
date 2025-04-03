using UnityEngine;
using XRMultiplayer;

public class PlayerUIArea : MonoBehaviour
{
    [Header("Menu Open")]
    [SerializeField] GameObject[] m_ObjectsToHideOnMenuOpen;
    [SerializeField] GameObject[] m_ObjectsToShowOnMenuOpen;

    [Header("Menu Close")]
    [SerializeField] GameObject[] m_ObjectsToHideOnMenuClose;
    [SerializeField] GameObject[] m_ObjectsToShowOnMenuClose;

    public void MenuToggled(bool menuOpen)
    {
        if (menuOpen)
        {
            foreach (GameObject g in m_ObjectsToHideOnMenuOpen)
            {
                g.SetActive(false);
            }

            foreach (GameObject g in m_ObjectsToShowOnMenuOpen)
            {
                g.SetActive(true);
            }
        }
        else
        {
            foreach (GameObject g in m_ObjectsToHideOnMenuClose)
            {
                g.SetActive(false);
            }

            foreach (GameObject g in m_ObjectsToShowOnMenuClose)
            {
                g.SetActive(true);
            }
        }
    }
}
