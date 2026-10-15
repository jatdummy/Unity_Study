using UnityEngine;

public class ProjectileExplosive : MonoBehaviour
{
    private Explosive explosive;

    private void Awake()
    {
        explosive = GetComponent<Explosive>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        explosive.TakeDamage(10000);
    }
}
