using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Bullet : MonoBehaviour
{

    [SerializeField] private float bulletSpeed;

    private Vector3 direction;
    public Vector3 Direction { set { direction = value; } }

    void Update()
    {
        transform.Translate(direction * bulletSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            other.GetComponent<Player>().Cmd_Die();
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
