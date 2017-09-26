using UnityEngine;
using System.Collections;
using System;

namespace Actor.StateMac
{
    public class FishEatMac : ABSStateMac
    {
        ActorEntity _actor;
        ActorAnimator _animator;

        int _eatTimes = 0;
        public override void Init(ActorEntity actor)
        {
            _actor = actor;
            _animator = actor.Animator;
        }



        public override void Update()
        {
            
        }

        public override void Reset()
        {
            
        }

        public override void OnStateEnter()
        {
            base.OnStateEnter();
            _eatTimes = 0;
            _animator.SetLoop(false);
            _animator.Play("EatFood", OnAnimaEnd);
        }

   

        void OnAnimaEnd()
        {
            if(++_eatTimes<3)
            {
                _animator.Play("EatFood", OnAnimaEnd);
                return;
            }
            if (_actor.ActorType == ActorType.ScanFish)
            {
                ChangeState(ActorState.Wandering);
            }
            else if(_actor.ActorType == ActorType.SceneFish)
            {
                ChangeState(ActorState.SceneWandering);
            }
        }

    }

}