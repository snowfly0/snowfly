using UnityEngine;
using System.Collections;

public class ActorData : GenericData<ActorData>
{
    static string _DataPath = "/ActorData/ActorData.xml";
	static GameDataType _DataType = GameDataType.XMLResources;
    public string FishClassName;
	public int MaxCount = 20;
	public string PrefabName;

    public string BirthZoneName;

    public bool CanMove = true;
    public float ViewFishDistanceLimit = 2.0f;
    public float MoveSpeed=10f;
    public float RoateSpeed=16f;

    public int StartGroupId = 0;
    public int MinTankGroup=0;
    public int MaxTankGroup=10;

    public int MutiMatIndex = 0;
}
