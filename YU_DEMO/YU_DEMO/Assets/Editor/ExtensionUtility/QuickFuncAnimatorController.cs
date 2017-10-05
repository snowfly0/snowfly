using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Diagnostics;
using UnityEditor.Animations;
using Debug = UnityEngine.Debug;
using System.Collections.Generic;

public class QuickFuncAnimatorController : ScriptableObject
{
    [MenuItem("NpcEditor/AnimatorController/在指定文件夹下生成AnimatorController")]
    static void CreateControllerInSelectedFolder()
    {
        string[] folders = EditorUtil.GetSelectedFoldersPathes();
        foreach (var path in folders)
        {
            string relativepath = path.Replace(Directory.GetCurrentDirectory() + "/", "");
            AnimationClip[] clips = EditorUtil.LoadAllAssetsInFolderChildren<AnimationClip>(path, ".fbx");
            if (clips.Length == 0)
            {
                Debug.LogError("无动画文件，生成控制器失败:" + path);
                continue;
            }
            string[] pathSpit = relativepath.Split('/');
            AnimatorController animatorController = AnimatorController.CreateAnimatorControllerAtPath(relativepath + "/" + pathSpit[pathSpit.Length - 1] + "Controller.controller");
            AnimatorControllerLayer layer = animatorController.layers[0];
            int xCount = 1;
            int yCount = 1;
            foreach (var clip in clips)
            {
                if (string.IsNullOrEmpty(clip.name))
                {
                    continue;
                }
                if (yCount < 10)
                {
                    AddStateTransition(clip, animatorController, layer, xCount, yCount);
                    yCount++;
                }
                else
                {
                    yCount = 1;
                    xCount++;
                    AddStateTransition(clip, animatorController, layer, xCount, yCount);
                    yCount++;
                }
            }
            var states = layer.stateMachine.states;
            for (int i = 0; i < states.Length; ++i)
            {
                for (int m = 0; m < states.Length; ++m)
                {
                    var trans = states[i].state.AddTransition(states[m].state);
                    trans.hasExitTime = false;
                    trans.duration = 0.25f;
                    trans.AddCondition(AnimatorConditionMode.If, 0f, states[m].state.name);
                }
            }
        }
    }




    [MenuItem("NpcEditor/AnimatorController/在指定文件夹下生成AnyStateAnimatorController")]
    public static void CreateControllerInSelectedFolder2()
    {
        string[] folders = EditorUtil.GetSelectedFoldersPathes();
        foreach (var path in folders)
        {
            string relativepath = path.Replace(Directory.GetCurrentDirectory() + "/", "");
            AnimationClip[] clips = EditorUtil.LoadAllAssetsInFolderChildren<AnimationClip>(path, ".fbx");
            if (clips.Length == 0)
            {
                Debug.LogError("无动画文件，生成控制器失败:" + path);
                continue;
            }
            string[] pathSpit = relativepath.Split('/');
            AnimatorController animatorController = AnimatorController.CreateAnimatorControllerAtPath(relativepath + "/" + pathSpit[pathSpit.Length - 1] + "Controller.controller");
            AnimatorControllerLayer layer = animatorController.layers[0];
            int xCount = 1;
            int yCount = 1;
            foreach (var clip in clips)
            {
                if(string.IsNullOrEmpty(clip.name))
                {
                    continue;
                }
                if (yCount < 10)
                {
                    AddAnyStateTransition(clip, animatorController, layer, xCount, yCount);
                    yCount++;
                }
                else
                {
                    yCount = 1;
                    xCount++;
                    AddAnyStateTransition(clip, animatorController, layer, xCount, yCount);
                    yCount++;
                }
            }

        }
    }

    private static void AddStateTransition(AnimationClip newClip, AnimatorController controller, AnimatorControllerLayer layer, int x, int y)
    {
        AnimatorStateMachine sm = layer.stateMachine;
        if (newClip == null) return;
        AnimatorState state = sm.AddState(newClip.name, new Vector3(sm.anyStatePosition.x + x * 200f, sm.anyStatePosition.y + y * 50f, sm.anyStatePosition.z));
        state.motion = newClip;
        //把state添加在layer里面
        //AnimatorStateTransition trans = sm.AddAnyStateTransition(state);
        ////把默认的时间条件删除
        //trans.duration = 0.25f;
        //trans.hasExitTime = false;
        controller.AddParameter(new AnimatorControllerParameter { type = AnimatorControllerParameterType.Bool, defaultBool = false, name = newClip.name });
        //  trans.AddCondition(AnimatorConditionMode.If, 0f, state.name);
    }

    private static void AddAnyStateTransition(AnimationClip newClip, AnimatorController controller, AnimatorControllerLayer layer, int x, int y)
    {
        AnimatorStateMachine sm = layer.stateMachine;
        if (newClip == null) return;
        AnimatorState state = sm.AddState(newClip.name, new Vector3(sm.anyStatePosition.x + x * 200f, sm.anyStatePosition.y + y * 50f, sm.anyStatePosition.z));
        state.motion = newClip;
        //把state添加在layer里面
        AnimatorStateTransition trans = sm.AddAnyStateTransition(state);
        ////把默认的时间条件删除
        trans.duration = 0.25f;
        trans.hasExitTime = false;
        controller.AddParameter(new AnimatorControllerParameter { type = AnimatorControllerParameterType.Bool, defaultBool = false, name = newClip.name });
        trans.AddCondition(AnimatorConditionMode.If, 0f, state.name);
    }
}