using UnityEngine;
using System.Collections;
namespace Actor.StateMac
{
    public class FishWanderMac : ABSStateMac
    {
        protected ActorEntity _actor;
        protected ActorAnimator _animator;
        GameObject _backPoint;
        bool _isMovingToPath;
        protected float _fishMoveSpeed;
        protected float _fishRotateSpeed;
        protected float _viewFishDistanceLimit;
        protected int _minGroup;
        protected int _maxGroup;
        protected TankBehaviour TB;
        float _changeTargetTime = 30f;
        float _currentTime;

        public override void Init(ActorEntity actor)
        {
            _actor = actor;
            _animator = actor.Animator;
            _fishMoveSpeed = actor.Data.MoveSpeed;
            _fishRotateSpeed = actor.Data.RoateSpeed;
            _viewFishDistanceLimit = actor.Data.ViewFishDistanceLimit;
            _minGroup = _actor.Data.MinTankGroup;
            _maxGroup = _actor.Data.MaxTankGroup;
        }



        public override void Update()
        {
            if (_isMovingToPath)
            {
                if (Vector3.Distance(_actor.transform.position, _backPoint.transform.position) < _viewFishDistanceLimit)
                {
                    _isMovingToPath = false;
                    TB.SetGroup(Random.Range(_actor.Data.MinTankGroup, _actor.Data.MaxTankGroup));
                }
                else
                    return;
            }
            if (Vector3.Distance(_actor.transform.position, TB.myGroup.transform.position) < 5)
            {
                TB.moveSpeed = 0;
                TB.rotateSpeed = 0;
                //_currentTime = _changeTargetTime;
            }
            else
            {
                TB.moveSpeed = _fishMoveSpeed;
                TB.rotateSpeed = _fishRotateSpeed;
            }
            _currentTime += Time.deltaTime;
            if (_currentTime >= _changeTargetTime)
            {
                _currentTime = 0f;
                TB.SetGroup(Random.Range(_minGroup,_maxGroup));
            }
        }

        public override void Reset()
        {
            _currentTime = 0f;
        }

        public override void OnStateEnter()
        {
            TB = _actor.TB;
            _actor.SwitchTB(true);
            if(_actor.PreviousState==ActorState.EatFood)
            {
                _animator.Play("Move");
                _animator.SetLoop(true);
            }
            else if (_actor.ActorType == ActorType.SceneFish)
            {
                _animator.Play("Move");
                _animator.SetLoop(true);
            }
            else
            {
                GoToBackPoint();
            }
        }


        public override void OnStateExit()
        {
            base.OnStateExit();
            _actor.SwitchTB(false);
        }

        void GoToBackPoint()
        {
            _backPoint = TankGroup.GetTankGroup(52).gameObject;
            if (!_backPoint)
            {
                UnityEngine.Debug.LogError("Can't find BackPoint52");
                return;
            }
            else
            {
                TB = _actor.TB;
                TB.SetGroup(52);
                TB.moveSpeed = 30;
                TB.rotateSpeed = 60;
                _isMovingToPath = true;
            }
        }
    }
}