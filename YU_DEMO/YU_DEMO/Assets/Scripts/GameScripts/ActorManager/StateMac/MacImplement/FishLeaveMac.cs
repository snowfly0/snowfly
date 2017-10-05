using UnityEngine;
using System.Collections;
using DG.Tweening;
namespace Actor.StateMac
{
    public class FishLeaveMac : ABSStateMac
    {
        ActorEntity _actor;
        ActorAnimator _animator;

        TankGroup _myGroup;
        Tweener _leaveTweener;

        public override void Init(ActorEntity actor)
        {
            _actor = actor;
            _animator = actor.Animator;
        }



        public override void Update()
        {
            //if (Vector3.Distance(_actor.transform.position, _myGroup.targetPosition) < 15)
            //{
            //    Leave();
            //}
        }

        public override void Reset()
        {

        }

        public override void OnStateExit()
        {
            base.OnStateExit();
            if(_leaveTweener!=null)
            {
                _leaveTweener.Kill();
                _leaveTweener = null;
            }
        }

        public override void OnStateEnter()
        {
            base.OnStateEnter();
            TankGroup t1= TankGroup.GetTankGroup(-1);
            TankGroup t2 = TankGroup.GetTankGroup(-2);
            Vector3 pos = _actor.transform.position;
            float distance1=Vector3.Distance(t1.targetPosition,pos);
            float distance2 = Vector3.Distance(t2.targetPosition, pos);
            _myGroup = distance1 < distance2 ? t1 : t2;
            float dis = Mathf.Min(distance1,distance2);
            float time = dis / _actor.Data.MoveSpeed;
            _actor.transform.LookAt(_myGroup.targetPosition);
            _leaveTweener = DOTween.To(() => _actor.transform.position, x => _actor.transform.position = x, _myGroup.targetPosition, time)
                .SetEase(Ease.Linear).OnComplete(Leave);
            //_actor.TB.SetGroup(_myGroup.groupID);
            //_actor.SwitchTB(true);
        }


        void Leave()
        {
            ActorPool.Return(_actor);
        }
    }
}