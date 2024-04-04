using MP.GameElements;
using Photon.Pun;
using UnityEngine.UI;
using MP.GameElements.Characters;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using MP.GameElements.InteractingSystem;
using System.Collections;

namespace MP.UI
{
    public class UIPlayerAction : UIScreenBase
    {
        private PlayerInteractor observingTarget
        {
            get
            {
                if (_observingTarget == null)
                {
                    if (ClientCharacterController.TryGetLocal(out ClientCharacterController controller))
                    {
                        _observingTarget = controller.interactor;
                        return _observingTarget;
                    }
                }

                return _observingTarget;
            }
        }

        private PlayerInteractor _observingTarget;
        private Button _jump;
        private Button _grab;
        private Button _ungrab;
        private Button _throw;
        private Button _use;
        private List<RaycastResult> _raycastBuffer = new List<RaycastResult>();

        protected override void Awake()
        {
            base.Awake();
            StartCoroutine(C_Init());
        }

        IEnumerator C_Init()
        {
            yield return new WaitUntil(() => observingTarget != null);

            _jump = transform.Find("Button - Jump").GetComponent<Button>();
            _grab = transform.Find("Button - Grab").GetComponent<Button>();
            _ungrab = transform.Find("Button - Ungrab").GetComponent<Button>();
            _throw = transform.Find("Button - Throw").GetComponent<Button>();
            _use = transform.Find("Button - Use").GetComponent<Button>();

            observingTarget.onFindInteractable += interactable =>
            {
                if (interactable)
                {
                    _grab.gameObject.SetActive((interactable.interactions & Interactions.Grab) > 0 && observingTarget.interactable == false);
                }
                else
                {
                    _grab.gameObject.SetActive(false);
                }
            };

            observingTarget.onInteractableChanged += interactable =>
            {
                if (interactable)
                {
                    _ungrab.gameObject.SetActive((interactable.interactions & Interactions.Ungrab) > 0);
                    _throw.gameObject.SetActive((interactable.interactions & Interactions.Throw) > 0);
                    _use.gameObject.SetActive((interactable.interactions & Interactions.Use) > 0);
                }
                else
                {
                    _ungrab.gameObject.SetActive(false);
                }
            };

            _grab.onClick.AddListener(() => (observingTarget.foundInteractable as IOneHandGrabbable)?.Grab());

            _ungrab.onClick.AddListener(() => (observingTarget.interactable as IOneHandGrabbable)?.Ungrab());

            _throw.onClick.AddListener(() => (observingTarget.interactable as IOneHandThrowable)?.Throw());

            _use.onClick.AddListener(() => (observingTarget.interactable as IUsable)?.Use());


            _jump.onClick.AddListener(() =>
            {
                if (ClientCharacterController.spawned.TryGetValue(PhotonNetwork.LocalPlayer.ActorNumber,
                                                                   out ClientCharacterController controller))
                {
                    controller.ChangeState(State.Jump);
                }
            });

            Show();
        }

        public override void InputAction()
        {
            base.InputAction();

            if (Input.GetMouseButtonDown(0))
            {
                _raycastBuffer.Clear();

                // 유저가 월드를 클릭했을때 플레이어를 해당 위치로 이동시킴
                if (Raycast(_raycastBuffer) == false)
                {
                    if (ClientCharacterController.spawned.TryGetValue(PhotonNetwork.LocalPlayer.ActorNumber,
                                                                       out ClientCharacterController controller))
                    {
                        controller.MoveTo(Input.mousePosition);
                    }
                }
            }
        }
    }
}