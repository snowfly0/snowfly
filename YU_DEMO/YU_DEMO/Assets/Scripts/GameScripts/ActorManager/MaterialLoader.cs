using System;
using System.Collections.Generic;
using UnityEngine;

public class MaterialLoader : XRSingleton<MaterialLoader>
{
    Dictionary<int, Material> _materials = new Dictionary<int, Material>();
    Dictionary<int, Texture> _textures = new Dictionary<int, Texture>();
    Dictionary<int, Texture> _testTextures = new Dictionary<int, Texture>();

    /// <summary>
    /// 获取材质球
    /// </summary>
    /// <param name="type"></param>
    /// <param name="isGlobalShare">是否全局共享（节省内存）</param>
    /// <returns></returns>
    public Material GetMaterial(int type, bool isGlobalShare = false,bool isTest=false)
    {
        Material mat;
        if (!_materials.TryGetValue(type, out mat))
        {
            mat = Resources.Load<Material>("ActorMaterials/Mat/" + type);
            if (mat)
            {
                _materials.Add(type, mat);
            }
            else
            {
                return null;
            }
        }
        Material result = isGlobalShare ? mat : new Material(mat);
        result.mainTexture =isTest?GetTestTexture(type):GetTexture(type);
        return result;
    }

    public Texture GetTexture(int type)
    {
        Texture tex;
        if (!_textures.TryGetValue(type, out tex))
        {
            string path = "ActorMaterials/Tex/";
            tex = Resources.Load<Texture>(path + type);
            if (tex)
            {
                _textures.Add(type, tex);
            }
        }
        return tex;
    }

    public Texture GetTestTexture(int type)
    {
        Texture tex;
        if (!_testTextures.TryGetValue(type, out tex))
        {
            string path = "ActorMaterials/TexTest/";
            tex = Resources.Load<Texture>(path + type);
            if (tex)
            {
                _testTextures.Add(type, tex);
            }
        }
        return tex;
    }
}

