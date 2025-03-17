using UnityEngine;

public class EatMechanic : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            enemy.EnemyKill();
            Debug.Log("You Eat enemy");
        }
    }
}
