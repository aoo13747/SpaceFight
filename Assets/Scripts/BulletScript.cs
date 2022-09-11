using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;

public class BulletScript : NetworkBehaviour
{
    Rigidbody rb;
    public float speed;
    public int damage;    
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * speed* Time.deltaTime,ForceMode.VelocityChange);
        Invoke("DestroyBullet",5f);
    }    
    private void Update()
    {
        
    }

    [ServerRpc]
    void DestroyBulletServerRpc()
    {
        Destroy(gameObject);
    }
    public void DestroyBullet()
    {
        DestroyBulletServerRpc();
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.tag);
        if (IsOwner)
            return;
        if (other.CompareTag("Player") && !IsLocalPlayer)
        {
            Debug.Log("hit player");
            other.GetComponent<PlayerStats>().TakeDamageServerRpc(damage);            
            Debug.Log("Damage Take");
            DestroyBullet();
            
        }
    }    
}
