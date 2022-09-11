using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MLAPI;
using MLAPI.Messaging;

public class PlayerRespawn : NetworkBehaviour
{
    private PlayerController player;
    public Behaviour[] scripts;
    public Vector3[] spawnPoints;
    private int randomNumber;
        
    // Start is called before the first frame update
    void Start()
    {
        player = gameObject.GetComponent<PlayerController>();        
    }
    void SetPlayerState(bool state)
    {
        foreach (var script in scripts)
        {
            script.enabled = state;
        }        
    }
    IEnumerator SpawnDelay(Vector3 spawnPos)
    {
        player.enabled = false;
        SetPlayerState(false);
        yield return new WaitForSeconds(1f);
        transform.position = spawnPos;
        player.enabled = true;
        SetPlayerState(true);
    }
    Vector3 GetRandPos()
    {
        return new Vector3(spawnPoints[randomNumber].x,spawnPoints[randomNumber].y,spawnPoints[randomNumber].z);
    }
    [ServerRpc]
    public void RespawnServerRpc()
    {
        RespawnClientRpc(GetRandPos());
    }
    [ClientRpc]
    public void RespawnClientRpc(Vector3 spawnPos)
    {
        StartCoroutine(SpawnDelay(spawnPos));
        //mainPlayer.enabled = false;
        //transform.position = spawnPos;
        //mainPlayer.enabled = true;
    }
    public void Respawn()
    {
        randomNumber = Random.Range(0, spawnPoints.Length);
        RespawnServerRpc();
        RespawnClientRpc(GetRandPos());        
        //foreach (var player in FindObjectsOfType<PlayerStats>())
        //{
        //    if (player != this.GetComponent<PlayerStats>())
        //    {
        //        player.AddScoreServerRpc();
        //    }
        //}
    }
}
