using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using TMPro;

public class Stage : MonoBehaviour
{
    [Header("Source")]
    public GameObject tilePrefab;
    public Transform backgroundNode;
    public Transform boardNode;
    public Transform tetrisNode;
    public GameObject gameoverPanel;

    public TextMeshProUGUI score;
    public TextMeshProUGUI level;
    public TextMeshProUGUI line;

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


    // UI 관련 설정들
    private int scoreVal = 0;
    private int levelVal = 1;
    private int lineVal; 
    private void Start()
    {
        // 게임 시작시 UI 텍스트 설정
        lineVal = levelVal * 2;
        score.text = "" + scoreVal;
        level.text = "" + levelVal;
        line.text = "" + lineVal;


        // 게임 시작시 게임오버 패널 off
        gameoverPanel.SetActive(false);

        halfWidth = Mathf.RoundToInt(boardWidth * 0.5f); // 너비의 중간값 설정
        halfHeight = Mathf.RoundToInt(boardHeight * 0.5f); // 높이의 중간값 설정

        nextFallTime = Time.time + fallCycle;

        CreateBackgorund(); // 배경 만들기

        // 높이만큼 행 노드 만들기
        for (int i =0; i< boardHeight; ++i)
        {
            // ToString을 이용하여 오브젝트 이름 설정
            var col = new GameObject((boardHeight - i - 1).ToString());
            // 위치 설정 >> 행 위치의 높이, 가로 중앙으로
            col.transform.position = new Vector3(0, halfHeight - i, 0);
            // 보드 노드의 자식으로 설정
            col.transform.parent = boardNode;
        }

        CreateTetris(); //테트리스 생성
    }

    private void Update()
    {
        if (gameoverPanel.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(0);
            }
        }
        else
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

            // 바닥에 닿을 때까지 이동
            if (Input.GetKeyDown(KeyCode.Space))
            {
                while (MoveTetris(Vector3.down, false))
                {

                }
            }

            // 씬을 다시 불러와 재시작
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }

            if (Time.time > nextFallTime)

                // 아래로 떨어지면 강제 이동
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

    }

    bool MoveTetris(Vector3 moveDir, bool isRotate)
    {
        // 이동, 회전 불가시 돌아가기 위한 값 저장
        Vector3 oldPos = tetrisNode.transform.position;
        Quaternion oldRot = tetrisNode.transform.rotation;

        // 이동, 회전
        tetrisNode.transform.position += moveDir;
        if (isRotate)
        {
            // 현재 테트리스 노드에 90도 회전 더하기
            tetrisNode.transform.rotation *= Quaternion.Euler(0, 0, 90);
        }

        // 이동 불가시 이전 위치, 회전 으로 돌아가기
        if (!CanMoveTo(tetrisNode))
        {
            tetrisNode.transform.position = oldPos;
            tetrisNode.transform.rotation = oldRot;

            if ((int)moveDir.y == -1 && (int)moveDir.x ==0 && isRotate == false)
            {
                AddToBoard(tetrisNode);
                CheckBoardColumn();
                CreateTetris();
            }

            // 노드 추가 직후 움직일 수 있는가?
            if (!CanMoveTo(tetrisNode))
            {
                gameoverPanel.SetActive(true);
            }

            return false;
        }

        return true;
    }

    // 테트리스노드 보드에 추가
    void AddToBoard(Transform root) // 매개변수 root로 가져오기
    {
        while (root.childCount > 0)
        {
            var node = root.GetChild(0);

            // 유니티 좌표에서 테트리스 좌표로 변환
            int x = Mathf.RoundToInt(node.transform.position.x + halfWidth);
            int y = Mathf.RoundToInt(node.transform.position.y + halfHeight - 1);

            // 부모노드 : 행 노드(y 위치), 오브젝트 이름 : x 위치
            node.parent = boardNode.Find(y.ToString());
            node.name = x.ToString();

        }
    }
    bool CanMoveTo(Transform root)
    {

        // Tetrisnode 자식 타일 모두 검사
        for (int i = 0; i < root.childCount; i++)
        {
            var node = root.GetChild(i);

            // 유니티 좌표에서 테트리스 좌표로 변환
            int x = Mathf.RoundToInt(node.transform.position.x + halfWidth);
            int y = Mathf.RoundToInt(node.transform.position.y + halfHeight - 1);

            // 이동 가능한 좌표인지 확인 후 변환
            if (x < 0 || x > boardWidth - 1)
                return false;
            if (y < 0)
                return false;

            // 이미 다른 타일이 있는지 확인
            var column = boardNode.Find(y.ToString());

            if (column != null && column.Find(x.ToString()) != null)
                return false;
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

    void CheckBoardColumn()
    {
        bool isCleared = false;

        // 한번에 사라질 행 갯수 확인
        int linecount = 0;
        // 완성된 행 == 행의 자식 갯수가 가로 크기
        foreach (Transform column in boardNode)
        {
            if (column.childCount == boardWidth)
            {
                // 행의 모든 자식 삭제
                foreach (Transform tile in column)
                {
                    Destroy(tile.gameObject);
                }
                // 행의 모든 자식들과 연결 끊기
                column.DetachChildren();
                isCleared = true;
                linecount++; // 완성된 행 하나당 linecount 증가
            }
        }
        // 완성된 행이 있을 경우 점수 추가
        if (linecount != 0)
        {
            scoreVal += linecount * linecount * 100;
            score.text = " " + scoreVal;
        }

        // 비어 있는 행이 존재하면 아래로 내리기
        if (isCleared)
        {
            // 바닥 행은 안내리기 위해 1부터 시작
            for (int i = 1; i < boardNode.childCount; ++i)
            {
                var column = boardNode.Find(i.ToString());

                // 이미 비어 있는 행은 무시함
                if (column.childCount == 0)
                    continue;

                // 현재 행 아래쪽에 빈 행이 존재하는가? 빈 행만큼 emptyCol 증가
                int emptyCol = 0;
                int j = i - 1;
                while (j >= 0)
                {
                    if (boardNode.Find(j.ToString()).childCount == 0)
                    {
                        emptyCol++;
                    }
                    j--;
                }

                // 현재 행 아래쪽에 빈 행이 존재시 아래로 내리기
                if (emptyCol > 0)
                {
                    var targetColumn = boardNode.Find((i - emptyCol).ToString());

                    while (column.childCount > 0)
                    {
                        Transform tile = column.GetChild(0);
                        tile.parent = targetColumn;
                        tile.transform.position += new Vector3(0, -emptyCol, 0);

                    }
                    column.DetachChildren();

                }

            }


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
