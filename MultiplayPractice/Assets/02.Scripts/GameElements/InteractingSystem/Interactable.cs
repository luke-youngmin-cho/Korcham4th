using MP.Network;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using ExitGames.Client.Photon;

namespace MP.GameElements.InteractingSystem
{
    [RequireComponent(typeof(PhotonView))]
    public abstract class Interactable : MonoBehaviourPunBase, IPunObservable, IOnEventCallback
    {
        public virtual Interactions interactions => Interactions.None;
        public const int NOBODY = -1;
        public int interactingClientID { get; private set; } = NOBODY;


        protected override void Awake()
        {
            base.Awake();
            SyncViewID();
        }

        protected virtual void OnDestroy()
        {
            EndInteraction();
        }

        private void SyncViewID()
        {
            if (PhotonNetwork.IsMasterClient == false)
                return;

            if (view.Owner != null)
                return;

            if (PhotonNetwork.AllocateViewID(view))
            {
                object raiseEventContent = new object[]
                {
                        view.ViewID,
                };
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions
                {
                    Receivers = ReceiverGroup.Others,
                };

                PhotonNetwork.RaiseEvent(PhotonEventCode.SYNC_VIEW_ID,
                                         raiseEventContent,
                                         raiseEventOptions,
                                         SendOptions.SendReliable);
            }
            else
            {
                throw new System.Exception("[Interactable] : Failed to allocate view ID");
            }
        }

        private void HandleSyncViewID(EventData photonEvent)
        {
            object[] data = (object[])photonEvent.CustomData;
            int viewID = (int)data[0];
            view.ViewID = viewID;
        }

        public void OnEvent(EventData photonEvent)
        {
            byte eventCode = photonEvent.Code;

            if (eventCode == PhotonEventCode.SYNC_VIEW_ID)
                HandleSyncViewID(photonEvent);
        }


        public virtual void BeginInteraction(int clientID)
        {
            if (clientID != NOBODY &&
                ClientCharacterController.spawned.TryGetValue(clientID, out ClientCharacterController controller))
            {
                controller.interactor.interactable = this;
                interactingClientID = clientID;
            }
        }

        public virtual void DuringInteraction()
        {

        }

        public virtual void EndInteraction()
        {
            if (interactingClientID != NOBODY &&
                ClientCharacterController.spawned.TryGetValue(interactingClientID, out ClientCharacterController controller))
            {
                controller.interactor.interactable = null;
                interactingClientID = NOBODY;
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(interactingClientID);
            }
            else
            {
                interactingClientID = (int)stream.ReceiveNext();
            }
        }

    }
}