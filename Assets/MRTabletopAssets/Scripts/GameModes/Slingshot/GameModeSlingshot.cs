using System.Collections;
using MRTTT;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using XRMultiplayer;

public class GameModeSlingshot : NetworkBehaviour, IGameMode
{
    // Added as a workaround for network variables not synchronizing when joining a new network session
    protected override void OnNetworkPreSpawn(ref NetworkManager networkManager)
    {
        // This handles resetting the NetworkList (NetworkVariableBase) for the scenario
        // where you have a NetworkVariable or NetworkList on an in-scene placed NetworkObject
        // and you can start and stop a network session without reloading the scene. Invoking
        // the Initialize method assures that the last time sent update is reset.
        m_CurrentLife.Initialize(this);
        m_GameActive.Initialize(this);
        base.OnNetworkPreSpawn(ref networkManager);
    }

    const int k_MaxLife = 3;
    NetworkVariable<int> m_CurrentLife = new NetworkVariable<int>(k_MaxLife);

    NetworkVariable<bool> m_GameActive = new NetworkVariable<bool>(false);
    /// <summary>
    /// The ID of the game mode. Used to order the game modes in the game mode manager.
    /// </summary>
    public int gameModeID => m_GameModeID;
    [SerializeField]
    int m_GameModeID = 2;

    float m_CurrentTime = 0.0f;
    [SerializeField] TMP_Text m_TimeText;

    int m_CurrentScore = 0;
    [SerializeField]
    TMP_Text m_ScoreText;

    float m_MaxTime = 0;
    [SerializeField]
    TMP_Text m_MaxTimeText;

    int m_MaxScore = 0;
    [SerializeField]
    TMP_Text m_MaxScoreText;

    [SerializeField]
    [Range(.8f, 2.0f)]
    float m_DifficultySpawnMultiplier = 1.0f;

    [SerializeField]
    float m_PlayerCountDifficultyMultiplier = 0.1f;

    [SerializeField]
    int m_TimeIncrementInSeconds = 15;
    float m_CurrentTimeIncrement = 0.0f;

    [SerializeField]
    Image m_CurrentLifeImage;

    [SerializeField]
    GameObject[] m_ObjectsToToggle;

    [SerializeField]
    SlingshotArea[] m_SlingshotAreas;

    [SerializeField]
    TargetSpawner m_TargetSpawner;

    [SerializeField]
    GameObject m_PreGameUIObject;

    [SerializeField]
    GameObject m_InGameUIObject;

    [SerializeField]
    GameObject m_NewScoreRecordGameObject;

    [SerializeField]
    GameObject m_NewTimeRecordGameObject;

    Vector2 m_StartingSpawnIntervalMinMax;

    void Start()
    {
        HideGameMode();
        m_CurrentLife.OnValueChanged += OnLifeChanged;
        m_GameActive.OnValueChanged += OnGameActiveChanged;
        m_TargetSpawner.OnTargetHit += OnTargetHit;

        m_StartingSpawnIntervalMinMax = m_TargetSpawner.spawnIntervalMinMax;

        if (!PlayerPrefs.HasKey("SlingshotMaxScore"))
            PlayerPrefs.SetInt("SlingshotMaxScore", 0);
        if (!PlayerPrefs.HasKey("SlingshotMaxTime"))
            PlayerPrefs.SetFloat("SlingshotMaxTime", 0);

        m_MaxScore = PlayerPrefs.GetInt("SlingshotMaxScore");
        m_MaxTime = PlayerPrefs.GetFloat("SlingshotMaxTime");

        m_MaxTimeText.text = Utils.GetTimeFormatted(m_MaxTime);
        m_MaxScoreText.text = $"{m_MaxScore}";

        m_NewScoreRecordGameObject.SetActive(false);
        m_NewTimeRecordGameObject.SetActive(false);
    }

    void Update()
    {
        if (m_GameActive.Value)
        {
            m_CurrentTime += Time.deltaTime;
            m_TimeText.text = Utils.GetTimeFormatted(m_CurrentTime);

            if (IsServer)
            {
                m_CurrentTimeIncrement += Time.deltaTime;
                if (m_CurrentTimeIncrement >= m_TimeIncrementInSeconds)
                {
                    m_CurrentTimeIncrement = 0.0f;
                    m_TargetSpawner.spawnIntervalMinMax /= m_DifficultySpawnMultiplier + (Mathf.Max(4, NetworkManager.Singleton.ConnectedClients.Count) * m_PlayerCountDifficultyMultiplier);
                }
            }
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsServer)
        {
            m_GameActive.Value = false;
        }
    }
    private IEnumerator FlashLifeImageRed()
    {
        Color originalColor = m_CurrentLifeImage.color;
        float duration = 0.5f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            m_CurrentLifeImage.color = Color.Lerp(originalColor, Color.red, Mathf.PingPong(elapsedTime * 2, 1));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        m_CurrentLifeImage.color = originalColor;
    }

    private void OnTargetHit(Color color)
    {
        if (color == Color.white)    // Bad target
        {
            StartCoroutine(FlashLifeImageRed());
            if (IsServer)
            {
                m_CurrentLife.Value--;
                if (m_CurrentLife.Value <= 0)
                {
                    m_GameActive.Value = false;
                }
            }
        }

        if (color == XRINetworkGameManager.LocalPlayerColor.Value)
        {
            m_CurrentScore++;
            m_ScoreText.text = m_CurrentScore.ToString();
        }
    }

    private void OnGameActiveChanged(bool previousValue, bool newValue)
    {
        if (newValue)
        {
            OnGameModeStart();
        }
        else
        {
            OnGameModeEnd();
        }
    }

    private void OnLifeChanged(int previousValue, int newValue)
    {
        m_CurrentLifeImage.fillAmount = (float)newValue / k_MaxLife;
    }

    public void HideGameMode()
    {
        // Debug.Log($"Hiding {transform.name} Game Mode");
        if (IsServer && m_GameActive.Value)
        {
            m_GameActive.Value = false;
        }

        foreach (var obj in m_ObjectsToToggle)
        {
            obj.SetActive(false);
        }
        foreach (var area in m_SlingshotAreas)
        {
            area.HideGameMode();
        }

        m_PreGameUIObject.SetActive(false);
        m_InGameUIObject.SetActive(false);
        m_TargetSpawner.StopSpawning();
    }

    public void ShowGameMode()
    {
        // Debug.Log($"Showing {transform.name} Game Mode");
        foreach (var obj in m_ObjectsToToggle)
        {
            obj.SetActive(true);
        }
        m_PreGameUIObject.SetActive(!m_GameActive.Value);
        m_InGameUIObject.SetActive(m_GameActive.Value);
    }

    public void OnGameModeEnd()
    {
        m_TargetSpawner.StopSpawning();
        m_PreGameUIObject.SetActive(true);
        m_InGameUIObject.SetActive(false);
        m_CurrentLifeImage.fillAmount = 1.0f;

        m_TargetSpawner.spawnIntervalMinMax = m_StartingSpawnIntervalMinMax;

        if (m_CurrentScore > m_MaxScore)
        {
            m_NewScoreRecordGameObject.SetActive(true);
            m_MaxScore = m_CurrentScore;
            m_MaxScoreText.text = $"{m_CurrentScore}";
            PlayerPrefs.SetInt("SlingshotMaxScore", m_MaxScore);
        }
        else
        {
            m_NewScoreRecordGameObject.SetActive(false);
        }

        if (m_CurrentTime > m_MaxTime)
        {
            m_NewTimeRecordGameObject.SetActive(true);
            m_MaxTime = m_CurrentTime;
            m_MaxTimeText.text = $"{Utils.GetTimeFormatted(m_CurrentTime)}";
            PlayerPrefs.SetFloat("SlingshotMaxTime", m_MaxTime);
        }
        else
        {
            m_NewTimeRecordGameObject.SetActive(false);
        }
    }

    public void OnGameModeStart()
    {
        m_TargetSpawner.StartSpawning();
        m_PreGameUIObject.SetActive(false);
        m_InGameUIObject.SetActive(true);

        m_CurrentTimeIncrement = 0.0f;
        m_CurrentTime = 0.0f;
        m_TimeText.text = Utils.GetTimeFormatted(m_CurrentTime);
        m_CurrentScore = 0;
        m_ScoreText.text = m_CurrentScore.ToString();
    }

    public void StartGameButtonPressed()
    {
        StartGameRpc();
    }

    [Rpc(SendTo.Server)]
    public void StartGameRpc()
    {
        if (m_GameActive.Value)
            return;

        m_GameActive.Value = true;
        m_CurrentLife.Value = k_MaxLife;
    }
}
