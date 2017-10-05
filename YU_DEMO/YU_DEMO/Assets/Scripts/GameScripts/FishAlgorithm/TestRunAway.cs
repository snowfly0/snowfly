using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.GameScripts.ActorManager.StateMac.MacImplement;
using UnityEngine;

namespace Assets.Scripts.GameScripts.FishAlgorithm
{
    public class TestRunAway:MonoBehaviour
    {
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = gameObject.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {
                    ActorEntity actor = hit.transform.GetComponent<ActorEntity>();
                    if (actor)
                    {
                        Vector3 position = actor.transform.forward*20 + actor.transform.position;
                        Debug.LogError(actor.transform.position);
                        Debug.LogError(position);
                        var obj = GameObject.Instantiate(Resources.Load("PointPrefabs/RunAwayFunctionPoint")) as GameObject;
                        obj.transform.position = position;
                        var group = obj.GetComponent<TankGroup>();
                        actor.TB.SetGroup(group.groupID);
                        actor.StateController.ChangeState(ActorState.RunAway);
                        actor.TB.SetGroup(group);
                    }
                }
            }
        }
    }
}
