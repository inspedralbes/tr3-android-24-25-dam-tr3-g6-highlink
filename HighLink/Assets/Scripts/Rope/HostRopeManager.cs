using Unity.Netcode;
using UnityEngine;
using System.Linq;

public class HostRopeManager : NetworkBehaviour
{
    [SerializeField] private GameObject ropePrefab; // Non-networked prefab

    public override void OnNetworkSpawn()
    {
        if (IsHost)
        {
            NetworkManager.OnClientConnectedCallback += OnClientConnected;
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        if (NetworkManager.ConnectedClients.Count == 2)
        {
            NetworkObject[] players = new NetworkObject[2];

            players[0] = NetworkManager.Singleton.LocalClient.PlayerObject;

            

            // Find first connected client player (index 1)
            foreach (var client in NetworkManager.ConnectedClients)
            {
                if (client.Key != NetworkManager.LocalClientId)
                {
                    players[1] = client.Value.PlayerObject;
                    break;
                }
            }

            Debug.Log(players[0] != null);

            Debug.Log(players[1] != null);

            // Debug.Log("Both players: " + players[0] != null && players[1] != null? "both not null" : "both null");

            if (players[0] != null)
            {
                Debug.Log("about to spawn rope");
                if(players[1] != null)
                {
                    Debug.Log("Spawning rope frfr");
                    SpawnRopeClientRpc(
                        new NetworkObjectReference(players[0]),
                        new NetworkObjectReference(players[1])
                    );
                }
            }
        }
    }

    [ClientRpc]
    private void SpawnRopeClientRpc(NetworkObjectReference player1Ref, NetworkObjectReference player2Ref)
    {
        if (player1Ref.TryGet(out NetworkObject player1) && 
            player2Ref.TryGet(out NetworkObject player2))
        {
            InstantiateRope(player1.gameObject, player2.gameObject);
        }
    }

    private void InstantiateRope(GameObject player1, GameObject player2)
    {
        GameObject rope = Instantiate(ropePrefab);

        // Get the script from the instantiated object
        var ropeCreator = rope.GetComponent<RopeCreator>();

        // Set the player references
        ropeCreator.player1 = player1.transform;
        ropeCreator.player2 = player2.transform;

        // Get the other script from the instantiated object
        var ropeController = rope.GetComponent<RopeConstraint2D>();

        // Set the player references
        ropeController.player1 = player1.transform;
        ropeController.player2 = player2.transform;
    }

    public override void OnDestroy()
    {
        if (IsHost)
        {
            NetworkManager.OnClientConnectedCallback -= OnClientConnected;
        }
    }
}