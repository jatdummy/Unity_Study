using System.Collections;
using UnityEngine;

public class Explosive : MonoBehaviour
{
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private float explosionDelayTime = 0.01f;
    [SerializeField] private float explosionRadius = 10f;
    [SerializeField] private float explosionForce = 500f;
    [SerializeField] private int maxHP = 100;
    private int currentHP;
    private bool isExploded = false;

    private void Awake()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;

        if ( currentHP <= 0 && isExploded == false)
        {
            StartCoroutine(nameof(OnExplosion));
        }
    }

    private IEnumerator OnExplosion()
    {
        yield return new WaitForSeconds(explosionDelayTime);

        // StackOverflow 방지용
        isExploded = true;

        if (explosionPrefab != null)
        {
            Bounds bounds = GetComponent<Collider>().bounds;
            Instantiate(explosionPrefab, bounds.center, transform.rotation);

        }

        // 폭발 범위에 있는 모든 오브젝트의 Collider 정보를 받아와 폭발 효과를 처리함.
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hit in colliders)
        {
            // 플레이어, 적 등 체력이 감소하거나 추가적인 처리시

            // 폭발 범위에 충돌한 오브젝트 종류에 따라 처리
            if (hit.TryGetComponent<Rigidbody>(out var rigidbody))
            {
                rigidbody.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
        }

        Destroy(gameObject);
    
    }


}
