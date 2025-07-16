using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

class NetcodePlayerInputAuthoring : MonoBehaviour
{

}

class NetcodePlayerInputAuthoringBaker : Baker<NetcodePlayerInputAuthoring>
{
    public override void Bake(NetcodePlayerInputAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new NetcodePlayerInputData());
    }
}
