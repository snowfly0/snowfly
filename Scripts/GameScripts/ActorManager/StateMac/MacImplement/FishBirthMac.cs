using UnityEngine;
using System.Collections;
using System;

namespace Actor.StateMac
{
    public class FishBirthMac : ABSStateMac
    {
        ActorEntity _actor;
        ActorAnimator _animator;


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
            Birth();
        }

        void Birth()
        {//-------------------------在指定区域内随机出生点；
            _fishBirthPoint FBP = BirthArea.Instance.GetArea(_actor.Data.BirthZoneName); //GameObject.Find(_actor.Data.BirthZoneName).GetComponent<_fishBirthPoint>();//--获得场景内目标出现区域的组件；
            _actor.transform.position = FBP._birthPoint();//---------------重新定义出生的鱼的坐标点；
            _actor.transform.localEulerAngles = new Vector3(0, 180, 0);
            //GameObject go = new GameObject("[BPTEST]");
            // go.transform.position = _actor.transform.position;
            ShowBirthLight();
            OnBirthAnimaEnd();
            //_animator.SetLoop(false);
            //_animator.Play("Birth", OnBirthAnimaEnd);
        }

        void OnBirthAnimaEnd()
        {
            ChangeState(ActorState.Showing);
        }

        void ShowBirthLight()
        {//--------------------------------------------------出生的时候周围发光特效；
            GameObject birthLight = BirthEffectPool.GetOne();
            birthLight.transform.position = _actor.transform.position;
            birthLight.transform.localScale = Vector3.zero;
            birthLight.gameObject.SetActive(true);
            AudioSource fallwaterAudio = birthLight.GetComponent<AudioSource>();
            fallwaterAudio.Play();//-------------------------------------------播放出生时候的音效；
            iTween.ScaleTo(birthLight, new Vector3(2f, 2f, 2f), 1.0f);

            TimerService.AddTimer<GameObject>(2.0f,
                (light) =>
                {
                    iTween.ScaleTo(light, Vector3.zero, 1.0f);
                    TimerService.AddTimer<GameObject>(1.0f, DisableBirthLight, light);
                }
                , birthLight);
        }
        void DisableBirthLight(GameObject light)
        {//----------------------------在鱼出生的时候产生白色的光柱；
            BirthEffectPool.Return(light);
        }
    }

}