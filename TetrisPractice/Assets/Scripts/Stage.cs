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

    private int halfWidth;
    private int halfHeight;

    private void Start()
    {
        halfWidth = Mathf.RoundToInt(boardWidth * 0.5f);
    }

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

}
