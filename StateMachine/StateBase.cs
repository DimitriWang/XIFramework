namespace XIFramework.Machine
{
    public interface IStateBase
    {
    }

    /// <summary>
    /// 状态基类
    /// </summary>
    public abstract class StateBase : IStateBase
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="onwer">宿主</param>
        /// <param name="stateType">当前状态代表的实际枚举的int直</param>
        public virtual void Init(IStateMachineOnwer onwer)
        {
        }

        public virtual void DeInit()
        {
        }

        /// <summary>
        /// 状态退出时 执行
        /// </summary>
        public virtual void Enter()
        {
        }


        public virtual void Exit()
        {
        }

        public virtual void Update()
        {
        }

        public virtual void LateUpdate()
        {
        }

        public virtual void FixedUpdate()
        {
        }
    }
}

