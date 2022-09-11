using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;

public class PlayerShooter : NetworkBehaviour
{
    public Transform firePoint;
    public GameObject flashPoint;
    public GameObject bulletPrefab;
    public float fireRate;
    public int maxAmmo;
    [SerializeField]
    private int remainAmmo;
    private float nextTimeOfFire = 0f;
    private bool canShoot;

    private void Start()
    {
        remainAmmo = maxAmmo;
    }
    private void Update()
    {
        Shoot();

        if(remainAmmo == 0)
        {

        }
    }
    void Shoot()
    {
        if (!IsOwner)
            return;
        if (remainAmmo > 0)
        {
            canShoot = true;
        }
        else
        {
            canShoot = false;
        }
        if (Input.GetKey(KeyCode.Mouse0))
        {            
            if (Time.time >= nextTimeOfFire)
            {
                if (canShoot)
                {
                    SpawnBulletServerRpc();
                    //StartCoroutine(WeaponFlash());
                    //if (IsClient)
                    //    SpawnBulletClientRpc();
                    //if (remainAmmo > 0)
                    //remainAmmo--;
                    //else if (remainAmmo <= 0)
                    //remainAmmo = 0;
                    //Debug.Log("Firing");
                    
                }
                nextTimeOfFire = Time.time + 1 / fireRate;
            }
        }
    }
    //IEnumerator WeaponFlash()
    //{
    //    flashPoint.SetActive(true);
    //    yield return new WaitForSeconds(0.1f);
    //    flashPoint.SetActive(false);
    //}

    [ServerRpc(Delivery = RpcDelivery.Reliable)]
    void SpawnBulletServerRpc()
    {
        NetworkObject bullet = Instantiate(bulletPrefab, firePoint.transform.position /*+ new Vector3(Random.Range(-0.05f, 0.05f), 0, 0)*/, firePoint.transform.rotation).GetComponent<NetworkObject>();
        bullet.SpawnWithOwnership(OwnerClientId);
    }
    [ClientRpc(Delivery = RpcDelivery.Reliable)]
    void SpawnBulletClientRpc()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.transform.position /*+ new Vector3(Random.Range(-0.05f, 0.05f), 0, 0)*/, firePoint.transform.rotation);
    }
}
