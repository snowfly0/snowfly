using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class ScanCacheData : GenericDataEx<ScanCacheData>
{
    static  GameDataType _DataType=GameDataType.XMLStreaming;
    static  string _DataPath = "/CacheData/ScanCacheData.xml";

    public string MD5Hash;
}

