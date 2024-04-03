using Photon.Pun;
using UnityEngine;

namespace MP.GameElements.Interactions
{
    [RequireComponent(typeof(PhotonView))]
    public abstract class Interactor : MonoBehaviour
    {
        protected LayerMask interactableMask;
        protected PhotonView view;
        protected Interactable current;

        protected virtual void Awake()
        {
            view = GetComponent<PhotonView>();
        }

        public bool TryInteraction()
        {
            if (current == null)
            {
                if (TryCastInteractable(out Interactable interactable))
                {
                    // 현재 상호작용을 아무도 안하고있다면 상호작용 시작
                    if (interactable.interactingClientID == Interactable.NOBODY)
                    {
                        if (interactable.BeginInteraction(view.OwnerActorNr))
                        {
                            current = interactable;
                        }
                    }
                }
            }
            else
            {
                current.EndInteraction();
                current = null;
            }

            return false;
        }

        protected virtual bool TryCastInteractable(out Interactable interactable)
        {
            Collider[] cols = Physics.OverlapSphere(transform.position, 0.5f, interactableMask);
            if (cols.Length > 0)
            {
                interactable = cols[0].GetComponent<Interactable>();
                return true;
            }

            interactable = null;
            return false;
        }
    }
}
