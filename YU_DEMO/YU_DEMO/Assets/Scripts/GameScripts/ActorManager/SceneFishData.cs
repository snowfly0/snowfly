using UnityEngine;
using System.Collections;

//场景内置鱼的数据
public class SceneFishData : GenericData<SceneFishData>
{
    static string _DataPath = "/FishData/SceneFishData.xml";
    static GameDataType _DataType = GameDataType.XMLStreaming;

    public int TypeId;

    public float MoveSpeed;

    public float RotateSpeed;

    public int StartGroupId = 0;
    public int MinTankGroup = 0;
    public int MaxTankGroup = 10;

    public string BirthZoneName;

    public bool Enable;
}
