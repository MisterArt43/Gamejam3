using TMPro;
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NetcodeForEntitiesUI : MonoBehaviour
{
    [SerializeField] private Button startServerButton;
    [SerializeField] private Button joinGameButton;
    [SerializeField] private TMP_InputField ipInputField;

    private void Awake()
    {
        startServerButton.onClick.AddListener(StartServer);
        joinGameButton.onClick.AddListener(JoinGame);
    }

    private void StartServer()
    {
        World serverWorld = ClientServerBootstrap.CreateServerWorld("ServerWorld");
        World clientWorld = ClientServerBootstrap.CreateClientWorld("ClientWorld");

        foreach (World world in World.All)
        {
            if (world.Flags == WorldFlags.Game)
            {
                world.Dispose();
                break;
            }
        }

        World.DefaultGameObjectInjectionWorld ??= serverWorld;


        // after creating the server world i can load the scene (don't forget to add subscene in the build Profiles)
        SceneManager.LoadSceneAsync("GameScene", LoadSceneMode.Single);

        ushort port = 7979;

        RefRW<NetworkStreamDriver> networkStreamDriver =
            serverWorld.EntityManager.CreateEntityQuery(typeof(NetworkStreamDriver)).GetSingletonRW<NetworkStreamDriver>();
        networkStreamDriver.ValueRW.Listen(NetworkEndpoint.AnyIpv4.WithPort(port));


        NetworkEndpoint connectNetworkEndpoint = NetworkEndpoint.LoopbackIpv4.WithPort(port);
        networkStreamDriver =
            clientWorld.EntityManager.CreateEntityQuery(typeof(NetworkStreamDriver)).GetSingletonRW<NetworkStreamDriver>();
        networkStreamDriver.ValueRW.Connect(clientWorld.EntityManager, connectNetworkEndpoint);
    }
    
    private void JoinGame()
    {
        World clientWorld = ClientServerBootstrap.CreateClientWorld("ClientWorld");

        foreach (World world in World.All)
        {
            if (world.Flags == WorldFlags.Game)
            {
                world.Dispose();
                break;
            }
        }

        if (World.DefaultGameObjectInjectionWorld == null)
        {
            World.DefaultGameObjectInjectionWorld = clientWorld;
        }


        // after creating the server world i can load the scene (don't forget to add subscene in the build Profiles)
        SceneManager.LoadSceneAsync("GameScene", LoadSceneMode.Single);

        ushort port = 7979;

        string ip = ipInputField.text;
        //"192.168.1.67";

        //check if ip format is valid
        if (!NetworkEndpoint.TryParse(ip, port, out NetworkEndpoint endpoint))
        {
            Debug.LogError("Invalid IP address format.");
            return;
        }


        NetworkEndpoint connectNetworkEndpoint = NetworkEndpoint.Parse(ip, port);

        RefRW<NetworkStreamDriver> networkStreamDriver =
            clientWorld.EntityManager.CreateEntityQuery(typeof(NetworkStreamDriver)).GetSingletonRW<NetworkStreamDriver>();
        networkStreamDriver.ValueRW.Connect(clientWorld.EntityManager, connectNetworkEndpoint);
    }
}
