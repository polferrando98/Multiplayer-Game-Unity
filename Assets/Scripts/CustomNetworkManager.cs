using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.UI;

public class MsgTypes
{
    public const short PlayerPrefabSelect = MsgType.Highest + 1;
    public class PlayerPrefabMsg : MessageBase
    {
        public short controlledId;
        public short prefabIndex;
    }
}

public class CustomNetworkManager : NetworkManager
{
    public short playerPrefabIndex;
    

    //Executed in the server
    public override void OnStartServer()
    {
        NetworkServer.RegisterHandler(MsgTypes.PlayerPrefabSelect, OnPrefabResponse);
        base.OnStartServer();
    }


    // Executed in the client
    public override void OnClientConnect(NetworkConnection conn)
    {
        client.RegisterHandler(MsgTypes.PlayerPrefabSelect, OnPrefabRequest);
        base.OnClientConnect(conn);
    }

    //  Executed in the server ---- This spawns the default player prefab
    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        MsgTypes.PlayerPrefabMsg msg = new MsgTypes.PlayerPrefabMsg();
        msg.controlledId = playerControllerId;
        NetworkServer.SendToClient(conn.connectionId, MsgTypes.PlayerPrefabSelect, msg);
    }

    private void OnPrefabRequest(NetworkMessage netMsg)
    {
        MsgTypes.PlayerPrefabMsg msg = netMsg.ReadMessage<MsgTypes.PlayerPrefabMsg>();
        msg.prefabIndex = playerPrefabIndex;
        client.Send(MsgTypes.PlayerPrefabSelect, msg);
    }
    
    private void OnPrefabResponse(NetworkMessage netMsg)
    {
        MsgTypes.PlayerPrefabMsg msg = netMsg.ReadMessage<MsgTypes.PlayerPrefabMsg>();
        playerPrefab = spawnPrefabs[msg.prefabIndex];
        base.OnServerAddPlayer(netMsg.conn, msg.controlledId);
    }






    public string[] playerNames = new string[] { "Boy", "Girl", "Robot" };

    private void OnGUI()
    {
        if (!isNetworkActive)
        {
            playerPrefabIndex = (short)GUI.SelectionGrid(
                new Rect(Screen.width - 200, 10, 200, 50), playerPrefabIndex,
                playerNames, 3);
        }
    }

    public void ChangePlayerPrefab(PlayerController currentPlayer, int prefabIndex)
    {
        GameObject newPlayer = Instantiate(spawnPrefabs[prefabIndex],
            currentPlayer.gameObject.transform.position,
            currentPlayer.gameObject.transform.rotation);

        NetworkServer.Destroy(currentPlayer.gameObject);

        NetworkServer.ReplacePlayerForConnection(currentPlayer.connectionToClient, newPlayer, 0);
    }

    public void AddObject(int objIndex, Vector3 pos)
    {
        GameObject newObject = Instantiate<GameObject>(
            spawnPrefabs[objIndex],
            pos,
            Quaternion.identity);

        NetworkServer.Spawn(newObject);
    }

    //[ClientRpc]

    //void RpcRunThisFunctionOnClient()
    //{
    //    // do something in all clients
    //}

   


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
