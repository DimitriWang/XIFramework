using System;
using System.Collections.Generic;

namespace XIFramework.Machine
{
    public interface IStateMachineOwner
    {
    }

    public class StateMachine
    {
        private IStateMachineOwner _owner;

        private Dictionary<Type, StateBase> stateDic = new Dictionary<Type, StateBase>();

        public bool hasState => currentState != null;

        public Type CurrentStateType
        {
            get { return currentState.GetType(); }
        }

        private StateBase currentState;

        /// <summary>
        /// 初始化 状态机
        /// </summary>
        /// <param name="owner"></param>
        public void Init(IStateMachineOwner owner)
        {
            this._owner = owner;
        }

        /// <summary>
        /// 切换状态
        /// </summary>
        /// <param name="refresh">状态不变时，刷新条件参数</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool ChangeState<T>(bool refresh = false) where T : StateBase, new()
        {
            if (currentState != null && CurrentStateType == typeof(T) && !refresh) return false;

            if (currentState != null)
            {
                currentState.Exit();
                MonoHelper.Instance.RemoveUpdateAction(currentState.Update);
                MonoHelper.Instance.RemoveFixedUpdateAction(currentState.FixedUpdate);
                MonoHelper.Instance.RemoveLateUpdateAction(currentState.LateUpdate);
            }

            currentState = GetState<T>();
            currentState.Enter();
            MonoHelper.Instance.AddUpdateAction(currentState.Update);
            MonoHelper.Instance.AddFixedUpdateAction(currentState.FixedUpdate);
            MonoHelper.Instance.AddLateUpdateAction(currentState.LateUpdate);

            return true;
        }

        private StateBase GetState<T>() where T : StateBase, new()
        {
            Type stateType = typeof(T);
            if (!stateDic.TryGetValue(stateType, out StateBase state))
            {
                state = new T();
                state.Init(_owner);
                stateDic.Add(stateType, state);
            }

            return state;
        }

        public void Stop()
        {
            if (hasState)
            {
                currentState.Exit();
                MonoHelper.Instance.RemoveUpdateAction(currentState.Update);
                MonoHelper.Instance.RemoveFixedUpdateAction(currentState.FixedUpdate);
                MonoHelper.Instance.RemoveLateUpdateAction(currentState.LateUpdate);
            }

            foreach (var state in stateDic)
            {
                var iterState = state.Value;
                iterState.DeInit();
            }

            stateDic.Clear();

            currentState = null;
        }
    }
}