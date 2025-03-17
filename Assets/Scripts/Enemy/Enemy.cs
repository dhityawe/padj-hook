using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private Transform waypoint;

    [SerializeField]
    private float speed = 1f;

    private Transform hook;

    private Vector2 playerPosition;

    private bool isHooked = false;

    public void SetWaypoint(Transform waypoint)
    {
        this.waypoint = waypoint;
    }

    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }

    public Transform GetWaypoint()
    {
        return waypoint;
    }

    public float GetSpeed()
    {
        return speed;
    }

    public void OnSpawn()
    {
        waypoint.parent = null;
        isHooked = false;
    }

    private void Awake()
    {
        OnSpawn();
    }

    private void Update()
    {
        Move();
        Hooked();
    }

    private void Move()
    {
        if (isHooked) return;

        transform.position = Vector3.MoveTowards(transform.position, waypoint.position, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, waypoint.position) < 0.1f)
        {
            waypoint.parent = transform;

            EnemyPool.DestroyEnemy(this);
        }
    }

    private void Hooked()
    {
        if (!isHooked) return;

        transform.position = hook.position;
    }

    public void Hook(Transform hook, Vector2 playerPosition)
    {
        this.hook = hook;
        this.playerPosition = playerPosition;
        isHooked = true;
    }

    public void EnemyKill()
    {
        EnemyPool.DestroyEnemy(this);
    }
}