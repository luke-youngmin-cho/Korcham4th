using Photon.Pun;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace MP.Network
{
    public class PhotonManager : MonoBehaviourPunCallbacks
    {
        #region Singleton
        public static PhotonManager instance
        {
            get
            {
                if (s_instance == null)
                {
                    s_instance = new GameObject(typeof(PhotonManager).Name).AddComponent<PhotonManager>();
                    DontDestroyOnLoad(s_instance.gameObject);
                }
                return s_instance;
            }
        }
        private static PhotonManager s_instance;
        #endregion

        private void Awake()
        {
            if (PhotonNetwork.IsConnected == false)
            {
                bool isConnected = PhotonNetwork.ConnectUsingSettings();
                Debug.Assert(isConnected, "[PhotonManager] : Failed to connect to photon server.");
            }
        }

        public override void OnConnectedToMaster()
        {
            base.OnConnected();
            PhotonNetwork.AutomaticallySyncScene = true; // PhotonNetwork.LoadLevel() 호출시 현재 동일한 방에있는 모든 클라이언트의 씬을 동기화 하는 옵션
        }

        public override void OnJoinedLobby()
        {
            base.OnJoinedLobby();
            Debug.Log($"[PhotonManager] : Joined lobby.");
        }

        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();

            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable
            {
                { "isReady" , false },
            });
        }

        public override void OnLeftRoom()
        {
            base.OnLeftRoom();

            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable
            {
            });
        }
    }
}