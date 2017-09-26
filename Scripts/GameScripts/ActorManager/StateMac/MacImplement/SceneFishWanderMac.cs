using UnityEngine;
using System.Collections;
namespace Actor.StateMac
{
    public class SceneFishWanderMac : FishWanderMac
    {
        SceneFishData _data;

        public override void Init(ActorEntity actor)
        {
            _actor = actor;
            _animator = actor.Animator;
            SceneFishData data = SceneFishData.GetData(actor.SceneId);
            _minGroup = data.MinTankGroup;
            _maxGroup = data.MaxTankGroup;
            _fishMoveSpeed = data.MoveSpeed;
            _fishRotateSpeed = data.RotateSpeed;
            _data = data;
        }

        public override void OnStateEnter()
        {
            bool isFeed = _actor.PreviousState == ActorState.EatFood;
            if (!isFeed)
            {
                _actor.transform.position = BirthArea.Instance.GetArea(_data.BirthZoneName)._birthPoint();
                base.OnStateEnter();
                TB.groupId = _data.StartGroupId;
                TB.SetGroup(_data.StartGroupId);
            }
            else
            {
                base.OnStateEnter();
            }
        }

        public override void Reset()
        {
            base.Reset();
        }

        public override void Update()
        {
            base.Update();
        }
    }
}