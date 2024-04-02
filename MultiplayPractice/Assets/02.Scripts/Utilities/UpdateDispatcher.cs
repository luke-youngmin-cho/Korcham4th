using MP.Singleton;
using System;
using System.Collections.Concurrent;

namespace MP.Utilities
{
    /// <summary>
    /// 멀티쓰레드 환경에서 UI 갱신 등은 UI 업데이트 쓰레드에서 실행되어야만 하는데, 
    /// Unity 는 GameLogic 주기에서 UI 갱신호출을 해야함. UI 뿐만아니라 하이어라키에있는 게임오브젝트의 
    /// 컴포넌트들에 대한 갱신을 할때는 GameLogic 에서 처리해야하므로 Queue 에 함수를 등록해놓고 Update 주기에 실행함.
    /// </summary>
    public class UpdateDispatcher : SingletonMonoBase<UpdateDispatcher>
    {
        private ConcurrentQueue<Action> _actions = new ConcurrentQueue<Action>();

        private void Update()
        {
            while (_actions.TryDequeue(out Action action))
            {
                action?.Invoke();
            }
        }

        public void Enqueue(Action action)
        {
            _actions.Enqueue(action);
        }
    }
}