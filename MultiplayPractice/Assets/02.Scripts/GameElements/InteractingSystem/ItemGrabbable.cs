using UnityEngine;
using Photon.Pun;
using PhotonRigidbodyView = MP.Network.PhotonRigidbodyView;

namespace MP.GameElements.InteractingSystem
{
    [RequireComponent(typeof(PhotonRigidbodyView))]
    public abstract class ItemGrabbable : Item, IOneHandGrabbable
    {
        public override Interactions interactions => base.interactions | Interactions.Grab | Interactions.Ungrab;
        private PhotonRigidbodyView _rigidbodyView;


        protected override void Awake()
        {
            base.Awake();
            _rigidbodyView = GetComponent<PhotonRigidbodyView>();
        }

        public void Grab()
        {
            view.RPC("GrabClientRpc", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber);
        }

        public void Ungrab()
        {
            view.RPC("UngrabClientRpc", RpcTarget.All);
        }

        [PunRPC]
        protected virtual void GrabClientRpc(int clientID)
        {
            if (ClientCharacterController.spawned.TryGetValue(clientID,
                                                              out ClientCharacterController controller) &&
                controller.TryGetComponent(out IOneHandGrabber grabber))
            {
                _rigidbodyView.enabled = false;
                rigidbody.velocity = Vector3.zero;
                rigidbody.isKinematic = true;
                transform.SetParent(grabber.hand);
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;
                BeginInteraction(clientID);
            }
        }

        [PunRPC]
        protected virtual void UngrabClientRpc()
        {
            if (ClientCharacterController.spawned.TryGetValue(interactingClientID,
                                                              out ClientCharacterController controller))
            {
                _rigidbodyView.enabled = true;
                rigidbody.isKinematic = false;
                transform.SetParent(null);
                EndInteraction();
            }
        }
    }
}
