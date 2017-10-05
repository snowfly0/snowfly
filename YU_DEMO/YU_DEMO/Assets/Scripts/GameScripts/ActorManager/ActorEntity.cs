using UnityEngine;
using System.Collections.Generic;
using Actor.StateMac;
public class ActorEntity : MonoBehaviour
{
    public static readonly int TextureWidth = 1754;
    public static readonly int TextureHeight = 1240;
    public static bool IsAnyOneShowing { get { return ShowingCount > 0; } }//是否在展示时候
    public static int ShowingCount;
    public int TypeId { get; protected set; }

    /// <summary>
    /// 场景Id，仅在作为内置鱼的时候有效
    /// </summary>
    public int SceneId { get; protected set; }

    public string QRCode { get; protected set; }

    public string MD5Code { get; protected set; }

    public ActorData Data { get; protected set; }

    public ABSStateMac CurrentMac; /*{ get; protected set; }*/
    public ActorState CurrentState;/* { get; protected set; }
*/
    public ActorState PreviousState { get; protected set; }

    public ActorType ActorType { get; protected set; }

    public ActorAnimator Animator { get; protected set; }

    public ABSStateController StateController { get; protected set; }

    public TankBehaviour TB;

    public bool CanMove { get { return Data.CanMove; } }

    public int UserId { get; protected set; }

    public int GrowValue { get; protected set; }

    public Renderer MainRender { get; protected set; }

    Renderer[] _renderers;

    GameObject _bubble;

    Transform _bubbleTarget;

    Rigidbody _rigidbody;

    Texture[] _orgTextures;

    Vector3 _initScale;

    public void Init(ActorData data)
    {
        Animator = new ActorAnimator(ActorUtil.FindComponentInChildren<Animator>(transform, true));
        _bubbleTarget = transform.FindChild("BubbleTarget");
        if (!_bubbleTarget)
        {
            _bubbleTarget = transform;
        }
        this.Data = data;
        TypeId = data.Id;
        _renderers = GetComponentsInChildren<Renderer>(true);
        if (_renderers.Length < 1)
        {
            Debug.LogError("can't render in a actor");
        }
        foreach (Renderer ren in _renderers)
        {
            if (ren.CompareTag("MainRender"))
            {
                MainRender = ren;
            }
            else if (ren.CompareTag("FishTooth"))
            {
                ren.material = MaterialLoader.Instance.GetMaterial(TypeId, true);
                continue;
            }
            else if (ren.CompareTag("FishParticle"))
            {
                continue;
            }
            Material mat = MaterialLoader.Instance.GetMaterial(TypeId, !Scanner.IsTestTexture, Scanner.IsTestTexture);
            if (mat)
            {
                Material[] mats = new Material[ren.materials.Length];
                for (int i = 0; i < mats.Length; ++i)
                {
                    if (i == Data.MutiMatIndex)
                    {
                        mats[Data.MutiMatIndex] = mat;
                    }
                    else
                    {
                        mats[i] = MaterialLoader.Instance.GetMaterial(TypeId, true);
                    }
                }
                ren.materials = mats;
            }
        }
        if (!MainRender)
        {
            Debug.LogError(Data.FishClassName + "没有多维材质");//把模型tag改成MainRender就好了   爸爸！！
            MainRender = _renderers[0];
        }
        int index = 0;
        _orgTextures = new Texture[_renderers.Length];
        foreach (var rend in _renderers)
        {
            _orgTextures[index++] = rend.materials.Length > 1 ? rend.materials[Data.MutiMatIndex].mainTexture : rend.material.mainTexture;
        }

        _initScale = transform.localScale;

        _rigidbody = GetComponent<Rigidbody>();
        TB = gameObject.AddComponent<TankBehaviour>();
        TB.groupId = data.StartGroupId;

        DOReset();
    }

    public void Create(Scanner.CreateInfo info)
    {
        QRCode = info.QRCode;
        MD5Code = info.MD5Hash;
        UserId = info.UserID;
        GrowValue = info.GrowValue;
        float scale = this.GetScale();
        transform.localScale = _initScale * scale;
        ActorType = ActorType.ScanFish;
        CreateController();
    }

    public void SetQrcode(string qrcode)
    {
        QRCode = qrcode;
    }

    public void Create(SceneFishData data)
    {
        SceneId = data.Id;
        ActorType = ActorType.SceneFish;
        CreateController();
    }

    void CreateController()
    {
        StateController = ABSStateController.Create(ActorType);
        if (StateController != null)
        {
            StateController.Init(this);
        }
        else
        {
            Debug.LogError(string.Format("Miss stateController of fishType:{0},prefabName:{1}", TypeId, Data.PrefabName));
        }
    }

    public void Run()
    {
        if (StateController != null)
        {
            StateController.Start();
        }
        else
        {
            Debug.LogError(string.Format("Miss stateController of fishType:{0},prefabName:{1}", TypeId, Data.PrefabName));
        }
    }

    /// <summary>
    /// 返回App
    /// </summary>
    public void ReturnApp()
    {
        if (TryChangeState(ActorState.Leaving))
        {
            this.QRCode = null;
        }
    }
    void Update()
    {
        Animator.UpdateAnimator();
        UpdateMacs();
    }
    void UpdateMacs()
    {
        if (CurrentMac != null)
        {
            CurrentMac.Update();
        }
    }


    public void ChangeState(ABSStateMac mac)
    {
        if (CurrentState != mac.Type)
        {
            PreviousState = CurrentState;
            ExitCurrentState();
            CurrentMac = mac;
            CurrentState = mac.Type;
            mac.OnStateEnter();
        }
    }

    /// <summary>
    /// 中断当前状态并切换状态
    /// </summary>
    /// <param name="state"></param>
    public bool TryChangeState(ActorState state)
    {
        if (this.StateController.CanChangeState(state))
        {
            this.StateController.ChangeState(state);
            return true;
        }
        return false;
    }



    public void NotifyFeed()
    {
        if (_bubble)
        {
            return;
        }
        ActorState state = ActorState.EatFood;
        if (this.StateController.CanChangeState(state))
        {
            this.StateController.ChangeState(state);
        }
    }

    void ExitCurrentState()
    {
        if (CurrentMac != null)
        {
            CurrentMac.OnStateExit();
        }
    }

    public void SetTexture(Texture texture)
    {
        foreach (var render in _renderers)
        {
            if (render.CompareTag("FishTooth") || render.CompareTag("FishParticle"))
            {
                continue;
            }
            //身体上贴图，眼睛不上
            if (render.materials.Length == 1)
            {
                render.material.mainTexture = texture;
            }
            else if (render.materials.Length > 1)
            {
                render.materials[Data.MutiMatIndex].mainTexture = texture;
            }
        }
    }

    public void SwitchTB(bool isOn)
    {
        if (TB)
        {
            TB.enabled = isOn;
        }
    }


    void ResetTextures()
    {
        int index = 0;
        foreach (Renderer render in _renderers)
        {
            Texture texture = _orgTextures[index++];
            if (render.CompareTag("FishTooth") || render.CompareTag("FishParticle"))
            {
                continue;
            }
            if (render.materials.Length == 1)
            {
                render.material.mainTexture = texture;
            }
            else if (render.materials.Length > 1)
            {
                render.materials[Data.MutiMatIndex].mainTexture = texture;
            }
        }
    }
    public void DOReset()
    {
        ExitCurrentState();
        if (StateController != null)
        {
            StateController.Reset();
            StateController = null;
        }
        SwitchTB(false);
        ResetTextures();
        Animator.Reset();
        transform.localScale = _initScale;
    }
}
