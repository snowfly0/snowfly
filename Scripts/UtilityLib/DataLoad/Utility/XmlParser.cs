using System;
using System.Collections.Generic;
using System.Collections;
using System.Xml;
using System.Text;
using DataEditorMono.Xml;
using System.Security;
using System.Reflection;
using UnityEngine;
using System.IO;

namespace GameData
{

    public class XmlParser
    {
        #region 读取内部数据
        /// <summary>
        /// 读取格式为树深度为2且指定变量名为键值的xml表数据
        /// </summary>
        public static Dictionary<TKEY, TVAL> LoadDataDictionary<TKEY, TVAL>(string filepath, string keyPropertyName = "Id") where TVAL : new()
        {
            Dictionary<TKEY, TVAL> dataDic = new Dictionary<TKEY, TVAL>();
            Type valType = typeof(TVAL);

            FieldInfo[] valFields = valType.GetFields(BindingFlags.Instance | BindingFlags.Public);
            //FieldInfo keyField = valType.GetField(keyFieldName,BindingFlags.Instance|BindingFlags.NonPublic|BindingFlags.Public);
            //if (keyField == null)
            //{
            //    DataLoadLogger.AddLoadError("LoadDataDictionay,keyfield null:" + valType.Name);
            //    return dataDic;
            //}

            SecurityElement root = LoadXml(filepath);


            if (root == null || root.Children == null) return dataDic;

            foreach (SecurityElement node in root.Children)
            {

                string keyStr = node.Attribute(keyPropertyName);
                object key = DataUtil.GetValue(keyStr, typeof(TKEY));
                if (key == null)
                {
                    Debug.LogError("LoadDataDictionay,key null:" + valType.Name + ":" + keyPropertyName);
                    continue;
                }

                TVAL valueObj = new TVAL();

                PropertyInfo pInfo = valType.GetProperty(keyPropertyName, BindingFlags.Public | BindingFlags.Instance);
                if (pInfo.PropertyType == typeof(TKEY))
                {
                    pInfo.SetValue(valueObj, key, null);
                }

                foreach (var field in valFields)
                {
                    try
                    {
                        string val = node.Attribute(field.Name);
                        object objValue = DataUtil.GetValue(val, field.FieldType);
                        if (objValue != null)
                        {
                            field.SetValue(valueObj, objValue);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError("LoadDataDictionary Error:" + ex.Message);
                    }
                }
                dataDic.Add((TKEY)key, valueObj);
            }

            return dataDic;

        }

        /// <summary>
        /// 读取格式为树深度为2且指定变量名为键值的xml表数据
        /// </summary>
        public static Dictionary<int, TVAL> LoadIntDictionary<TVAL>(string filepath, string keyPropertyName = "Id") where TVAL : new()
        {
            Dictionary<int, TVAL> dataDic = new Dictionary<int, TVAL>();
            Type valType = typeof(TVAL);

            FieldInfo[] valFields = valType.GetFields(BindingFlags.Instance | BindingFlags.Public);
            SecurityElement root = LoadXml(filepath);

            if (root == null || root.Children == null) return dataDic;

            foreach (SecurityElement node in root.Children)
            {

                string keyStr = node.Attribute(keyPropertyName);
                object key = DataUtil.GetValue(keyStr, typeof(int));
                if (key == null)
                {
                    Debug.LogError("LoadDataDictionay,key null:" + valType.Name + ":" + keyPropertyName);
                    continue;
                }

                TVAL valueObj = new TVAL();

                PropertyInfo pInfo = valType.GetProperty(keyPropertyName, BindingFlags.Public | BindingFlags.Instance);
                if (pInfo.PropertyType == typeof(int))
                {
                    pInfo.SetValue(valueObj, key, null);
                }

                foreach (var field in valFields)
                {
                    try
                    {
                        string val = node.Attribute(field.Name);
                        object objValue = DataUtil.GetValue(val, field.FieldType);
                        if (objValue != null)
                        {
                            field.SetValue(valueObj, objValue);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError("LoadDataDictionary Error:" + ex.Message);
                    }
                }
                dataDic.Add((int)key, valueObj);
            }
            return dataDic;
        }

        public static Dictionary<string, TVAL> LoadStringDictionary<TVAL>(string filepath, string keyPropertyName = "Id") where TVAL : new()
        {
            Dictionary<string, TVAL> dataDic = new Dictionary<string, TVAL>();
            Type valType = typeof(TVAL);

            FieldInfo[] valFields = valType.GetFields(BindingFlags.Instance | BindingFlags.Public);
            SecurityElement root = LoadXml(filepath);

            if (root == null || root.Children == null) return dataDic;

            foreach (SecurityElement node in root.Children)
            {

                string keyStr = node.Attribute(keyPropertyName);
                object key = DataUtil.GetValue(keyStr, typeof(string));
                if (key == null)
                {
                    Debug.LogError("LoadDataDictionay,key null:" + valType.Name + ":" + keyPropertyName);
                    continue;
                }

                TVAL valueObj = new TVAL();

                PropertyInfo pInfo = valType.GetProperty(keyPropertyName, BindingFlags.Public | BindingFlags.Instance);
                if (pInfo.PropertyType == typeof(string))
                {
                    pInfo.SetValue(valueObj, key, null);
                }

                foreach (var field in valFields)
                {
                    try
                    {
                        string val = node.Attribute(field.Name);
                        object objValue = DataUtil.GetValue(val, field.FieldType);
                        if (objValue != null)
                        {
                            field.SetValue(valueObj, objValue);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError("LoadDataDictionary Error:" + ex.Message);
                    }
                }
                dataDic.Add((string)key, valueObj);
            }
            return dataDic;
        }

        /// <summary>
        /// 读取格式为树深度为3且Tag为键值的xml表数据
        /// </summary>
        /// <typeparam name="TVAL"></typeparam>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static Dictionary<string, List<TVAL>> LoadDataMap<TVAL>(string filepath) where TVAL : new()
        {
            Dictionary<string, List<TVAL>> dataDic = new Dictionary<string, List<TVAL>>();
            Type valType = typeof(TVAL);

            FieldInfo[] valFields = valType.GetFields(BindingFlags.Instance | BindingFlags.Public);

            SecurityElement root = LoadXml(filepath);

            if (root == null || root.Children == null) return dataDic;

            foreach (SecurityElement node in root.Children)
            {
                string ID = node.Tag;
                if (node.Children == null || node.Children.Count == 0)
                {
                    continue;
                }


                List<TVAL> valueList = new List<TVAL>();
                foreach (SecurityElement child in node.Children)
                {
                    TVAL valueObj = new TVAL();
                    foreach (var field in valFields)
                    {
                        try
                        {
                            string val = child.Attribute(field.Name);
                            object objValue = DataUtil.GetValue(val, field.FieldType);
                            if (objValue != null)
                            {
                                field.SetValue(valueObj, objValue);
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError("LoadDataDictionary Error:" + ex.Message);
                        }
                    }
                    valueList.Add(valueObj);
                }

                dataDic.Add(ID, valueList);
            }

            return dataDic;

        }

        /// <summary>
        /// 从XML文件中读取T类型的静态公有Field
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filepath"></param>
        public static void LoadXmlSetting<T>(string filepath)
        {
            SecurityElement root = LoadXml(filepath);
            if (root == null || root.Children == null) return;
            Type type = typeof(T);
            FieldInfo[] valFields = type.GetFields(BindingFlags.Static | BindingFlags.Public);
            foreach (var info in valFields)
            {
                SecurityElement pair = root.SearchForChildByTag(info.Name);
                if (pair != null)
                {
                    object value = DataUtil.GetValue(pair.Text, info.FieldType);
                    if (value != null)
                    {
                        info.SetValue(null, value);
                    }
                }
            }
        }
        #endregion


        #region 读取外部数据



        public static Dictionary<TKEY, TVAL> LoadDataDictionaryExternal<TKEY, TVAL>(string fileFullpath, string keyPropertyName = "Id") where TVAL : new()
        {
            Dictionary<TKEY, TVAL> dataDic = new Dictionary<TKEY, TVAL>();
            Type valType = typeof(TVAL);
            FieldInfo[] valFields = valType.GetFields(BindingFlags.Instance | BindingFlags.Public);
            XmlDocument xmldoc = new XmlDocument();
            try
            {
                LoadXml(xmldoc, fileFullpath);
               // xmldoc.Load(fileFullpath);
            }
            catch (Exception ex)
            {
                Debug.LogError(string.Format("加载{0}数据失败：", valType.Name) + ex.Message);
                return dataDic;
            }
            foreach (XmlElement element in xmldoc.DocumentElement.ChildNodes)
            {
                string keyStr = element.GetAttribute(keyPropertyName);
                object key = DataUtil.GetValue(keyStr, typeof(TKEY));
                if (key == null)
                {
                    Debug.LogError("LoadDataDictionay,key null:" + valType.Name + ":" + keyPropertyName);
                    continue;
                }

                TVAL valueObj = new TVAL();

                PropertyInfo pInfo = valType.GetProperty(keyPropertyName, BindingFlags.Public | BindingFlags.Instance);
                if (pInfo.PropertyType == typeof(TKEY))
                {
                    pInfo.SetValue(valueObj, key, null);
                }

                foreach (var field in valFields)
                {
                    try
                    {
                        string val = element.GetAttribute(field.Name);
                        object objValue = DataUtil.GetValue(val, field.FieldType);
                        if (objValue != null)
                        {
                            field.SetValue(valueObj, objValue);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError("LoadDataDictionary Error:" + ex.Message);
                    }
                }
                dataDic.Add((TKEY)key, valueObj);
            }

            return dataDic;
        }


        public static Dictionary<string,TVAL> LoadStringDictionaryExternal<TVAL>(string fileFullpath, string keyPropertyName = "Id") where TVAL : new()
        {
            Dictionary<string, TVAL> dataDic = new Dictionary<string, TVAL>();
            Type valType = typeof(TVAL);
            FieldInfo[] valFields = valType.GetFields(BindingFlags.Instance | BindingFlags.Public);
            XmlDocument xmldoc = new XmlDocument();
            try
            {
                LoadXml(xmldoc, fileFullpath);
                // xmldoc.Load(fileFullpath);
            }
            catch (Exception ex)
            {
                Debug.LogError(string.Format("加载{0}数据失败：", valType.Name) + ex.Message);
                return dataDic;
            }
            foreach (XmlElement element in xmldoc.DocumentElement.ChildNodes)
            {
                string keyStr = element.GetAttribute(keyPropertyName);
                object key = DataUtil.GetValue(keyStr, typeof(string));
                if (key == null)
                {
                    Debug.LogError("LoadDataDictionay,key null:" + valType.Name + ":" + keyPropertyName);
                    continue;
                }

                TVAL valueObj = new TVAL();

                PropertyInfo pInfo = valType.GetProperty(keyPropertyName, BindingFlags.Public | BindingFlags.Instance);
                if (pInfo.PropertyType == typeof(string))
                {
                    pInfo.SetValue(valueObj, key, null);
                }

                foreach (var field in valFields)
                {
                    try
                    {
                        string val = element.GetAttribute(field.Name);
                        object objValue = DataUtil.GetValue(val, field.FieldType);
                        if (objValue != null)
                        {
                            field.SetValue(valueObj, objValue);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError("LoadDataDictionary Error:" + ex.Message);
                    }
                }
                dataDic.Add((string)key, valueObj);
            }

            return dataDic;
        }


        public static Dictionary<int, TVAL> LoadIntDictionaryExternal<TVAL>(string fileFullpath, string keyPropertyName = "Id") where TVAL : new()
        {
            Dictionary<int, TVAL> dataDic = new Dictionary<int, TVAL>();
            Type valType = typeof(TVAL);
            FieldInfo[] valFields = valType.GetFields(BindingFlags.Instance | BindingFlags.Public);
            XmlDocument xmldoc = new XmlDocument();
            try
            {
                LoadXml(xmldoc, fileFullpath);
                // xmldoc.Load(fileFullpath);
            }
            catch (Exception ex)
            {
                Debug.LogError(string.Format("加载{0}数据失败：", valType.Name) + ex.Message);
                return dataDic;
            }
            foreach (XmlElement element in xmldoc.DocumentElement.ChildNodes)
            {
                string keyStr = element.GetAttribute(keyPropertyName);
                object key = DataUtil.GetValue(keyStr, typeof(int));
                if (key == null)
                {
                    Debug.LogError("LoadDataDictionay,key null:" + valType.Name + ":" + keyPropertyName);
                    continue;
                }

                TVAL valueObj = new TVAL();

                PropertyInfo pInfo = valType.GetProperty(keyPropertyName, BindingFlags.Public | BindingFlags.Instance);
                if (pInfo.PropertyType == typeof(int))
                {
                    pInfo.SetValue(valueObj, key, null);
                }

                foreach (var field in valFields)
                {
                    try
                    {
                        string val = element.GetAttribute(field.Name);
                        object objValue = DataUtil.GetValue(val, field.FieldType);
                        if (objValue != null)
                        {
                            field.SetValue(valueObj, objValue);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError("LoadDataDictionary Error:" + ex.Message);
                    }
                }
                dataDic.Add((int)key, valueObj);
            }

            return dataDic;
        }


        public static Dictionary<string, List<TVAL>> LoadDataMapExternal<TVAL>(string fileFullpath) where TVAL : new()
        {
            Dictionary<string, List<TVAL>> dataDic = new Dictionary<string, List<TVAL>>();
            Type valType = typeof(TVAL);
            FieldInfo[] valFields = valType.GetFields(BindingFlags.Instance | BindingFlags.Public);
            XmlDocument xmldoc = new XmlDocument();
            try
            {
                LoadXml(xmldoc, fileFullpath);
                //xmldoc.Load(fileFullpath);
            }
            catch (Exception ex)
            {
                Debug.LogError(string.Format("加载{0}数据失败：", valType.Name) + ex.Message);
                return dataDic;
            }

            foreach (XmlElement element in xmldoc.DocumentElement.ChildNodes)
            {
                string ID = element.Name;
                List<TVAL> valueList = new List<TVAL>();
                foreach (XmlElement child in element.ChildNodes)
                {
                    TVAL valueObj = new TVAL();
                    foreach (var field in valFields)
                    {
                        try
                        {
                            string val = child.GetAttribute(field.Name);
                            object objValue = DataUtil.GetValue(val, field.FieldType);
                            if (objValue != null)
                            {
                                field.SetValue(valueObj, objValue);
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError("LoadDataDictionary Error:" + ex.Message);
                        }
                    }
                    valueList.Add(valueObj);
                }

                dataDic.Add(ID, valueList);
            }

            return dataDic;
        }

        public static void LoadXmlSettingExternal<T>(string fileFullpath)
        {
            Type type = typeof(T);
            XmlDocument xmldoc = new XmlDocument();
            try
            {
                LoadXml(xmldoc, fileFullpath);
                //xmldoc.Load(fileFullpath);
            }
            catch (Exception ex)
            {
                Debug.LogError(string.Format("加载{0}数据失败：", type.Name) + ex.Message);
                return;
            }
            XmlElement rootElement = xmldoc.DocumentElement;
            FieldInfo[] valFields = type.GetFields(BindingFlags.Static | BindingFlags.Public);
            foreach (var info in valFields)
            {
                XmlNode node = rootElement.SelectSingleNode(info.Name);
                if (node == null) continue;
                XmlElement element = node as XmlElement;
                if (element == null) continue;
                object value = DataUtil.GetValue(element.InnerText, info.FieldType);
                if (value != null)
                {
                    info.SetValue(null, value);
                }
            }
        }
        #endregion


        #region 写入数据


        /// <summary>
        /// 将数据词典保存为XML文件
        /// </summary>
        /// <typeparam name="T">数据类型（T类型的对象里面包含的必须是基本类型）</typeparam>
        /// <param name="path">保存的绝对路径</param>
        /// <param name="dataMap">数据词典</param>
        /// <param name="exceptTypes">剔除的保存类型</param>
        public static void SaveXmlDictionaryList<T>(string path, Dictionary<string, List<T>> dataMap, params Type[] exceptTypes)
        {
            XmlDocument xmlDoc = new XmlDocument();
            CreateDirForFileIfNotExist(path);
            xmlDoc.AppendChild(xmlDoc.CreateXmlDeclaration("1.0", "utf-8", null));


            XmlElement root = xmlDoc.CreateElement("Root");
            xmlDoc.AppendChild(root);
            XmlComment commentInfo = xmlDoc.CreateComment(CreateXmlComment<T>());
            xmlDoc.InsertBefore(commentInfo, root);

            Type dataType = typeof(T);
            var fileInfos = ExceptFieldInfos(dataType, BindingFlags.Public | BindingFlags.Instance, exceptTypes);
            foreach (var pair in dataMap)
            {
                XmlNode rootChild = root.AppendChild(xmlDoc.CreateElement(pair.Key));
                foreach (var item in pair.Value)
                {
                    XmlElement itemNode = xmlDoc.CreateElement("Item");
                    foreach (var fInfo in fileInfos)
                    {
                        object value = fInfo.GetValue(item);
                        string valStr = DataUtil.GetString(value, fInfo.FieldType);
                        itemNode.SetAttribute(fInfo.Name, valStr);
                    }
                    rootChild.AppendChild(itemNode);
                }
            }
            xmlDoc.Save(path);
            UnityEngine.Debug.Log("Create xml file at:" + path);
        }


        /// <summary>
        /// 将数据词典保存为XML文件
        /// </summary>
        /// <typeparam name="T">数据类型（T类型的对象里面包含的必须是基本类型）</typeparam>
        /// <param name="path">保存的绝对路径(包含后缀)</param>
        /// <param name="dataMap">数据词典</param>
        /// <param name="exceptTypes">剔除的保存类型</param>
        public static void SaveXmlDictionary<TKEY, TVAL>(string path, Dictionary<TKEY, TVAL> dataMap,string keyPropertyName = "Id", params Type[] exceptTypes)
        {
            CreateDirForFileIfNotExist(path);
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.AppendChild(xmlDoc.CreateXmlDeclaration("1.0", "utf-8", null));

            XmlElement root = xmlDoc.CreateElement("Root");
            xmlDoc.AppendChild(root);

            XmlComment commentInfo = xmlDoc.CreateComment(CreateXmlComment<TVAL>());
            xmlDoc.InsertBefore(commentInfo, root);

            Type dataType = typeof(TVAL);
            var fileInfos = ExceptFieldInfos(dataType, BindingFlags.Public | BindingFlags.Instance, exceptTypes);
            foreach (var pair in dataMap)
            {
                XmlElement item = xmlDoc.CreateElement("Item");
                string id= DataUtil.GetString(pair.Key, typeof(TKEY));
                item.SetAttribute(keyPropertyName, id);
                foreach (var fInfo in fileInfos)
                {
                    object value = fInfo.GetValue(pair.Value);
                    string valStr = DataUtil.GetString(value, fInfo.FieldType);
                    item.SetAttribute(fInfo.Name, valStr);
                }
                root.AppendChild(item);
            }
            xmlDoc.Save(path);
            UnityEngine.Debug.Log("Create xml file at:" + path);
        }



        public static void SaveXmlIntDictionary<TVAL>(string path, Dictionary<int, TVAL> dataMap, string keyPropertyName = "Id", params Type[] exceptTypes)
        {
            CreateDirForFileIfNotExist(path);
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.AppendChild(xmlDoc.CreateXmlDeclaration("1.0", "utf-8", null));

            XmlElement root = xmlDoc.CreateElement("Root");
            xmlDoc.AppendChild(root);

            XmlComment commentInfo = xmlDoc.CreateComment(CreateXmlComment<TVAL>());
            xmlDoc.InsertBefore(commentInfo, root);

            Type dataType = typeof(TVAL);
            var fileInfos = ExceptFieldInfos(dataType, BindingFlags.Public | BindingFlags.Instance, exceptTypes);
            foreach (var pair in dataMap)
            {
                XmlElement item = xmlDoc.CreateElement("Item");
                string id = DataUtil.GetString(pair.Key, typeof(int));
                item.SetAttribute(keyPropertyName, id);
                foreach (var fInfo in fileInfos)
                {
                    object value = fInfo.GetValue(pair.Value);
                    string valStr = DataUtil.GetString(value, fInfo.FieldType);
                    item.SetAttribute(fInfo.Name, valStr);
                }
                root.AppendChild(item);
            }
            xmlDoc.Save(path);
            UnityEngine.Debug.Log("Create xml file at:" + path);
        }


        public static void SaveXmlStringDictionary<TVAL>(string path, Dictionary<string, TVAL> dataMap, string keyPropertyName = "Id", params Type[] exceptTypes)
        {
            CreateDirForFileIfNotExist(path);
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.AppendChild(xmlDoc.CreateXmlDeclaration("1.0", "utf-8", null));

            XmlElement root = xmlDoc.CreateElement("Root");
            xmlDoc.AppendChild(root);

            XmlComment commentInfo = xmlDoc.CreateComment(CreateXmlComment<TVAL>());
            xmlDoc.InsertBefore(commentInfo, root);
            Type dataType = typeof(TVAL);
            var fileInfos = ExceptFieldInfos(dataType, BindingFlags.Public | BindingFlags.Instance, exceptTypes);
            foreach (var pair in dataMap)
            {
                XmlElement item = xmlDoc.CreateElement("Item");
                string id = DataUtil.GetString(pair.Key, typeof(string));
                item.SetAttribute(keyPropertyName, id);
                foreach (var fInfo in fileInfos)
                {
                    object value = fInfo.GetValue(pair.Value);
                    string valStr = DataUtil.GetString(value, fInfo.FieldType);
                    item.SetAttribute(fInfo.Name, valStr);
                }
                root.AppendChild(item);
            }
            xmlDoc.Save(path);
            UnityEngine.Debug.Log("Create xml file at:" + path);
        }

        /// <summary>
        /// 保存T类型里的静态Field为Xml配置文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fullpath">完全路径</param>
        /// <param name="exceptTypes">剔除的保存类型</param>
        public static void SaveXmlSetting<T>(string fullpath, params Type[] exceptTypes)
        {
            XmlDocument xmlDoc = new XmlDocument();
            CreateDirForFileIfNotExist(fullpath);
            xmlDoc.AppendChild(xmlDoc.CreateXmlDeclaration("1.0", "utf-8", null));

            XmlElement root = xmlDoc.CreateElement("Root");
            xmlDoc.AppendChild(root);

            XmlComment commentInfo = xmlDoc.CreateComment(CreateXmlComment<T>());
            xmlDoc.InsertBefore(commentInfo, root);
            Type dataType = typeof(T);
            var fileInfos = ExceptFieldInfos(dataType, BindingFlags.Public | BindingFlags.Static, exceptTypes);
            foreach (var info in fileInfos)
            {
                XmlElement item = xmlDoc.CreateElement(info.Name);
                object value = info.GetValue(null);
                string valStr = DataUtil.GetString(value,info.FieldType);
                if (value != null)
                {
                    item.InnerText = valStr;
                }
                root.AppendChild(item);
            }
            xmlDoc.Save(fullpath);
            UnityEngine.Debug.Log("Create xml file at:" + fullpath);
        }
        #endregion


        #region Util
        public static string CreateXmlComment<T>()
        {
            return CreateXmlComment(typeof(T));
        }

        public static string CreateXmlComment(Type type)
        {
            StringBuilder sb = new StringBuilder();
            GDataDescriptionAttribute attribute = Attribute.GetCustomAttribute(type, typeof(GDataDescriptionAttribute)) as GDataDescriptionAttribute;
            if (attribute != null)
            {
                sb.AppendLine(attribute.BaseDescpt + ":" + attribute.DetailDescpt);
            }
            var fileInfos = type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

            foreach (FieldInfo info in fileInfos)
            {
                attribute = Attribute.GetCustomAttribute(info, typeof(GDataDescriptionAttribute)) as GDataDescriptionAttribute;
                if (attribute != null)
                {
                    sb.AppendLine(info.Name + ":" + attribute.BaseDescpt + "***" + attribute.DetailDescpt);
                }
            }
            return sb.ToString();
        }

        static FieldInfo[] ExceptFieldInfos(Type type, BindingFlags bindingFlags, params Type[] exceptTypes)
        {
            var fileInfos = type.GetFields(bindingFlags);
            List<FieldInfo> allfileInfos = new List<FieldInfo>(fileInfos);
            for (int i = allfileInfos.Count - 1; i >= 0; --i)
            {
                for (int m = 0; m < exceptTypes.Length; ++m)
                {
                    if (allfileInfos[i].FieldType == exceptTypes[m])
                    {
                        allfileInfos.RemoveAt(i);
                        continue;
                    }
                }
            }
            return allfileInfos.ToArray();
        }



        static void CreateDirForFileIfNotExist(string fullpathName)
        {
            string dirpath = Path.GetDirectoryName(fullpathName);
            if (!Directory.Exists(dirpath))
            {
                Directory.CreateDirectory(dirpath);
            }
        }

        static void LoadXml(XmlDocument doc,string path)
        {
            if(Application.platform!=RuntimePlatform.Android)
            {
                doc.Load(path);
                return;
            }
            foreach (WWW www in LoadWWW(doc, path))
            {
                if(www!=null)
                {
                    if(www.error!=null)
                    {
                        throw new Exception(www.error+"-->Failed to load at path:"+path);
                    }
                    else
                    {
                        StringReader reader = new StringReader(www.text);
                        reader.Read();
                        string text = reader.ReadToEnd();
                        //string text= Encoding.UTF8.GetString(www.bytes, 2, www.bytes.Length - 2);
                        doc.LoadXml(text);
                        break;
                    }
                }
            }
        }

        static IEnumerable<WWW> LoadWWW(XmlDocument doc, string path)
        {
            WWW www = new WWW(path);
            while(!www.isDone)
            {
                yield return null;
            }
            yield return www;
        }

        public static SecurityElement LoadXml(string path)
        {
            SecurityElement root = null;
            TextAsset assets = Resources.Load<TextAsset>(path);
            if (assets != null)
            {
                SecurityParser docEvents = new SecurityParser();
                docEvents.LoadXml(assets.ToString().Trim());
                root = docEvents.ToXml();
            }
            return root;
        }
        #endregion
    }




}
