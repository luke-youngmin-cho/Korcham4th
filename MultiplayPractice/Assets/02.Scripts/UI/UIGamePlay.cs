using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MP.UI
{
    public class UIGamePlay : UIScreenBase, IInRoomCallbacks
    {
        private Button _leaveRoom;


        protected override void Awake()
        {
            base.Awake();

            _leaveRoom = transform.Find("Button - LeaveRoom").GetComponent<Button>();
            _leaveRoom.onClick.AddListener(() =>
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    foreach (var player in PhotonNetwork.PlayerList)
                    {
                        if (player.IsMasterClient == false)
                        {
                            if (PhotonNetwork.SetMasterClient(player))
                                break;
                        }
                    }
                }

                PhotonNetwork.LeaveRoom();
                SceneManager.LoadScene("Lobby");
            });
        }

        private void OnEnable()
        {
            PhotonNetwork.AddCallbackTarget(this);
        }

        private void OnDisable()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        public void OnMasterClientSwitched(Player newMasterClient)
        {
        }

        public void OnPlayerEnteredRoom(Player newPlayer)
        {
        }

        public void OnPlayerLeftRoom(Player otherPlayer)
        {
        }

        public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
        }

        public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
        }
    }
}
