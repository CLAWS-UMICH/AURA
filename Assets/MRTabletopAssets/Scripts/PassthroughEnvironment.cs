using UnityEngine;

public class PassthroughEnvironment : MonoBehaviour
{
    [SerializeField]
    Transform m_PassthroughVolumeTransform;

    [SerializeField]
    Vector3 regularScale = new Vector3(.4f, .4f, .4f);

    [SerializeField]
    Vector3 fullScale = new Vector3(10, 10, 10);

    public void SetPassthroughState(int stateIdx)
    {
        switch (stateIdx)
        {
            case 1:
                m_PassthroughVolumeTransform.gameObject.SetActive(true);
                m_PassthroughVolumeTransform.localScale = regularScale;
                break;
            case 2:
                m_PassthroughVolumeTransform.gameObject.SetActive(true);
                m_PassthroughVolumeTransform.localScale = fullScale;
                break;
            default:
                m_PassthroughVolumeTransform.gameObject.SetActive(false);
                m_PassthroughVolumeTransform.localScale = regularScale;
                break;
        }
    }
}
