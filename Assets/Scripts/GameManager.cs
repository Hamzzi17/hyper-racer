using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // 프리팹
    [SerializeField] private GameObject carPrefab;
    [SerializeField] private GameObject roadPrefab;
    
    // UI 관련 코드
    [SerializeField] private MoveButton leftMoveButton;
    [SerializeField] private MoveButton rightMoveButton;
    [SerializeField] private TMP_Text gasText;
    
    // 자동차
    private CarController _carController;
    
    // 도로 오브젝트 풀
    private Queue<GameObject> _roadPool = new Queue<GameObject>();
    private int _roadPoolSize = 3;
    
    // 도로 이동
    private List<GameObject> _activeRoads = new List<GameObject>();
    
    // 만들어지는 도로의 index
    private int _roadIndex = 0;
    
    // 상태
    public enum State {Start, Play, End}

    public State GameState { get; private set; }
    
    // 싱글턴
    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        Time.timeScale = 5f;
    }
    
    private void Start()
    {
        // Road 오브젝트 풀 초기화
        InitializeRoadPool();
        
        // 게임 상태 Start로 변경
        GameState = State.Start;
        
        // 게임 시작
        StartGame();
    }

    private void Update()
    {
        switch (GameState)
        {
            case State.Start:
                break;
            case State.Play:
                // 활성화된 도로를 아래로 서서히 이동
                foreach (var activeRoad in _activeRoads)
                {
                    activeRoad.transform.Translate(-Vector3.forward * Time.deltaTime);
                }
        
                // Gas 정보 출력
                if (_carController != null) gasText.text = _carController.Gas.ToString();
                break;
            case State.End:
                break;
        }
        
    }

    private void StartGame()
    {
        // 도로 생성
        SpawnRoad(Vector3.zero);
        
        // 자동차 생성
        _carController = Instantiate(carPrefab, new Vector3(0, 0, -3f), Quaternion.identity)
            .GetComponent<CarController>();

        // Left, Right move button에 자동차 컨트롤 기능 적용
        leftMoveButton.OnMoveButtonDown += () => _carController.Move(-1f);
        rightMoveButton.OnMoveButtonDown += () => _carController.Move(1f);
        
        // 게임 상태를 Play로 벼녁ㅇ
        GameState = State.Play;
    }

    public void EndGame()
    {
        // 게임 상태 변경
        GameState = State.End;
        
        // 자동차 제거
        Destroy(_carController.gameObject);
        
        // 도로 제거
        foreach (var activeRoad in _activeRoads)
        {
            activeRoad.SetActive(false);
        }
        
        // TODO: 게임 오버 패널 표시
        
    }
    
    // 도로 생성 및 관리
    #region 도로 생성 및 관리

    private void InitializeRoadPool()
    {
        for (int i = 0; i < _roadPoolSize; i++)
        {
            GameObject road = Instantiate(roadPrefab);
            road.SetActive(false);
            _roadPool.Enqueue(road);
        }
    }
    
    /// <summary>
    /// 도로 오브젝트 풀에서 불러와 배치하는 함수
    /// </summary>

    public void SpawnRoad(Vector3 position)
    {
        GameObject road;
        if (_roadPool.Count > 0)
        {
            road = _roadPool.Dequeue();
            road.transform.position = position;
            road.SetActive(true);
        }
        else
        {
            road = Instantiate(roadPrefab, position, Quaternion.identity);
        }
        
        // 가스 아이템 생성
        if (_roadIndex > 0 && _roadIndex % 2 == 0)
        {
            Debug.Log("Spawn Gas Road Index: " + _roadIndex);
            road.GetComponent<RoadController>().SpawnGas();
        }
        
        // 활성화된 길을 움직이기 위해 List에 저장
        _activeRoads.Add(road);
        _roadIndex++;
    }
    public void DestroyRoad(GameObject road)
    {
        road.SetActive(false);
        _activeRoads.Remove(road);
        _roadPool.Enqueue(road);
    }
    
    #endregion
    
}
