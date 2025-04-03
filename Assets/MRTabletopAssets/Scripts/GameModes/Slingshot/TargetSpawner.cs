using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
public class TargetSpawner : NetworkBehaviour
{
    public Action<Color> OnTargetHit;
    [SerializeField] AudioSource m_TargetAudio;
    [SerializeField] AudioClip[] m_TargetHitSounds;
    [SerializeField] AudioClip[] m_TargetBadDestroySounds;
    [SerializeField] AudioClip[] m_TargetSpawnSounds;
    [SerializeField] Color[] m_DestroyColors;
    [SerializeField] GameObject targetPrefab;
    [SerializeField] List<Transform> m_SpawnPoints;

    public Vector2 spawnIntervalMinMax { get => m_SpawnIntervalMinMax; set => m_SpawnIntervalMinMax = value; }
    [SerializeField] Vector2 m_SpawnIntervalMinMax = new Vector2(2f, 5f); // Default interval of 5 seconds

    List<ITarget> m_Targets = new List<ITarget>();

    private bool m_IsSpawning = false;
    private float m_Timer = 0f;

    float m_SpawnInteval;

    void Update()
    {
        if (!IsServer || !m_IsSpawning)
            return;

        m_Timer += Time.deltaTime;
        if (m_Timer >= m_SpawnInteval)
        {
            SpawnTargetsRpc(GetSpawnValues());
        }
    }

    IEnumerator ClearAllTargets()
    {
        if (m_Targets.Count == 0)
            yield break;

        // Fisher-Yates shuffle algorithm to randomize the m_Targets list
        for (int i = m_Targets.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            ITarget temp = m_Targets[i];
            m_Targets[i] = m_Targets[j];
            m_Targets[j] = temp;
        }
        for (int i = m_Targets.Count - 1; i >= 0; i--)
        {
            if (i >= m_Targets.Count)
                continue;

            if (m_Targets[i] != null)
                m_Targets[i].OnHit(m_DestroyColors[UnityEngine.Random.Range(0, m_DestroyColors.Length)]);
            else
                Debug.Log("Target is null");
            if (m_Targets.Count > 16)
                yield return new WaitForSeconds(.01f);
            else if (m_Targets.Count > 8)
                yield return new WaitForSeconds(.05f);
            else if (m_Targets.Count > 4)
                yield return new WaitForSeconds(.1f);
        }
        m_Targets.Clear();
    }

    [Rpc(SendTo.Everyone)]
    public void SpawnTargetsRpc(int[] spawnIndexes)
    {
        m_SpawnInteval = UnityEngine.Random.Range(m_SpawnIntervalMinMax.x, m_SpawnIntervalMinMax.y);

        for (int i = 0; i < spawnIndexes.Length; i++)
        {
            ITarget target = Instantiate(targetPrefab, m_SpawnPoints[spawnIndexes[i]].position, m_SpawnPoints[spawnIndexes[i]].rotation).GetComponent<ITarget>();
            m_TargetAudio.transform.position = ((BalloonTarget)target).transform.position;
            m_TargetAudio.PlayOneShot(m_TargetSpawnSounds[UnityEngine.Random.Range(0, m_TargetHitSounds.Length)]);
            m_Targets.Add(target);
            target.OnHitAction += (Color c) => TargetHit(target, c);
        }
    }

    void TargetHit(ITarget target, Color c)
    {
        OnTargetHit?.Invoke(c);
        m_TargetAudio.transform.position = ((BalloonTarget)target).transform.position;
        if (c == Color.white)
            m_TargetAudio.PlayOneShot(m_TargetBadDestroySounds[UnityEngine.Random.Range(0, m_TargetBadDestroySounds.Length)]);
        else
            m_TargetAudio.PlayOneShot(m_TargetHitSounds[UnityEngine.Random.Range(0, m_TargetHitSounds.Length)]);

        m_Targets.Remove(target);

    }

    int[] GetSpawnValues()
    {
        m_Timer = 0f;
        float randomValue = UnityEngine.Random.value;

        int[] spawnIndexes = new int[1];

        if (randomValue > .25f)
        {
            spawnIndexes = new int[2];
        }
        else if (randomValue > .5f)
        {
            spawnIndexes = new int[3];
        }
        else if (randomValue > .75f)
        {
            spawnIndexes = new int[4];
        }

        List<int> availableSpawnPoints = new List<int>();
        for (int i = 0; i < m_SpawnPoints.Count; i++)
        {
            availableSpawnPoints.Add(i);
        }
        for (int i = 0; i < spawnIndexes.Length; i++)
        {
            spawnIndexes[i] = availableSpawnPoints[UnityEngine.Random.Range(0, availableSpawnPoints.Count)];
            availableSpawnPoints.Remove(spawnIndexes[i]);
        }

        return spawnIndexes;
    }

    [ContextMenu("Start Spawning")]
    public void StartSpawning()
    {
        m_IsSpawning = true;
        m_SpawnInteval = 0;
        m_Timer = 0f;
    }

    [ContextMenu("Stop Spawning")]
    public void StopSpawning()
    {
        if (m_IsSpawning)
        {
            m_IsSpawning = false;
        }
        StartCoroutine(ClearAllTargets());
    }
}
