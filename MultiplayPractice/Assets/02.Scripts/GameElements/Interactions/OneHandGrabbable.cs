using UnityEngine;
using Photon.Pun;

namespace MP.GameElements.Interactions
{
    public class OneHandGrabbable : Interactable, IOneHandGrabbable
    {
        protected Rigidbody rigidbody;
        protected Collider collider;

        protected override void Awake()
        {
            base.Awake();
            rigidbody = GetComponent<Rigidbody>();
            collider = GetComponent<Collider>();
        }

        public override bool BeginInteraction(int clientID)
        {
            if (base.BeginInteraction(clientID) == false)
                return false;

            if (ClientCharacterController._spawned.ContainsKey(clientID) == false)
                throw new System.Exception($"[OneHandGrabbable] : Failed to BeginInteraction. wrong clientID {clientID}");

            view.RPC("GrabClientRpc", RpcTarget.All, new object[] { clientID });
            return true;
        }

        public override void EndInteraction()
        {
            base.EndInteraction();

            view.RPC("UngrabClientRpc", RpcTarget.All, null);
        }

        [PunRPC]
        public void GrabClientRpc(int clientID)
        {
            interactingClientID = clientID;
            Debug.Log($"GrabClientRpc... {clientID}");
            if (ClientCharacterController._spawned.TryGetValue(clientID, out ClientCharacterController controller))
            {
                if (controller.TryGetComponent(out IOneHandGrabber oneHandGrabber))
                {
                    Grab(oneHandGrabber);
                }
            }
        }

        [PunRPC]
        public void UngrabClientRpc()
        {
            Debug.Log($"UnGrabClientRpc...");
            Ungrab();
            interactingClientID = NOBODY;
        }

        public void Grab(IOneHandGrabber grabber)
        {
            Debug.Log($"Grabbed by {interactingClientID}");
            rigidbody.isKinematic = true;
            collider.isTrigger = true;
            transform.SetParent(grabber.hand);
            transform.localPosition = Vector3.zero;
        }

        public void Ungrab()
        {
            rigidbody.isKinematic = false;
            collider.isTrigger = false;
            transform.SetParent(null);
        }
    }
}
