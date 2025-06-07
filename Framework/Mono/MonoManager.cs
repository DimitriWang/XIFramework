using System;

namespace XIFramework
{
    //TODO: Tick 接口化
    public interface ITick
    {
        void Tick(float deltaTime);
    }

    public interface IFixedTick
    {
        void FixedTick(float deltaTime);
    }

    public interface ILateTick
    {
        void LateTick(float deltaTime);
    }

    public interface IMonoTickInterface : IFixedTick, ITick, ILateTick
    {
        
    }
    
    public class MonoManager : SingletonMono<MonoManager>
    {
        private event Action updateAction;
        private event Action lateUpdateAction;
        private event Action fixedUpdateAction;

        public void AddUpdateAction(Action action)
        {
            updateAction += action;
        }

        public void AddLateUpdateAction(Action action)
        {
            lateUpdateAction += action;
        }

        public void AddFixedUpdateAction(Action action)
        {
            fixedUpdateAction += action;
        }

        public void RemoveUpdateAction(Action action)
        {
            updateAction -= action;
        }

        public void RemoveLateUpdateAction(Action action)
        {
            lateUpdateAction -= action;
        }

        public void RemoveFixedUpdateAction(Action action)
        {
            fixedUpdateAction -= action;
        }

        private void Update()
        {
            updateAction?.Invoke();
        }

        private void LateUpdate()
        {
            lateUpdateAction?.Invoke();
        }

        private void FixedUpdate()
        {
            fixedUpdateAction?.Invoke();
        }
    }
}