using UnityEngine;
using System.Collections;
using DG.Tweening;

namespace Actor.StateMac
{
    public class FishShowMac : ABSStateMac
    {
        enum State
        {
            None,
            Drop,
            Birth,
            MoveToView,
            Viewing
        }
        ActorEntity _actor;
        Transform _actorTransf;
        ActorAnimator _animator;
        GameObject _view;
        float _viewFishDistanceLimit;
        bool _isMoveToViewPoint;
        public bool IsClicked { get; protected set; }
        const float VIEW_TIME = 16.0f;
        BaseTimer _timer;
        Tweener _tweener;
        Transform _root;
        State _state;
        public override void Init(ActorEntity actor)
        {
            _actor = actor;
            _animator = actor.Animator;
            _actorTransf = actor.transform;
            _viewFishDistanceLimit = _actor.Data.ViewFishDistanceLimit;
        }



        public override void Update()
        {
            //if (!_isMoveToViewPoint) return;
            //float fishToPointDistance = Vector3.Distance(_actorTransf.position, _view.transform.position);
            //if (fishToPointDistance <= _viewFishDistanceLimit)
            //{
            //        _isMoveToViewPoint = false;
            //        _actor.TB.moveSpeed = 0;
            //        _actor.TB.rotateSpeed = 0;
            //        OnBirthAnimaEnd();
            //}
        }

        public bool ToggleClick()
        {
            if(IsClicked)
            {
                IsClicked = false;
                switch (_state)
                {
                    case State.MoveToView:
                        {
                            _tweener.TogglePause();
                            break;
                        }
                    case State.Viewing:
                        {
                            _timer.IsPause = false;
                            break;
                        }
                }
                return true;
            }
            switch(_state)
            {
                case State.MoveToView:
                    {
                        _tweener.TogglePause();
                        break;
                    }
                case State.Viewing:
                    {
                        _timer.IsPause = true;
                        break;
                    }
                default:
                    return false;
            }
            IsClicked = true;
            return true;
        }



        void OnBirthAnimaEnd()
        {
            _state = State.Viewing;
            _tweener = null;
            _root=_actor.transform.parent;
            _actor.transform.SetParent(_view.transform);//Test
            iTween.RotateTo(_actor.gameObject, new Vector3(0, -90, 0), 4);
            AudioClip clip = AudioClipManager.Instance.GetClip(ClipType.MyFish, _actor.Data.Id);
            SceneManager.audios.clip = clip;
            SceneManager.audios.Play();
            //_animator.SetLoop(true);
            //_animator.Play("Move");
            _timer = TimerService.AddTimer(VIEW_TIME,
                    () =>
                    {
                        _timer = null;
                        _actor.transform.SetParent(_root);
                        ChangeState(ActorState.Wandering);
                    });
        }

        public override void OnStateExit()
        {
            base.OnStateExit();
            if (_timer != null)
            {
                _timer.Kill(false);
                _timer = null;
            }
            if(_tweener!=null)
            {
                _tweener.Kill();
                _tweener = null;
            }
            _state = State.None;
            IsClicked = false;
            ActorEntity.ShowingCount--;
        }

        public override void Reset()
        {
            _isMoveToViewPoint = false;
        }

        public override void OnStateEnter()
        {
            _state = State.Drop;
            base.OnStateEnter();
            _view = TankGroup.GetTankGroup(_actor.TypeId).gameObject;
            _animator.Play("Move");
            _animator.SetLoop(true);
            Vector3 pos=_actor.transform.position;
            float y = _actor.transform.position.y - 25;
            _tweener=DOTween.To(() => _actor.transform.position, x => _actor.transform.position = x, new Vector3(pos.x, y, pos.z), 3.0f)
                .OnComplete(PlayBirthAnima).SetEase(Ease.OutExpo);
            ActorEntity.ShowingCount++;
        }

        void PlayBirthAnima()
        {
            _state = State.Birth;
            _animator.SetLoop(false);
            _animator.Play("Birth", MoveToViewpoint);
        }


        void MoveToViewpoint()
        {
            _state = State.MoveToView;
            _tweener = DOTween.To(() => _actor.transform.position, x => _actor.transform.position = x, _view.transform.position, 5.0f)
               .OnComplete(OnBirthAnimaEnd);
            //_actor.SwitchTB(true);
            //_actor.TB.SetGroup(_actor.TypeId);
            //_actor.TB.moveSpeed = _actor.Data.MoveSpeed;
            //_actor.TB.rotateSpeed = _actor.Data.RoateSpeed;
            //_isMoveToViewPoint = true;
            _animator.Play("Move");
            _animator.SetLoop(true);
        }
    }
}