using Photon.Pun;
using System;
using UnityEngine;

namespace MP.GameElements.InteractingSystem
{
    [RequireComponent(typeof(PhotonView))]
    public abstract class Interactor : MonoBehaviour
    {
        public Interactable interactable
        {
            get => _interactable;
            set
            {
                _interactable = value;
                onInteractableChanged?.Invoke(value);
            }
        }
        public Interactable foundInteractable
        {
            get => _foundInteractable;
            set
            {
                _foundInteractable = value;
            }
        }

        private Interactable _interactable;
        private Interactable _foundInteractable;
        protected LayerMask interactableMask;
        protected PhotonView view;
        
        public Action<Interactable> onFindInteractable;
        public Action<Interactable> onBeginInteraction;
        public Action<Interactable> onEndInteraction;
        public Action<Interactable> onInteractableChanged;


        protected virtual void Awake()
        {
            view = GetComponent<PhotonView>();
        }

        protected virtual void Update()
        {
            if (!view.IsMine)
                return;

            FindInteractable();
        }

        protected virtual void FindInteractable()
        {
            Collider[] cols = Physics.OverlapSphere(transform.position, 0.5f, interactableMask);
            foundInteractable = cols.Length > 0 ? cols[0].GetComponent<Interactable>() : null;
            onFindInteractable?.Invoke(foundInteractable);
        }
    }
}
