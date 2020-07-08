using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using GameObject = UnityEngine.GameObject;
using System;

public class TrackManager : MonoBehaviour
{
    static public TrackManager instance { get { return s_Instance; } }
    static protected TrackManager s_Instance;

    public delegate int MultiplierModifier(int current);
    public MultiplierModifier modifyMultiply;

    [Header("Character & Movements")]
    public Player player;
    public float minSpeed = 8.0f;
    public float maxSpeed = 50.0f;
    public int speedStep = 4;
    public float laneOffset = 1.0f;

    public bool invincible = false;

    public System.Action<TrackSegment> newSegmentCreated;
    public System.Action<TrackSegment> currentSegementChanged;

    public float timeToStart { get { return m_TimeToStart; } } 

    public int score { get { return m_Score; } }
    public float worldDistance { get { return m_TotalWorldDistance; } }
    public float speed { get { return m_Speed; } }
    public float speedRatio { get { return (m_Speed - minSpeed) / (maxSpeed - minSpeed); } }
    public int currentZone { get { return m_CurrentZone; } }

    public TrackSegment currentSegment { get { return m_Segments[0]; } }

    public bool isMoving { get { return m_IsMoving; } }
    public bool isRerun { get { return m_Rerun; } set { m_Rerun = value; } }
    public bool isLoaded { get; set; } 

    protected float m_TimeToStart = -1.0f;

    protected float m_CurrentSegmentDistance;
    protected float m_TotalWorldDistance;
    protected bool m_IsMoving;
    protected float m_Speed;

    protected List<TrackSegment> m_Segments = new List<TrackSegment>();
    protected List<TrackSegment> m_PastSegments = new List<TrackSegment>();
    protected int m_SafeSegementLeft;

    public ThemeData m_CurrentThemeData;
    protected int m_CurrentZone;
    protected float m_CurrentZoneDistance;
    protected int m_PreviousSegment = -1;

    protected int m_Score;
    protected float m_ScoreAccum;
    protected bool m_Rerun;     // This lets us know if we are entering a game over (ads) state or starting a new game (see GameState)

    Vector3 m_CameraOriginalPos = Vector3.zero;

    const float k_FloatingOriginThreshold = 10000f;

    protected const float k_CountdownToStartLength = 2f;
    protected const float k_CountdownSpeed = 1.5f;
    protected const float k_StartingSegmentDistance = 1f;
    protected const int k_DesiredSegmentCount = 10;
    protected const float k_SegmentRemovalDistance = -30f;
    protected const float k_Acceleration = .5f;

    protected void Awake()
    {
        m_ScoreAccum = 0.0f;
        s_Instance = this;
    }

    public void StartMove(bool isRestart = true)
    {
        player.StartMoving();
        m_IsMoving = true;
        if (isRestart)
            m_Speed = minSpeed;
    }

    public void StopMove()
    {
        m_IsMoving = false;
    }

    IEnumerator WaitToStart()
    {
        //TestDebug.Debugging("wait to start");

        float length = k_CountdownToStartLength;
        m_TimeToStart = length;

        while (m_TimeToStart >= 0)
        {
            yield return null;
            m_TimeToStart -= Time.deltaTime * k_CountdownSpeed;
        }

        m_TimeToStart = -1;

        if (m_Rerun)
        {
            player.characterCollider.SetInvincible();
        }

        player.StartRunning();
        StartMove();
    }

    public IEnumerator Begin()
    {
        if (!m_Rerun)
        {
            m_CameraOriginalPos = Camera.main.transform.position;

            // Since this is not a rerun, init the whole system (on rerun we want to keep the states we had on death)
            m_CurrentSegmentDistance = k_StartingSegmentDistance;
            m_TotalWorldDistance = 0.0f;

            this.player.gameObject.SetActive(true);

            //Addressables 1.0.1-preview
            // Spawn the player
            //Addressables.InstantiateAsync(PlayerData.instance.characters[PlayerData.instance.usedCharacter],
            //    new Vector3(0, 0.5f, 0),
            //    Quaternion.identity)
            //    .Completed += OnLoadDone;
            //TestDebug.Debugging("Begin before instantiating character");

            var chr = CharacterDatabase.GetCharacter(PlayerData.instance.characters[PlayerData.instance.usedCharacter]);
            //TestDebug.Debugging(chr.characterName);

            Character character = Instantiate(chr, Vector3.zero, Quaternion.identity);

            //TestDebug.Debugging(character.characterName);

            player.character = character;
            player.trackManager = this;

            player.Init();
            player.CheatInvincible(invincible);

            character.transform.SetParent(player.characterCollider.transform, false);
            Camera.main.transform.SetParent(player.transform, true);

            m_CurrentZone = 0;
            m_CurrentZoneDistance = 0;

            RenderSettings.fog = true;

            gameObject.SetActive(true);
            this.player.gameObject.SetActive(true);

            m_Score = 0;
            m_ScoreAccum = 0;
        }

        yield return new WaitForSeconds(0.5f);

        try {
            player.Begin();

            StartCoroutine(WaitToStart());
            isLoaded = true;
        }
        catch (Exception ex)
        {
            TestDebug.Debugging(ex.Message);
        }
    }

    //private void OnLoadDone(AsyncOperationHandle<GameObject> obj)
    //{
    //    TestDebug.Debugging("Completed");

    //    character =  obj.Result.GetComponent<Character>();

    //    player.character = character;
    //    player.trackManager = this;

    //    player.Init();
    //    player.CheatInvincible(invincible);

    //    character.transform.SetParent(player.characterCollider.transform, false);
    //    Camera.main.transform.SetParent(player.transform, true);

    //    m_CurrentZone = 0;
    //    m_CurrentZoneDistance = 0;

    //    RenderSettings.fog = true;

    //    gameObject.SetActive(true);
    //    this.player.gameObject.SetActive(true);

    //    m_Score = 0;
    //    m_ScoreAccum = 0;
    //}

    public void End()
    {
        foreach (TrackSegment seg in m_Segments)
        {
            Addressables.ReleaseInstance(seg.gameObject);
            _spawnedSegments--;
        }

        for (int i = 0; i < m_PastSegments.Count; ++i)
        {
            Addressables.ReleaseInstance(m_PastSegments[i].gameObject);
        }

        m_Segments.Clear();
        m_PastSegments.Clear();

        gameObject.SetActive(false);
        Addressables.ReleaseInstance(player.character.gameObject);
        player.character = null;

        Camera.main.transform.SetParent(null);
        Camera.main.transform.position = m_CameraOriginalPos;

        player.gameObject.SetActive(false);
    }

    private int _spawnedSegments = 0;

    void Update()
    {
        while (_spawnedSegments < k_DesiredSegmentCount)
        {
            StartCoroutine(SpawnNewSegment());
            _spawnedSegments++;
        }

        if (!m_IsMoving)
            return;

        float scaledSpeed = m_Speed * Time.deltaTime;
        m_ScoreAccum += scaledSpeed;
        m_CurrentZoneDistance += scaledSpeed;

        int intScore = Mathf.FloorToInt(m_ScoreAccum);
        if (intScore != 0) AddScore(intScore);
        m_ScoreAccum -= intScore;

        m_TotalWorldDistance += scaledSpeed;
        m_CurrentSegmentDistance += scaledSpeed;

        if (m_CurrentSegmentDistance > m_Segments[0].worldLength)
        {
            m_CurrentSegmentDistance -= m_Segments[0].worldLength;

            m_PastSegments.Add(m_Segments[0]);
            m_Segments.RemoveAt(0);
            _spawnedSegments--;

            if (currentSegementChanged != null) currentSegementChanged.Invoke(m_Segments[0]);
        }

        Vector3 currentPos;
        Quaternion currentRot;
        Transform characterTransform = player.transform;

        m_Segments[0].GetPointAtInWorldUnit(m_CurrentSegmentDistance, out currentPos, out currentRot);

        bool needRecenter = currentPos.sqrMagnitude > k_FloatingOriginThreshold;

        if (needRecenter)
        {
            int count = m_Segments.Count;
            for (int i = 0; i < count; i++)
            {
                m_Segments[i].transform.position -= currentPos;
            }

            count = m_PastSegments.Count;
            for (int i = 0; i < count; i++)
            {
                m_PastSegments[i].transform.position -= currentPos;
            }

            m_Segments[0].GetPointAtInWorldUnit(m_CurrentSegmentDistance, out currentPos, out currentRot);
        }

        characterTransform.rotation = currentRot;
        characterTransform.position = currentPos;

        for (int i = 0; i < m_PastSegments.Count; ++i)
        {
            if ((m_PastSegments[i].transform.position - currentPos).z < k_SegmentRemovalDistance)
            {
                m_PastSegments[i].Cleanup();
                m_PastSegments.RemoveAt(i);
                i--;
            }
        }

        if (m_Speed < maxSpeed)
            m_Speed += k_Acceleration * Time.deltaTime;
        else
            m_Speed = maxSpeed;
    }

    private readonly Vector3 _offScreenSpawnPos = new Vector3(-100f, -100f, -100f);
    public IEnumerator SpawnNewSegment()
    {
        int segmentUse = UnityEngine.Random.Range(0, m_CurrentThemeData.zones[m_CurrentZone].prefabList.Length);
        if (segmentUse == m_PreviousSegment) segmentUse = (segmentUse + 1) % m_CurrentThemeData.zones[m_CurrentZone].prefabList.Length;

        AsyncOperationHandle segmentToUseOp = m_CurrentThemeData.zones[m_CurrentZone].prefabList[segmentUse].InstantiateAsync(_offScreenSpawnPos, Quaternion.identity);
        yield return segmentToUseOp;
        if (segmentToUseOp.Result == null || !(segmentToUseOp.Result is GameObject))
        {
            Debug.LogWarning(string.Format("Unable to load segment {0}.", m_CurrentThemeData.zones[m_CurrentZone].prefabList[segmentUse].Asset.name));
            yield break;
        }
        TrackSegment newSegment = (segmentToUseOp.Result as GameObject).GetComponent<TrackSegment>();

        Vector3 currentExitPoint;
        Quaternion currentExitRotation;

        if (m_Segments.Count > 0)
        {
            m_Segments[m_Segments.Count - 1].GetPointAt(1.0f, out currentExitPoint, out currentExitRotation);
        }
        else
        {
            currentExitPoint = transform.position;
            currentExitRotation = transform.rotation;
        }

        newSegment.transform.rotation = currentExitRotation;

        Vector3 entryPoint;
        Quaternion entryRotation;
        newSegment.GetPointAt(0.0f, out entryPoint, out entryRotation);


        Vector3 pos = currentExitPoint + (newSegment.transform.position - entryPoint);
        newSegment.transform.position = pos;
        newSegment.manager = this;

        newSegment.transform.localScale = new Vector3((UnityEngine.Random.value > 0.5f ? -1 : 1), 1, 1);
        newSegment.objectRoot.localScale = new Vector3(1.0f / newSegment.transform.localScale.x, 1, 1);

        if (m_SafeSegementLeft <= 0)
        {
            SpawnObstacle(newSegment);
        }
        else
            m_SafeSegementLeft -= 1;
        m_Segments.Add(newSegment);

        if (newSegmentCreated != null) newSegmentCreated.Invoke(newSegment);
    }

    public void SpawnObstacle(TrackSegment segment)
    {
        if (segment.possibleObstacles.Length != 0)
        {
            for (int i = 0; i < segment.obstaclePositions.Length; ++i)
            {
                AssetReference assetRef = segment.possibleObstacles[UnityEngine.Random.Range(0, segment.possibleObstacles.Length)];
                StartCoroutine(SpawnFromAssetReference(assetRef, segment, i));
            }
        }
    }

    private IEnumerator SpawnFromAssetReference(AssetReference reference, TrackSegment segment, int posIndex)
    {
        AsyncOperationHandle op = reference.LoadAssetAsync<GameObject>();
        yield return op;
        GameObject obj = op.Result as GameObject;
        if (obj != null)
        {
            Obstacle obstacle = obj.GetComponent<Obstacle>();
            if (obstacle != null)
                yield return obstacle.Spawn(segment, segment.obstaclePositions[posIndex]);
        }
    }

    public void AddScore(int amount)
    {
        int finalAmount = amount;
        m_Score += finalAmount;
    }
}
