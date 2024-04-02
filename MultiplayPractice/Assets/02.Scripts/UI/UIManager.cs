using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using MP.Singleton;

namespace MP.UI
{
    /// <summary>
    /// 모든 UI를 관리, 전체스크린용 및 팝업용 UI 추가로 관리.
    /// </summary>
    public class UIManager : SingletonMonoBase<UIManager>
    {
        private Dictionary<Type, IUI> _uis = new Dictionary<Type, IUI>(); // 타입으로 UI 검색
        private List<IUI> _screens = new List<IUI>(); // 활성화 되어있는 Screen UI 들
        private Stack<IUI> _popups = new Stack<IUI>(); // 활성화 되어있는 Popup UI 들


        private void Update()
        {
            UpdateInpuActions();
        }

        /// <summary>
        /// 현재 상호작용 가능한 UI 의 상호작용 업데이트
        /// </summary>
        public void UpdateInpuActions()
        {
            // 활성화된 Popup이 존재한다면 최상단 Popup UI 만 상호작용
            if (_popups.Count > 0)
            {
                if (_popups.Peek().inputActionEnable)
                    _popups.Peek().InputAction();
            }

            // 활성화된 Screen UI 가 존재한다면 모두 상호작용
            for (int i = _screens.Count - 1; i >= 0 ; i--)
            {
                if (_screens[i].inputActionEnable)
                    _screens[i].InputAction();
            }
        }

        /// <summary>
        /// 스크린용 UI 등록
        /// </summary>
        public void RegisterScreen(IUI ui)
        {
            if (_uis.TryAdd(ui.GetType(), ui))
            {
            }
            else
            {
                throw new Exception($"[UIManager] : UI 등록 실패. {ui.GetType()} 는 이미 등록되어있습니다...");
            }
        }

        /// <summary>
        /// 팝업용 UI 등록
        /// </summary>
        public void RegisterPopup(IUI ui)
        {
            if (_uis.TryAdd(ui.GetType(), ui))
            {
            }
            else
            {
                throw new Exception($"[UIManager] : UI 등록 실패. {ui.GetType()} 는 이미 등록되어있습니다...");
            }
        }

        /// <summary>
        /// Resolve. 원하는 UI 가져오는 함수
        /// </summary>
        /// <typeparam name="T"> 가져오고 싶은 UI 타입 Type 객체 </typeparam>
        public T Get<T>()
            where T : IUI
        {
            return (T)_uis[typeof(T)];
        }

        /// <summary>
        /// 새로 활성화될 Popup UI 관리
        /// </summary>
        public void PushPopup(IUI ui)
        {
            if (_popups.Count > 0)
                _popups.Peek().inputActionEnable = false; // 기존에 있던 팝업에 입력 안먹히게

            _popups.Push(ui);
            _popups.Peek().inputActionEnable = true; // 새로 띄울 팝업에 입력 먹히게
            _popups.Peek().sortingOrder = _popups.Count; // 새로 띄울 팝업을 최상단으로 정렬
        }

        /// <summary>
        /// 닫으려는 Popup UI 관리
        /// </summary>
        /// <exception cref="Exception"> Popup UI는 최상단 부터만 닫을 수 있음. 닫으려는 UI 가 최상단에 없을경우 </exception>
        public void PopPopup(IUI ui)
        {
            // 닫으려는 UI 가 최상단에 있지 않으면 예외
            if (_popups.Peek() != ui)
                throw new Exception($"[UIManager] : {ui.GetType()} 팝업을 닫기 시도했으나 최상단에 있지않음..");

            _popups.Pop();

            // 이전 팝업의 입력이 먹히게
            if (_popups.Count > 0)
                _popups.Peek().inputActionEnable = true;
        }

        /// <summary>
        /// 전체화면 UI 를 설정
        /// </summary>
        public void SetScreen(IUI ui)
        {
            for (int i = _screens.Count - 1; i >= 0; i--)
            {
                _screens[i].Hide();
                _screens.RemoveAt(i);
            }

            _screens.Add(ui);
        }
    }
}
