using UnityEngine;
using UnityEditorInternal;
using UnityEditor;
using System.Collections.Generic;
using UnityEditor.Utils;

public class QuickFunCreatePrefab : ScriptableObject
{
    [MenuItem("NpcEditor/为选定文件夹下的模型生成预设（到场景）")]
    static List<GameObject> DoIt()
    {
        GameObject model = null;
        List<GameObject> objects = new List<GameObject>();
        foreach (string path in EditorUtil.GetSelectedFoldersPathes(false))
        {
            GameObject[] gos = EditorUtil.LoadAllAssetsInFolderChildren<GameObject>(path, ".fbx");
            foreach (GameObject go in gos)
            {
                if (!go.name.Contains("@") && go.GetComponent<Animator>())
                {
                    model = go;
                    break;
                }
            }
            if (model)
            {
                GameObject newPrefab = CreateForModel(model, path);
                objects.Add(newPrefab);
            }
        }

        return objects;
    }

    static GameObject CreateForModel(GameObject model, string folderPath)
    {
        GameObject prefab = Resources.Load("ActorPrefabs/" + model.name) as GameObject;
        GameObject animatorNode = GameObject.Instantiate<GameObject>(model);
        animatorNode.name = model.name;
        //特殊处理，设置Tag
        SetRenderTag(animatorNode.transform);
        GameObject prefRoot = null;
        if (prefab)
        {
            prefRoot = GameObject.Instantiate<GameObject>(prefab);
            prefRoot.name = model.name;
            Transform child = prefRoot.transform.FindChild(model.name);
            if(child)
            {
                GameObject.DestroyImmediate(child.gameObject);
            }
            child = prefRoot.transform.FindChild("BubbleTarget");
            if(!child)
            {
                child = new GameObject("BubbleTarget").transform;
                child.SetParent(prefRoot.transform, false);
            }
        }
        else
        {
            prefRoot = new GameObject(model.name);
        }
        animatorNode.transform.SetParent(prefRoot.transform);
        animatorNode.transform.SetAsFirstSibling();
        animatorNode.transform.localScale = Vector3.one;
        animatorNode.transform.localPosition = Vector3.zero;
        if (prefab)
        {
            Animator animatorPref = ActorUtil.FindComponentInChildren<Animator>(prefab.transform, true);
            Transform animatorPrefParent = animatorPref.transform.parent;

            //CopyComponentIfExist<Animator>(go, animatorNode, true);
            if (animatorPref)
            {
                Animator anima = animatorNode.GetComponent<Animator>();
                if (animatorPref.runtimeAnimatorController)
                {
                    anima.runtimeAnimatorController = animatorPref.runtimeAnimatorController;
                }
                else
                {
                    SetAnimaController(animatorNode, folderPath);
                }
                anima.cullingMode = animatorPref.cullingMode;
            }
            prefRoot.transform.localScale = prefab.transform.localScale;
            GameObject colliderNode = animatorPref.gameObject;
            if (animatorPrefParent)
            {
                animatorNode.transform.localPosition = animatorPref.transform.localPosition;
                CopyComponentIfExist<Rigidbody>(animatorPrefParent.gameObject, prefRoot);
            }
            else
            {
                CopyComponentIfExist<Rigidbody>(colliderNode, prefRoot);
            }
            CopyComponentIfExist<Collider>(colliderNode, animatorNode);

        }
        else
        {
            //如果事先不存在预设，使用默认参数
            SetAnimaController(animatorNode, folderPath);
            Rigidbody rig = prefRoot.AddComponent<Rigidbody>();
            rig.useGravity = false;
            rig.constraints = RigidbodyConstraints.FreezeRotation;
            rig.mass = 500;
            rig.drag = 500;
        }
        return prefRoot;
    }

    static void SetAnimaController(GameObject animatorNode, string folderPath)
    {
        string[] names = folderPath.Split('/');
        string name = names[names.Length - 1];
        RuntimeAnimatorController controller = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(folderPath + "/" + name + "Controller" + ".controller");
        if (controller)
        {
            Animator anima = animatorNode.GetComponent<Animator>();
            anima.runtimeAnimatorController = controller;
            anima.cullingMode = AnimatorCullingMode.AlwaysAnimate;
        }
    }



    static void CopyComponentIfExist<T>(GameObject src, GameObject dest, bool isOnlyValue = false) where T : Component
    {
        T t = src.GetComponent<T>();
        if (t)
        {
            ComponentUtility.CopyComponent(t);
            if (isOnlyValue && dest.GetComponent<T>())
            {
                ComponentUtility.PasteComponentValues(dest.GetComponent<T>());
            }
            else
                ComponentUtility.PasteComponentAsNew(dest);
        }
    }

    static void SetRenderTag(Transform src)
    {
        foreach (Transform t in src)
        {
            Renderer rSrc = t.GetComponent<Renderer>();
            if (rSrc && rSrc.sharedMaterials.Length == 2)
            {
                rSrc.tag = "MainRender";
                break;
            }
        }
    }


    [MenuItem("NpcEditor/为选定文件夹下的模型生成预设（到Resource目录）")]
    static void DoIt1()
    {
        if (!EditorUtil.DisplayNotify("会覆盖原有预设，确定？"))
        {
            return;
        }
        List<GameObject> objects = DoIt();
        foreach (GameObject go in objects)
        {
            string path = "Assets/Resources/ActorPrefabs/";
            GameObject p = PrefabUtility.CreatePrefab(path + go.name + ".prefab", go, ReplacePrefabOptions.ConnectToPrefab);
            Debug.Log(string.Format("创建预设{0}于路径{1}", p.name, path));
        }
    }



}