using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Photon.Pun;

namespace MP.Network
{
    [RequireComponent(typeof(PhotonView))]
    public abstract class MonoBehaviourPunBase : MonoBehaviour
    {
        protected PhotonView view;

        protected virtual void Awake()
        {
            view = GetComponent<PhotonView>();
        }

        public void DestroySelf()
        {
            view.RPC("DestroyClientRpc", RpcTarget.All, view.OwnerActorNr);
        }

        [PunRPC]
        protected virtual void DestroyClientRpc(int ownerClientID)
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber == ownerClientID)
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }
}
