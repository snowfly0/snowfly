using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Actor.StateMac;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.GameScripts.ActorManager.StateMac.MacImplement
{
    /// <summary>
    /// 鱼逃跑状态
    /// </summary>
    public class FishRunAwayMac: ABSStateMac
    {
        public ActorEntity _actor;
        protected ActorAnimator _animator;
        public float _fishMoveSpeed=100f;
        public float _fishRotateSpeed=100f;
        public TankBehaviour TB;
        public override void Init(ActorEntity actor)
        {
            _actor = actor;
            _animator = actor.Animator;
            //Vector3 position=new Vector3(0,0,0);//TODO
            //var obj = GameObject.Instantiate(Resources.Load("PointPrefabs/RunAwayFunctionPoint")) as GameObject;
            //obj.transform.position = position;
            //var group = obj.GetComponent<TankGroup>();
            TB = actor.TB;
            //TB.myGroup = group;
            
        }

        public override void Update()
        {
            if ((_actor.transform.position - _actor.TB.myGroup.transform.position).magnitude <= 2)
            {
                _actor.StateController.ChangeState(ActorState.Wandering);
            }
        }

        public override void Reset()
        {
        }

        public override void OnStateEnter()
        {
            base.OnStateEnter();
            _actor.SwitchTB(true);
            TB.moveSpeed = _fishMoveSpeed;
            TB.rotateSpeed = _fishRotateSpeed;
        }

        public override void OnStateExit()
        {
            base.OnStateExit();
            _actor.SwitchTB(false);
            TB.SetGroup(Random.Range(_actor.Data.MinTankGroup,_actor.Data.MaxTankGroup));
            GameObject.Destroy(TB.myGroup.gameObject,1f);
        }
    }
}
