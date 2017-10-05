using System;
using System.Collections.Generic;
using System.Reflection;
using System.Diagnostics;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Collections;

namespace GameData
{
    public class INIParser
    {

        /// <summary>
        /// 读取INI文件词典
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fullpath"></param>
        /// <returns></returns>
        public static Dictionary<string, T> LoadDataDictionary<T>(string fullpath) where T : new()
        {
            Dictionary<string, T> dataMap = new Dictionary<string, T>();
            if (!File.Exists(fullpath))
            {
                return dataMap;
            }
            Type valType = typeof(T);
            FieldInfo[] valFields = valType.GetFields(BindingFlags.Instance | BindingFlags.Public);
            INI appData = new INI(fullpath);
            string section = "GlobalSetting";
            int typeCount = int.Parse(appData.ReadValue(section, "TypeCount"));
            section = "Types";
            for (int i = 0; i < typeCount; ++i)
            {
                T data = new T();
                string key = appData.ReadValue(section, "Type" + i);
                foreach (var field in valFields)
                {
                    object value = DataUtil.GetValue(appData.ReadValue(key, field.Name), field.FieldType);
                    if (value != null)
                    {
                        field.SetValue(data, value);
                    }
                }
                dataMap.Add(key, data);
            }
            return dataMap;
        }

        /// <summary>
        /// 保存数据词典为INI文件
        /// </summary>
        /// <typeparam name="T">类类型</typeparam>
        /// <param name="dataMap"></param>
        /// <param name="fullpath">完全保存路径</param>
        public static void SaveDictionary<T>(Dictionary<string, T> dataMap, string fullpath) where T : new()
        {
            Type valType = typeof(T);
            if (!File.Exists(fullpath))
            {
                string[] dirSpits = fullpath.Split('/');
                string dir = fullpath.Replace(dirSpits[dirSpits.Length - 1], "");
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                var stream = File.Create(fullpath);
                stream.Close();
            }
            else
            {
                File.Delete(fullpath);
                var stream = File.Create(fullpath);
                stream.Close();
            }
            FieldInfo[] valFields = valType.GetFields(BindingFlags.Instance | BindingFlags.Public);
            INI appData = new INI(fullpath);
            string section = "GlobalSetting";
            appData.WriteValue(section, "TypeCount", dataMap.Count.ToString());
            section = "Types";
            int nCount = 0;
            foreach (var pair in dataMap)
            {
                appData.WriteValue(section, "Type" + nCount, pair.Key);
                foreach (var filed in valFields)
                {
                    object value = filed.GetValue(pair.Value);
                    string valStr = DataUtil.GetString(value, filed.FieldType);
                    appData.WriteValue(pair.Key, filed.Name, valStr);
                }
                nCount++;
            }
#if UNITY_EDITOR
            UnityEngine.Debug.Log("Save ini config file at:" + fullpath);
#endif
        }

        public static void LoadSetting<T>(string fullpath)
        {
            Type type = typeof(T);
            FieldInfo[] valFields = type.GetFields(BindingFlags.Static | BindingFlags.Public);
            if (!File.Exists(fullpath))
            {
                return;
            }
            INI appData = new INI(fullpath);
            string section = "GlobalSetting";
            string valueStr = null;
            foreach(var info in valFields)
            {
                if (info.IsLiteral) continue;
                valueStr = appData.ReadValue(section, info.Name);
                object valobj = DataUtil.GetValue(valueStr,info.FieldType);
                if(valobj!=null)
                {
                    info.SetValue(null, valobj);
                }
            }

        }

        public static void SaveSetting<T>(string fullpath)
        {
            Type type = typeof(T);
            if (!File.Exists(fullpath))
            {
                string[] dirSpits = fullpath.Split('/');
                string dir = fullpath.Replace(dirSpits[dirSpits.Length - 1], "");
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                var stream = File.Create(fullpath);
                stream.Close();
            }
            else
            {
                File.Delete(fullpath);
                var stream = File.Create(fullpath);
                stream.Close();
            }
            INI appData = new INI(fullpath);
            string section = "GlobalSetting";
            string valueStr = null;
            FieldInfo[] valFields = type.GetFields(BindingFlags.Static | BindingFlags.Public);

            foreach (var info in valFields)
            {
                if (info.IsLiteral) continue;
                valueStr = DataUtil.GetString(info.GetValue(null), info.FieldType);
                if (!string.IsNullOrEmpty(valueStr))
                {
                    appData.WriteValue(section, info.Name, valueStr);
                }
            }
        }


        class INI
        {
            IniParser m_iniParser;

            public INI(string INIPath)
            {
                m_iniParser = new IniParser(INIPath);
            }

            //向Ini文件中写数据
            public void WriteValue(string Section, string Key, string Value)
            {
                m_iniParser.AddSetting(Section, Key, Value);
                m_iniParser.SaveSettings();
            }

            //从Ini文件中读数据
            public string ReadValue(string Section, string Key)
            {
                return m_iniParser.GetSetting(Section, Key);
            }


            public class IniParser
            {
                private class SectionPair
                {
                    public String Section;
                    public String Key;

                    public static bool operator ==(SectionPair lhs, SectionPair rhs)
                    {
                        if ((lhs as object) == null) return ((rhs as object) == null);
                        if ((rhs as object) == null) return ((lhs as object) == null);
                        if (lhs.Section.CompareTo(rhs.Section) == 0 && lhs.Key.CompareTo(rhs.Key) == 0)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }

                    public static bool operator !=(SectionPair lhs, SectionPair rhs)
                    {
                        return !(lhs == rhs);
                    }
                }
                private Dictionary<SectionPair, String> keyPairs = new Dictionary<SectionPair, string>();
                private String iniFilePath;



                /// <summary>
                /// Opens the INI file at the given path and enumerates the values in the IniParser.
                /// </summary>
                /// <param name="iniPath">Full path to INI file.</param>
                public IniParser(String iniPath)
                {
                    TextReader iniFile = null;
                    String strLine = null;
                    String currentRoot = null;
                    String[] keyPair = null;

                    iniFilePath = iniPath;

                    if (File.Exists(iniPath))
                    {
                        try
                        {
                            iniFile = new StreamReader(iniPath);

                            strLine = iniFile.ReadLine();

                            while (strLine != null)
                            {
                                strLine = strLine.Replace(" ", "").Trim();

                                if (strLine != "")
                                {
                                    if (strLine.StartsWith("[") && strLine.EndsWith("]"))
                                    {
                                        currentRoot = strLine.Substring(1, strLine.Length - 2);
                                    }
                                    else
                                    {
                                        keyPair = strLine.Split(new char[] { '=' }, 2);

                                        SectionPair sectionPair = new SectionPair();
                                        String value = null;

                                        if (currentRoot == null)
                                            currentRoot = "ROOT";

                                        sectionPair.Section = currentRoot;
                                        sectionPair.Key = keyPair[0];

                                        if (keyPair.Length > 1)
                                            value = keyPair[1];

                                        keyPairs.Add(sectionPair, value);
                                    }
                                }

                                strLine = iniFile.ReadLine();
                            }

                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        finally
                        {
                            if (iniFile != null)
                                iniFile.Close();
                        }
                    }
                    else
                        throw new FileNotFoundException("Unable to locate " + iniPath);

                }

                /// <summary>
                /// Returns the value for the given section, key pair.
                /// </summary>
                /// <param name="sectionName">Section name.</param>
                /// <param name="settingName">Key name.</param>
                public String GetSetting(String sectionName, String settingName)
                {
                    String Ret = null;
                    SectionPair sectionPair = new SectionPair();
                    sectionPair.Section = sectionName;
                    sectionPair.Key = settingName;

                    if (keyPairs != null)
                    {
                        foreach (SectionPair Scr in keyPairs.Keys)
                        {
                            if (Scr.Section.CompareTo(sectionPair.Section) == 0 && Scr.Key.CompareTo(sectionPair.Key) == 0)
                            {
                                Ret = keyPairs[Scr];
                                break;
                            }
                        }
                    }
                    if (Ret == null)
                    {
                        Console.Write("INIParser GetSetting Error");
                    }
                    return Ret;
                }

                /// <summary>
                /// Enumerates all lines for given section.
                /// </summary>
                /// <param name="sectionName">Section to enum.</param>
                public String[] EnumSection(String sectionName)
                {
                    ArrayList tmpArray = new ArrayList();

                    foreach (SectionPair pair in keyPairs.Keys)
                    {
                        if (pair.Section == sectionName)
                            tmpArray.Add(pair.Key);
                    }

                    return (String[])tmpArray.ToArray(typeof(String));
                }

                /// <summary>
                /// Adds or replaces a setting to the table to be saved.
                /// </summary>
                /// <param name="sectionName">Section to add under.</param>
                /// <param name="settingName">Key name to add.</param>
                /// <param name="settingValue">Value of key.</param>
                public void AddSetting(String sectionName, String settingName, String settingValue)
                {
                    SectionPair sectionPair = new SectionPair();
                    sectionPair.Section = sectionName;
                    sectionPair.Key = settingName;

                    SectionPair exsitPair = ContainsKey(keyPairs, sectionPair);
                    if (exsitPair != null)
                        keyPairs.Remove(exsitPair);

                    keyPairs.Add(sectionPair, settingValue);
                }

                private SectionPair ContainsKey(Dictionary<SectionPair, String> sour, SectionPair target)
                {
                    foreach (SectionPair sourPair in sour.Keys)
                    {
                        if (sourPair == target)
                        {
                            return sourPair;
                        }
                    }
                    return null;
                }


                /// <summary>
                /// Adds or replaces a setting to the table to be saved with a null value.
                /// </summary>
                /// <param name="sectionName">Section to add under.</param>
                /// <param name="settingName">Key name to add.</param>
                public void AddSetting(String sectionName, String settingName)
                {
                    AddSetting(sectionName, settingName, null);
                }

                /// <summary>
                /// Remove a setting.
                /// </summary>
                /// <param name="sectionName">Section to add under.</param>
                /// <param name="settingName">Key name to add.</param>
                public void DeleteSetting(String sectionName, String settingName)
                {
                    SectionPair sectionPair = new SectionPair();
                    sectionPair.Section = sectionName;
                    sectionPair.Key = settingName;

                    if (keyPairs.ContainsKey(sectionPair))
                        keyPairs.Remove(sectionPair);
                }

                /// <summary>
                /// Save settings to new file.
                /// </summary>
                /// <param name="newFilePath">New file path.</param>
                public void SaveSettings(String newFilePath)
                {
                    ArrayList sections = new ArrayList();
                    String tmpValue = "";
                    String strToSave = "";

                    foreach (SectionPair sectionPair in keyPairs.Keys)
                    {
                        if (!sections.Contains(sectionPair.Section))
                            sections.Add(sectionPair.Section);
                    }

                    foreach (String section in sections)
                    {
                        strToSave += ("[" + section + "]\r\n");

                        foreach (SectionPair sectionPair in keyPairs.Keys)
                        {
                            if (sectionPair.Section == section)
                            {
                                tmpValue = (String)keyPairs[sectionPair];

                                if (tmpValue != null)
                                    tmpValue = "=" + tmpValue;

                                strToSave += (sectionPair.Key + tmpValue + "\r\n");
                            }
                        }

                        strToSave += "\r\n";
                    }

                    try
                    {
                        TextWriter tw = new StreamWriter(newFilePath);
                        tw.Write(strToSave);
                        tw.Close();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }

                /// <summary>
                /// Save settings back to ini file.
                /// </summary>
                public void SaveSettings()
                {
                    SaveSettings(iniFilePath);
                }
            }

        }


    }




}
