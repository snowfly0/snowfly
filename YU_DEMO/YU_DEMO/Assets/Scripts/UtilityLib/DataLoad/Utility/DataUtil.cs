using System;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Text;

namespace GameData
{
    public class DataSetting
    {
        //数组类型分隔符
        public const char SPLITOT_ARRAY = ';';
        //键值对分隔符
        public const char SPLITOR_KEYVALUE = ':';
        //向量、四元数、颜色分隔符
        public const char SPLITOR_VECTOR = ',';
        //路径分隔符
        public const char SPLITOR_OBJPATH = '/';
        //自定义结构类型分隔符
        public const char SPLITOT_DEFINE = '-';
    }


    public static class DataUtil
    {
        /// <summary>
        /// 根据路径和物件类型获取物件
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="type">物件类型</param>
        /// <returns></returns>
        public static Transform GetComponent(string path)
        {
            Transform comp = null;
            string[] Nodes = path.Split(DataSetting.SPLITOR_OBJPATH);
            try
            {
                GameObject go = GameObject.Find(Nodes[0]);
                Transform t = go.transform;
                for (int i = 1; i < Nodes.Length; i++)
                {
                    t = t.FindChild(Nodes[i]);
                }
                comp = t;
            }
            catch
            {
                Debug.LogError(String.Format("获取{0}类型的容器时,出现异常,路径{1}", "Transform", path));
            }

            if (comp == null)
            {
                Debug.LogError(String.Format("获取{0}类型的容器得到空值,路径{1}", "Transform", path));
            }
            return comp;
        }

        /// <summary>
        /// 根据限定条件获取类型的字段属性集合
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="bFlags">绑定标识</param>
        /// <param name="conditions">限定开头名的集合</param>
        /// <returns></returns>
        public static List<FieldInfo> GetFieldInfoByRestrict(Type type, BindingFlags bFlags, params string[] conditions)
        {
            FieldInfo[] Infos = type.GetFields(bFlags);
            List<FieldInfo> ret = new List<FieldInfo>();
            foreach (var info in Infos)
            {
                foreach (string con in conditions)
                {
                    if (info.Name.StartsWith(con))
                    {
                        ret.Add(info);
                        break;
                    }
                }
            }
            return ret;
        }

        /// <summary>
        /// 将字符串映射成相应类型的基本数据
        /// </summary>
        /// <param name="value">要转换的字符串</param>
        /// <param name="type">目标类型</param>
        /// <returns>转换后的值</returns>
        public static object GetValue(String value, Type type)
        {
            if (String.IsNullOrEmpty(value))
            {
                return null;
            }
            if (type == typeof(string))
                return value;
            else if (type == typeof(Int32))
                return Convert.ToInt32(value);
            else if (type == typeof(float))
                return float.Parse(value);
            else if (type == typeof(bool))
            {
                return (value == "1") ? true : false;
            }
            else if (type == typeof(Int16))
                return Convert.ToInt16(Convert.ToDouble(value));
            else if (type == typeof(Vector3))
            {
                Vector3 result;
                ParseVector3(value, out result);
                return result;
            }
            else if (type == typeof(Color))
            {
                Color result;
                ParseColor(value, out result);
                return result;
            }
            else if (type == typeof(Quaternion))
            {
                Quaternion result;
                ParseQuaternion(value, out result);
                return result;
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                Type[] types = type.GetGenericArguments();
                var map = ParseMap(value);
                var result = type.GetConstructor(Type.EmptyTypes).Invoke(null);
                foreach (var item in map)
                {
                    var key = GetValue(item.Key, types[0]);
                    var v = GetValue(item.Value, types[1]);
                    type.GetMethod("Add").Invoke(result, new object[] { key, v });
                }
                return result;
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                Type t = type.GetGenericArguments()[0];
                var list = ParseList(value);
                var result = type.GetConstructor(Type.EmptyTypes).Invoke(null);
                foreach (var item in list)
                {
                    var v = GetValue(item, t);
                    type.GetMethod("Add").Invoke(result, new object[] { v });
                }
                return result;
            }
            else if (type == typeof(double))
                return double.Parse(value);
            else if (type.BaseType == typeof(Enum))
            {
                //object val= GetValue(value, Enum.GetUnderlyingType(type));
                return Enum.Parse(type, value);
            }
            else if (type == typeof(byte))
                return Convert.ToByte(Convert.ToDouble(value));
            else if (type == typeof(sbyte))
                return Convert.ToSByte(Convert.ToDouble(value));
            else if (type == typeof(UInt32))
                return Convert.ToUInt32(Convert.ToDouble(value));

            else if (type == typeof(Int64))
                return Convert.ToInt64(Convert.ToDouble(value));
            else if (type == typeof(UInt16))
                return Convert.ToUInt16(Convert.ToDouble(value));
            else if (type == typeof(UInt64))
                return Convert.ToUInt64(Convert.ToDouble(value));

            else
                return null;
        }



        private static bool ParseColor(string _inputString, out Color result)
        {
            string trimString = _inputString.Trim();
            result = Color.clear;
            try
            {
                string[] _detail = trimString.Split(DataSetting.SPLITOR_VECTOR);
                if (_detail.Length != 4)
                {
                    return false;
                }
                result = new Color(float.Parse(_detail[0]) / 255, float.Parse(_detail[1]) / 255, float.Parse(_detail[2]) / 255, float.Parse(_detail[3]) / 255);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError("Parse Color  error: " + trimString + e.ToString());
                return false;
            }
        }



        private static bool ParseVector2(string _inputString, out Vector2 result)
        {
            string trimString = _inputString.Trim();
            result = new Vector2();
            try
            {
                string[] _detail = trimString.Split(DataSetting.SPLITOR_VECTOR);
                if (_detail.Length != 2)
                {
                    return false;
                }
                result.x = float.Parse(_detail[0]);
                result.y = float.Parse(_detail[1]);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError("Parse Vector3  error: " + trimString + e.ToString());
                return false;
            }
        }



        /// <summary>
        /// 将指定格式(1.0, 2, 3.4) 转换为 Vector3 
        /// </summary>
        /// <param name="_inputString"></param>
        /// <param name="result"></param>
        /// <returns>返回 true/false 表示是否成功</returns>
        private static bool ParseVector3(string _inputString, out Vector3 result)
        {
            string trimString = _inputString.Trim();
            result = new Vector3();
            try
            {
                string[] _detail = trimString.Split(DataSetting.SPLITOR_VECTOR);//.Substring(1, trimString.Length - 2)
                if (_detail.Length != 3)
                {
                    return false;
                }
                result.x = float.Parse(_detail[0]);
                result.y = float.Parse(_detail[1]);
                result.z = float.Parse(_detail[2]);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError("Parse Vector3  error: " + trimString + e.ToString());
                return false;
            }
        }

        /// <summary>
        /// 将指定格式(0.0, 0.2,0.5,0 ) 转换为 Quaternion
        /// </summary>
        /// <param name="_inputString"></param>
        /// <param name="result"></param>
        /// <returns>返回 true/false 表示是否成功</returns>
        private static bool ParseQuaternion(string _inputString, out Quaternion result)
        {
            string trimString = _inputString.Trim();
            result = new Quaternion();

            try
            {
                string[] _detail = trimString.Split(DataSetting.SPLITOR_VECTOR);//.Substring(1, trimString.Length - 2)
                if (_detail.Length != 4)
                {
                    return false;
                }
                result.x = float.Parse(_detail[0]);
                result.y = float.Parse(_detail[1]);
                result.z = float.Parse(_detail[2]);
                result.w = float.Parse(_detail[3]);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError("Parse Quaternion error: " + trimString + e.ToString());
                return false;
            }
        }




        /// <summary>
        /// 将字典字符串转换为键类型与值类型都为字符串的字典对象。
        /// </summary>
        /// <param name="strMap">字典字符串</param>
        /// <param name="keyValueSpriter">键值分隔符</param>
        /// <param name="mapSpriter">字典项分隔符</param>
        /// <returns>字典对象</returns>
        private static Dictionary<String, String> ParseMap(this String strMap, Char keyValueSpriter = DataSetting.SPLITOR_KEYVALUE, Char mapSpriter = DataSetting.SPLITOT_ARRAY)
        {
            Dictionary<String, String> result = new Dictionary<String, String>();
            if (String.IsNullOrEmpty(strMap))
            {
                return result;
            }

            var map = strMap.Split(mapSpriter);//根据字典项分隔符分割字符串，获取键值对字符串
            for (int i = 0; i < map.Length; i++)
            {
                if (String.IsNullOrEmpty(map[i]))
                {
                    continue;
                }

                var keyValuePair = map[i].Split(keyValueSpriter);//根据键值分隔符分割键值对字符串
                if (keyValuePair.Length == 2)
                {
                    if (!result.ContainsKey(keyValuePair[0]))
                        result.Add(keyValuePair[0], keyValuePair[1]);
                    else
                        Debug.LogError(String.Format("Key {0} already exist, index {1} of {2}.", keyValuePair[0], i, strMap));
                }
                else
                {
                    Debug.LogError(String.Format("KeyValuePair are not match: {0}, index {1} of {2}.", map[i], i, strMap));
                }
            }
            return result;
        }



        /// <summary>
        /// 将列表字符串转换为字符串的列表对象。
        /// </summary>
        /// <param name="strList">列表字符串</param>
        /// <param name="listSpriter">数组分隔符</param>
        /// <returns>列表对象</returns>
        private static List<String> ParseList(this String strList, Char listSpriter = DataSetting.SPLITOT_ARRAY)
        {
            var result = new List<String>();
            if (String.IsNullOrEmpty(strList))
                return result;

            var trimString = strList.Trim();
            if (String.IsNullOrEmpty(strList))
            {
                return result;
            }
            var detials = trimString.Split(listSpriter);//.Substring(1, trimString.Length - 2)
            foreach (var item in detials)
            {
                if (!String.IsNullOrEmpty(item))
                    result.Add(item.Trim());
            }

            return result;
        }


        public static string GetString(object value, Type type)
        {
            if (value == null)
            {
                return string.Empty;
            }
            if (type == typeof(string))
                return value.ToString();
            else if (type == typeof(bool))
            {
                int res = (value.ToString().ToLower() == "true") ? 1 : 0;
                return res.ToString();
            }
            else if (CompareType(type, typeof(int), typeof(float)))
            {
                return value.ToString();
            }
            else if (type == typeof(Vector2) || type == typeof(Vector3) || type == typeof(Color) || type == typeof(Quaternion) || type == typeof(Vector4))
            {
                string[] strs = value.ToString().Split('(');
                string str = strs[1].Replace(")", "");
                return str;
            }
            //else if (CompareType(type,typeof(uint), typeof(Int16), typeof(UInt16), typeof(Int64), typeof(UInt64), typeof(double)))
            //{
            //    return value.ToString();
            //}
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                if (type == typeof(List<string>))
                {
                    return ListToString<string>(value);
                }
                else if (type == typeof(List<bool>))
                {
                    return ListToString<bool>(value);
                }
                else if (type == typeof(List<int>))
                {
                    return ListToString<int>(value);
                }
                else if (type == typeof(List<float>))
                {
                    return ListToString<float>(value);
                }
                else if (type == typeof(List<Vector2>))
                {
                    return ListToString<Vector2>(value);
                }
                else if (type == typeof(List<Vector3>))
                {
                    return ListToString<Vector3>(value);
                }
                else if (type == typeof(List<Quaternion>))
                {
                    return ListToString<Quaternion>(value);
                }
                else if (type == typeof(List<Color>))
                {
                    return ListToString<Color>(value);
                }
                else if (type == typeof(List<Vector4>))
                {
                    return ListToString<Vector4>(value);
                }
                else
                return null;
            }
            else if (type == typeof(byte))
            {
                return Convert.ToString(value);
            }
            else
                return value.ToString();
        }


        //public static string ListToString(Type listType, object value)
        //{
        //   // FieldInfo info=new FieldInfo()
        //    //listType.
        //}

        public static string ListToString<T>(object value)
        {
            StringBuilder sb = new StringBuilder();
            List<T> strings = (List<T>)value;
            int count = 0;
            foreach (var v in strings)
            {
                string append = GetString(v, typeof(T));
                if (++count < strings.Count)
                {
                    sb.Append(append + ";");
                }
                else sb.Append(append + "");
            }
            return sb.ToString();
        }

        static bool CompareType(Type type, params Type[] types)
        {
            foreach (Type t in types)
            {
                if (t == type) return true;
            }
            return false;
        }


    }




}
