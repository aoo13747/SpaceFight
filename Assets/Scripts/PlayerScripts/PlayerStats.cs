using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MLAPI;
using MLAPI.NetworkVariable;
using MLAPI.Messaging;

public class PlayerStats : NetworkBehaviour
{
    public Text playerNameTextPrefab;
    private Text nameLabel;
    string textBoxName = "";    
    public Text scorePrefab;
    private Text scoreLabel;
    public Transform player1ScorePos;

    public NetworkVariableVector3 Position = new NetworkVariableVector3(new NetworkVariableSettings
    {
        WritePermission = NetworkVariablePermission.ServerOnly,
        ReadPermission = NetworkVariablePermission.Everyone
    });
    public NetworkVariable<string> playerName = new NetworkVariable<string>(new NetworkVariableSettings
    {
        WritePermission = NetworkVariablePermission.OwnerOnly
    }, "Player");
    public NetworkVariable<int> score_Network = new NetworkVariable<int>(new NetworkVariableSettings
    {
        WritePermission = NetworkVariablePermission.ServerOnly,
        ReadPermission = NetworkVariablePermission.Everyone
    }, 0);
    [SerializeField]
    private NetworkVariableInt healthValue = new NetworkVariableInt(new NetworkVariableSettings
    {
        WritePermission = NetworkVariablePermission.ServerOnly,
        ReadPermission = NetworkVariablePermission.Everyone
    }, 100);
    void OnPlayerNameChanged(string oldValue, string newValue)
    {
        if (!IsClient)
        {
            return;
        }
        gameObject.name = newValue;
        Debug.LogFormat("Old name : {0} >> New Name : {1}", oldValue, newValue);
    }
    void OnScoreChange(int _oldScore, int _newScore)
    {
        Debug.LogFormat("Old score : {0} >> New score : {1}", _oldScore, _newScore);
    }
    public override void NetworkStart()
    {
        //Move();
        //SetPlayerName();
        GameObject canvas = GameObject.FindWithTag("MainCanvas");
        nameLabel = Instantiate(playerNameTextPrefab, Vector3.zero, Quaternion.identity) as Text;
        nameLabel.transform.SetParent(canvas.transform);
        scoreLabel = Instantiate(scorePrefab, Vector3.zero, Quaternion.identity) as Text;
        scoreLabel.transform.SetParent(canvas.transform);
        player1ScorePos = GameObject.FindWithTag("ScorePos").transform;
        scoreLabel.transform.position = player1ScorePos.transform.position;
        player1ScorePos.transform.position += new Vector3(800, 0, 0);
    }   
    private void OnDestroy()
    {
        if (nameLabel != null)
        {
            Destroy(nameLabel.gameObject);
        }
    }
    public void SetPlayerName()
    {
        if (!IsOwner)
            return;
        if (NetworkManager.Singleton.IsServer)
        {
            playerName.Value = "Player1";
            score_Network.Value = 0;
        }
        else
        {
            playerName.Value = "Player2";
            score_Network.Value = 0;
        }
    }    
    [ServerRpc]
    public void AddScoreServerRpc()
    {
        AddScore();
    }
    public void AddScore()
    {
        score_Network.Value += 1;
    }
    [ServerRpc]
    public void SetPlayerNameServerRpc(string name)
    {
        playerName.Value = name;
    }
    private void Update()
    {
        //transform.position = Position.Value;
        Vector3 nameLabelPos = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, 2f, 0));
        nameLabel.text = gameObject.name;
        nameLabel.transform.position = nameLabelPos;
        scoreLabel.text = gameObject.name + " : " + score_Network.Value.ToString();       
    }   

    [ServerRpc]
    public void TakeDamageServerRpc(int _damage)
    {
        healthValue.Value -= _damage;
        if (healthValue.Value <= 0)
        {
            gameObject.GetComponent<PlayerRespawn>().Respawn();
            healthValue.Value = 100;
            foreach (var player in FindObjectsOfType<PlayerStats>())
            {
                if (player != this.gameObject.GetComponent<PlayerStats>())
                {
                    player.AddScore();
                    //if(IsServer)
                    //player.AddScoreServerRpc();
                    
                }                
            }
        }
    }
    
    void OnHealthChange(int oldValue, int newValue)
    {
        
        Debug.LogFormat("{0} : OldValue : {1} :NewValue : {2}", gameObject.name, oldValue, newValue);
        
    }
    private void OnEnable()
    {
        healthValue.OnValueChanged += OnHealthChange;
        playerName.OnValueChanged += OnPlayerNameChanged;
        score_Network.OnValueChanged += OnScoreChange;
        if (nameLabel != null)
        {
            nameLabel.enabled = true;
        }
    }
    private void OnDisable()
    {
        healthValue.OnValueChanged -= OnHealthChange;
        playerName.OnValueChanged -= OnPlayerNameChanged;
        score_Network.OnValueChanged -= OnScoreChange;
        if (nameLabel != null)
        {
            nameLabel.enabled = false;
        }
    }    
    #region Unuse
    /*
    public void Move()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            var randPos = GetRandPos();
            transform.position = randPos;
            Position.Value = randPos;
        }
        else
        {
            ChangePosServerRpc();
        }
    }

    [ServerRpc]
    void ChangePosServerRpc(ServerRpcParams rpcParams = default)
    {
        Position.Value = GetRandPos();
    }
    static Vector3 GetRandPos()
    {
        return new Vector3(Random.Range(-3f, 3f), 1f, Random.Range(-3, 3f));
    }

    */
    #endregion
}


