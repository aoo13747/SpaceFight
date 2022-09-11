using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using MLAPI;
using MLAPI.Transports.UNET;

public class LoginManager : MonoBehaviour
{
    public GameObject loginPanal;   
    public Button hostButton, clientButton;
    public Text nameInputField_Text;
    private UNetTransport uNetTransport;
    public string ipAddress = "127.0.0.1";

    public Transform[] spawnPos;
    
    public void OnIpAddressChanged(string address)
    {
        this.ipAddress = address;
    }

    public void Host()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        //Vector3 spawnPos = new Vector3(-2f, 1f, 0f);
        //Quaternion spawnRotate = Quaternion.Euler(0f, 135f, 0f);
        NetworkManager.Singleton.StartHost(spawnPos[Random.Range(0,spawnPos.Length)].position,Quaternion.identity);
    }

    private void ApprovalCheck(byte[] connectionData, ulong clientId, MLAPI.NetworkManager.ConnectionApprovedDelegate callback)
    {
        //Your logic here
        string playerName = Encoding.ASCII.GetString(connectionData);
        bool approve = playerName != nameInputField_Text.text;
        bool createPlayerObject = true;

        Vector3 _spawnPos = Vector3.zero;
        Quaternion spawnRotate = Quaternion.identity;

        switch (NetworkManager.Singleton.ConnectedClients.Count)
        {
            case 1:
                _spawnPos = spawnPos[Random.Range(0, spawnPos.Length)].position;                
                break;
        }

        // The prefab hash. Use null to use the default player prefab
        // If using this hash, replace "MyPrefabHashGenerator" with the name of a prefab added to the NetworkPrefabs field of your NetworkManager object in the scene
        //ulong? prefabHash = NetworkSpawnManager.GetPrefabHashFromGenerator("MyPrefabHashGenerator");

        //If approve is true, the connection gets added. If it's false. The client gets disconnected
        callback(createPlayerObject, null, approve, null, null);

    }
    public void Client()
    {
        //uNetTransport = NetworkManager.Singleton.GetComponent<UNetTransport>();
        //uNetTransport.ConnectAddress = ipAddress;
        NetworkManager.Singleton.NetworkConfig.ConnectionData = Encoding.ASCII.GetBytes(nameInputField_Text.text);
        NetworkManager.Singleton.StartClient();
    }
    private void Start()
    {
        loginPanal.SetActive(true);
        hostButton.onClick.AddListener(() => Host());
        clientButton.onClick.AddListener(() => Client());

        NetworkManager.Singleton.OnServerStarted += HandleServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += HandleClinetDisconnected;
    }
    private void OnDestroy()
    {
        if (NetworkManager.Singleton == null)
        {
            return;
        }
        NetworkManager.Singleton.OnServerStarted -= HandleServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClinetDisconnected;
    }
    void HandleClientConnected(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            loginPanal.SetActive(false);            
            SetPlayerName(clientId);
        }

    }
    void SetPlayerName(ulong clientId)
    {
        if (!NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out var networkClient))
        {
            return;
        }
        if (!networkClient.PlayerObject.TryGetComponent<PlayerStats>(out var mainPlayer))
        {
            return;
        }
        string playerName = nameInputField_Text.text;
        mainPlayer.SetPlayerNameServerRpc(playerName);
    }
    void HandleClinetDisconnected(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            loginPanal.SetActive(true);
            
        }
    }
    void HandleServerStarted()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            HandleClientConnected(NetworkManager.Singleton.LocalClientId);
        }
    }
    public void Leave()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.StopHost();
            NetworkManager.Singleton.ConnectionApprovalCallback -= ApprovalCheck;
        }
        else if (NetworkManager.Singleton.IsClient)
        {
            NetworkManager.Singleton.StopClient();
        }
        loginPanal.SetActive(true);
        
    }
}
