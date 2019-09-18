using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Bullet : NetworkBehaviour
{

    [SerializeField] private float bulletSpeed;

    private Vector3 direction;
    public Vector3 Direction { set { direction = value; } }

    void Update()
    {
        if (isServer)
        {
            transform.position += direction * bulletSpeed * Time.deltaTime;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            other.GetComponent<Player>().Cmd_Die();
            Destroy(gameObject);
        }
        else if (other.transform.tag == "Level")
        {
            Destroy(gameObject);
        }
    }
}
