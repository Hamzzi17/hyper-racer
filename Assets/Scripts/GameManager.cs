using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // 프리팹
    [SerializeField] private GameObject carPrefab;
    [SerializeField] private GameObject roadPrefab;
    
    // UI 관련 코드
    [SerializeField] private MoveButton leftMoveButton;
    [SerializeField] private MoveButton rightMoveButton;
    
    // 도로 오브젝트 풀
    private Queue<GameObject> _roadPool = new Queue<GameObject>();
    private int _roadPoolSize = 3;
    
    // 도로 이동
    private List<GameObject> _activeRoads = new List<GameObject>();
    
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
    }
    
    private void Start()
    {
        // Road 오브젝트 풀 초기화
        InitializeRoadPool();
        
        // 게임 시작
        StartGame();
    }

    private void Update()
    {
        // 활성화된 도로를 아래로 서서히 이동
        foreach (var activeRoad in _activeRoads)
        {
            activeRoad.transform.Translate(-Vector3.forward * Time.deltaTime);
        }
    }

    private void StartGame()
    {
        // 도로 생성
        SpawnRoad(Vector3.zero);
        
        // 자동차 생성
        var carController = Instantiate(carPrefab, new Vector3(0, 0, -3f), Quaternion.identity)
            .GetComponent<CarController>();

        // Left, Right move button에 자동차 컨트롤 기능 적용
        leftMoveButton.OnMoveButtonDown += () => carController.Move(-1f);
        rightMoveButton.OnMoveButtonDown += () => carController.Move(1f);
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
        if (_roadPool.Count > 0)
        {
            GameObject road = _roadPool.Dequeue();
            road.transform.position = position;
            road.SetActive(true);
            
            // 활성화된 길을 움직이기 위해 List에 저장
            _activeRoads.Add(road);
        }
        else
        {
                GameObject road = Instantiate(roadPrefab, position, Quaternion.identity);
                _activeRoads.Add(road);
        }
    }
    public void DestroyRoad(GameObject road)
    {
        road.SetActive(false);
        _activeRoads.Remove(road);
        _roadPool.Enqueue(road);
    }
    
    #endregion
    
}
