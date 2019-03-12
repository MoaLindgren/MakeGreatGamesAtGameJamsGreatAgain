using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProjectile
{
    void ShootMe();
}

public class ProjectileScript : MonoBehaviour, IProjectile
{
    [SerializeField]
    MeshRenderer projectileMesh;

    Vector3 direction = Vector3.zero;

    TankScript shooter;

    float speed = 0.0f;

    int damage;

    public void Init(Vector3 direction, float speed, TankScript parent, int damage)
    {
        this.direction = direction;
        this.speed = speed;
        this.shooter = parent;
        this.damage = damage;
        Destroy(gameObject, 15);
    }

    void Update()
    {
        if (GameManager.Instance.Paused)
            return;
        transform.Translate(direction * speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        TankScript hitTank = other.gameObject.GetComponent<TankScript>();
        if (hitTank != null && hitTank != shooter)
        {
            hitTank.TakeDamage(damage);
            Destroy(gameObject);
        }
        if (hitTank == null || hitTank != shooter)
        {
            Destroy(gameObject);
        }
    }

    public void Render(bool render)
    {
        //projectileMesh.
    }

    public void ShootMe()
    {
        return;
    }
}