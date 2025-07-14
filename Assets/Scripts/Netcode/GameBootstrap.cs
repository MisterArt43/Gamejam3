using Unity.NetCode;
using UnityEngine;

[UnityEngine.Scripting.Preserve] // preserve the class for the Netcode for GameObjects this mean that the class will not be stripped by Unity's build process
public class GameBootstrap : ClientServerBootstrap
{
    public override bool Initialize(string defaultWorldName)
    {
        AutoConnectPort = 7979;
        return base.Initialize(defaultWorldName);
    }
}
