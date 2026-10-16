using UnityEngine;

public class Stage : MonoBehaviour
{
    public GameObject tilePrefab;
    public Transform backgroundNode;
    public Transform boardNode;
    public Transform tetrisNode;

    //높이 설정
    public int boardWidth = 10;
    public int boardHeight = 20;
    // 떨어지는 속도
    public float fallCycle = 1.0f;
    // 위치 보정
    public float offset_x = 0f;
    public float offset_y = 0f;


    private float nextFallTime; // 다음 테트리스 떨어질 시간
    private int halfWidth;
    private int halfHeight;

    private void Start()
    {
        halfWidth = Mathf.RoundToInt(boardWidth * 0.5f); // 너비의 중간값 설정
        halfHeight = Mathf.RoundToInt(boardHeight * 0.5f); // 높이의 중간값 설정

        nextFallTime = Time.time + fallCycle;

        CreateBackgorund(); // 배경 만들기
        CreateTetris(); //테트리스 생성
    }

    private void Update()
    {
        Vector3 moveDir = Vector3.zero; // 이동 저장
        bool isRotate = false; // 회전 저장

        if (Input.GetKeyDown(KeyCode.A))
        {
            moveDir.x = -1;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            moveDir.x = 1;
        }
        
        if (Input.GetKeyDown(KeyCode.W))
        {
            isRotate = true;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            moveDir.y = -1;
        }

        if (moveDir != Vector3.zero || isRotate)
        {
            MoveTetris(moveDir, isRotate);
        }

        // 아래로 떨어지는건 강제로 이동
        if (Time.time > nextFallTime)
        {
            nextFallTime = Time.time + fallCycle; // 다음 노드 떨어질 시간 재설정
            moveDir.y = -1; //아래로 한 칸 이동
            isRotate = false; // 강제 이동시 회전 없음
        }

        // 아무런 입력이 없을 시 테트리스 노드 움직이지 않음.
        if (moveDir != Vector3.zero || isRotate)
        {
            MoveTetris(moveDir, isRotate);
        }

    }

    bool MoveTetris(Vector3 moveDir, bool isRotate)
    {

        tetrisNode.transform.position += moveDir;
        if (isRotate)
        {
            // 현재 테트리스 노드에 90도 회전 +
            tetrisNode.transform.rotation *= Quaternion.Euler(0, 0, 90);
        }
        return true;
    }

    // 타일 생성
    Tile CreateTile(Transform parent, Vector2 position, Color color, int order = 1)
    {
        var go = Instantiate(tilePrefab); // tilePrefab을 복제한 오브젝트 생성
        go.transform.parent = parent; // 부모 지정
        go.transform.localPosition = position; // 위치지정

        var tile = go.GetComponent<Tile>(); // tilePrefab의 Tile 스크립트 불러오기
        tile.color = color;
        tile.sortingOrder = order;

        return tile;
    }

    // 배경 타일 생성
    void CreateBackgorund()
    {
        // 배경 색 설정
        Color color = Color.gray;

        // 타일 보드
        color.a = 0.5f; // 테두리와 구분 위해 투명도 바꾸기
        for (int x = -halfWidth; x < halfWidth; x++)
        {
            for (int y = -halfHeight; y > -halfHeight; --y)
            {
                CreateTile(backgroundNode, new Vector2(x, y), color, 0);
            }
        }

        // 좌우 테두리
        color.a = 1.0f;
        for (int y = halfHeight; y > -halfHeight; --y)
        {
            CreateTile(backgroundNode, new Vector2(-halfWidth - 1, y), color, 0);
            CreateTile(backgroundNode, new Vector2(halfWidth, y), color, 0);
        }

        // 아래 테두리
        for (int x = -halfWidth - 1; x <= halfWidth; ++x)
        {
            CreateTile(backgroundNode, new Vector2(x, -halfHeight), color, 0);
        }
    }

    void CreateTetris()
    {
        int index = Random.Range(0, 7); // 랜덤으로 0~6 사이의 값

        Color32 color = Color.white;

        // 회전 계산
        tetrisNode.rotation = Quaternion.identity;
        // 테트리스 생성 위치 (중앙상단)
        tetrisNode.position = new Vector2(offset_x, halfHeight + offset_y);

        switch (index)
        {
            case 0: // I
                color = new Color32(115, 251, 253, 255);    // 하늘색
                CreateTile(tetrisNode, new Vector2(-2f, 0.0f), color);
                CreateTile(tetrisNode, new Vector2(-1f, 0.0f), color);
                CreateTile(tetrisNode, new Vector2(0f, 0.0f), color);
                CreateTile(tetrisNode, new Vector2(1f, 0.0f), color);
                break;

            case 1: // J
                color = new Color32(0, 33, 245, 255);    // 파란색
                CreateTile(tetrisNode, new Vector2(-1f, 0.0f), color);
                CreateTile(tetrisNode, new Vector2(0f, 0.0f), color);
                CreateTile(tetrisNode, new Vector2(1f, 0.0f), color);
                CreateTile(tetrisNode, new Vector2(-1f, 1.0f), color);
                break;

            case 2: // L
                color = new Color32(243, 168, 59, 255);    // 주황색
                CreateTile(tetrisNode, new Vector2(-1f, 0.0f), color);
                CreateTile(tetrisNode, new Vector2(0f, 0.0f), color);
                CreateTile(tetrisNode, new Vector2(1f, 0.0f), color);
                CreateTile(tetrisNode, new Vector2(1f, 1.0f), color);
                break;

            case 3: // O 
                color = new Color32(255, 253, 84, 255);    // 노란색
                CreateTile(tetrisNode, new Vector2(0f, 0f), color);
                CreateTile(tetrisNode, new Vector2(1f, 0f), color);
                CreateTile(tetrisNode, new Vector2(0f, 1f), color);
                CreateTile(tetrisNode, new Vector2(1f, 1f), color);
                break;

            case 4: //  S
                color = new Color32(117, 250, 76, 255);    // 녹색
                CreateTile(tetrisNode, new Vector2(-1f, -1f), color);
                CreateTile(tetrisNode, new Vector2(0f, -1f), color);
                CreateTile(tetrisNode, new Vector2(0f, 0f), color);
                CreateTile(tetrisNode, new Vector2(1f, 0f), color);
                break;

            case 5: //  T
                color = new Color32(155, 47, 246, 255);    // 자주색
                CreateTile(tetrisNode, new Vector2(-1f, 0f), color);
                CreateTile(tetrisNode, new Vector2(0f, 0f), color);
                CreateTile(tetrisNode, new Vector2(1f, 0f), color);
                CreateTile(tetrisNode, new Vector2(0f, 1f), color);
                break;

            case 6: // Z
                color = new Color32(235, 51, 35, 255);    // 빨간색
                CreateTile(tetrisNode, new Vector2(-1f, 1f), color);
                CreateTile(tetrisNode, new Vector2(0f, 1f), color);
                CreateTile(tetrisNode, new Vector2(0f, 0f), color);
                CreateTile(tetrisNode, new Vector2(1f, 0f), color);
                break;


        }
    }
}
