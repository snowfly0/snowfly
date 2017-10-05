using UnityEngine;
using System.Collections;
//数据模块支持加载的数据类型包括基本类型和它们组成的链表
namespace GameData.ExampleData
{
    //SettingData加载的是配置数据，需要加载的数据需为public static的field，property则会无视
    [NoDataWindow]//使用此特性后，Tools/数据编辑器项目中不会出现此数据表
    public class ExampleSettingData : SettingData<ExampleSettingData>
    {
        //必须包含静态私有成员，名为“_DataPath”，表示相对加载路径
        static string _DataPath = "/TestSetting/ExampleSettingData";
        //必须包含静态私有成员，名为“_DataType”，表示加载方式
        static GameDataType _DataType = GameDataType.XMLStreaming;

        public static string SettingName = "123456";//可以设置初始值，生成配置文件后，会被读取的值覆盖
        public static int Setting123 = 123;
        public static int Setting111 { get; set; }//property不会被保存和读取
    }


    //GenericData加载的是key为int的字典数据，需要加载的数据需为public static的field，property则会无视
    [GDataModule(ModuleName = "Example数据")]//使用此特性后，Tools/数据编辑器中会给此表分栏
    public class ExampleGenericDataData : GenericData<ExampleGenericDataData>
    {
        //必须包含静态私有成员，名为“_DataPath”，表示相对加载路径
        static string _DataPath = "/TestSetting/ExampleGenericDataData";
        //必须包含静态私有成员，名为“_DataType”，表示加载方式
        static GameDataType _DataType = GameDataType.XMLResources;

        public  string SettingName = "123456";//可以设置初始值，生成配置文件后，会被读取的值覆盖
        public  int Setting123 = 123;
        public  int Setting111 { get; set; }//property不会被保存和读取
    }

    //GenericDataEx加载的是key为string的字典数据，需要加载的数据需为public static的field，property则会无视
    [GDataModule(ModuleName = "Example数据")]//使用此特性后，Tools/数据编辑器中会给此表分栏
    public class ExampleGenericDataExData : GenericDataEx<ExampleGenericDataExData>
    {
        //必须包含静态私有成员，名为“_DataPath”，表示相对加载路径
        static string _DataPath = "/TestSetting/ExampleGenericDataExData";
        //必须包含静态私有成员，名为“_DataType”，表示加载方式
        static GameDataType _DataType = GameDataType.XMLResources;

        public string SettingName = "123456";//可以设置初始值，生成配置文件后，会被读取的值覆盖
        public int Setting123 = 123;
        public int Setting111 { get; set; }//property不会被保存和读取
    }
    //GenericDataList加载的是key为string，Value为List<T>的字典数据，需要加载的数据需为public static的field，property则会无视
    [GDataModule(ModuleName = "Example数据")]//使用此特性后，Tools/数据编辑器中会给此表分栏
    public class ExampleGenericDataListData : GenericDataList<ExampleGenericDataListData>
    {
        //必须包含静态私有成员，名为“_DataPath”，表示相对加载路径
        static string _DataPath = "/TestSetting/ExampleGenericDataExData";
        //必须包含静态私有成员，名为“_DataType”，表示加载方式
        static GameDataType _DataType = GameDataType.XMLResources;

        public string SettingName = "123456";//可以设置初始值，生成配置文件后，会被读取的值覆盖
        public int Setting123 = 123;
        public int Setting111 { get; set; }//property不会被保存和读取
    }
}