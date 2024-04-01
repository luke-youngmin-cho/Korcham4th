using MP.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MP.UI
{
    public class UILobby : UIScreenBase, ILobbyCallbacks, IMatchmakingCallbacks
    {
        public const int NOT_SELECTED = -1;

        public int roomListSlotIndexSelected
        {
            get => _roomListSlotIndexSelected;
            set
            {
                _roomListSlotIndexSelected = value;
                _joinRoom.interactable = value > NOT_SELECTED;
            }
        }

        // Main panel
        private int _roomListSlotIndexSelected = NOT_SELECTED;
        private RoomListSlot _roomListSlot;
        private List<RoomListSlot> _roomListSlots = new List<RoomListSlot>(20);
        private RectTransform _roomListContent;
        private Button _joinRoom;
        private Button _createRoom;
        private List<RoomInfo> _localRoomList;

        // RoomOption panel
        private GameObject _roomOptionPanel;
        private TMP_InputField _roomName;
        private Scrollbar _maxPlayer;
        private TMP_Text _maxPlayerValue;
        private Button _confirmRoomOptions;
        private Button _cancelRoomOptions;

        protected override void Awake()
        {
            base.Awake();
            _roomListSlot = Resources.Load<RoomListSlot>("UI/RoomListSlot");
            _roomListContent = transform.Find("Panel/Scroll View - RoomList/Viewport/Content").GetComponent<RectTransform>();
            _joinRoom = transform.Find("Panel/Button - JoinRoom").GetComponent<Button>();
            _createRoom = transform.Find("Panel/Button - CreateRoom").GetComponent<Button>();
            _joinRoom.interactable = false;
            _joinRoom.onClick.AddListener(() =>
            {
                if (PhotonNetwork.JoinRoom(_localRoomList[_roomListSlotIndexSelected].Name))
                {
                    UIManager.instance.Get<UILoadingPanel>()
                                      .Show();
                }
                else
                {
                    UIManager.instance.Get<UIWarningWindow>()
                                      .Show("The room is invalid");    
                }
            });
            _createRoom.onClick.AddListener(() =>
            {
                _roomName.text = string.Empty;
                _maxPlayer.value = 0f;
                _roomOptionPanel.gameObject.SetActive(true);
            });

            _roomOptionPanel = transform.Find("Panel - RoomOptions").gameObject;
            _roomName = transform.Find("Panel - RoomOptions/Panel/InputField (TMP) - RoomName").GetComponent<TMP_InputField>();
            _maxPlayer = transform.Find("Panel - RoomOptions/Panel/Scrollbar - MaxPlayer").GetComponent<Scrollbar>();
            _maxPlayerValue = transform.Find("Panel - RoomOptions/Panel/Text (TMP) - MaxPlayerValue").GetComponent<TMP_Text>();
            _confirmRoomOptions = transform.Find("Panel - RoomOptions/Panel/Button - ConfirmRoomOptions").GetComponent<Button>();
            _cancelRoomOptions = transform.Find("Panel - RoomOptions/Panel/Button - CancelRoomOptions").GetComponent<Button>();

            _roomName.onValueChanged.AddListener(value => _confirmRoomOptions.interactable = value.Length > 1); // 방제목 두글자 이상일때만 확인버튼 누를수있음.
            _maxPlayer.onValueChanged.AddListener(value =>
            {
                _maxPlayerValue.text = Mathf.RoundToInt(value * _maxPlayer.numberOfSteps + 1).ToString();
            });
            _cancelRoomOptions.onClick.AddListener(() => _roomOptionPanel.gameObject.SetActive(false));
            _confirmRoomOptions.interactable = false;
            _confirmRoomOptions.onClick.AddListener(() =>
            {
                if (PhotonNetwork.CreateRoom(_roomName.text,
                                             new RoomOptions
                                             {
                                                 CustomRoomProperties = new ExitGames.Client.Photon.Hashtable
                                                 {
                                                     {"levelLimit", 10 },
                                                 },
                                                 MaxPlayers = Mathf.RoundToInt(_maxPlayer.value * _maxPlayer.numberOfSteps + 1),
                                                 PublishUserId = true,
                                             }))
                {
                    UIManager.instance.Get<UILoadingPanel>()
                                      .Show();
                }
            });
        }

        private void OnEnable()
        {
            PhotonNetwork.AddCallbackTarget(this);
            StartCoroutine(C_JoinLobby());
        }

        private void OnDisable()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        IEnumerator C_JoinLobby()
        {
            Debug.Log($"[UILobby] : Wait until conntected to server.");
            yield return new WaitUntil(() => PhotonNetwork.NetworkClientState == ClientState.ConnectedToMasterServer);
            PhotonNetwork.JoinLobby();
        }

        public void OnJoinedLobby()
        {
            Debug.Log("[UILobby] : Joined lobby.");
            UIManager.instance.Get<UILoadingPanel>()
                              .Hide();
        }

        public void OnLeftLobby()
        {
        }

        public void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            _localRoomList = roomList;

            for (int i = 0; i < _roomListSlots.Count; i++)
                Destroy(_roomListSlots[i].gameObject);

            _roomListSlots.Clear();

            for (int i = 0; i < roomList.Count; i++)
            {
                RoomListSlot slot = Instantiate(_roomListSlot, _roomListContent);
                slot.roomIndex = i;
                slot.Refresh(roomList[i].Name, roomList[i].PlayerCount, roomList[i].MaxPlayers);
                slot.onSelected += (index) => roomListSlotIndexSelected = index;
                _roomListSlots.Add(slot);
            }
        }

        public void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
        {
            throw new System.NotImplementedException();
        }

        public void OnFriendListUpdate(List<FriendInfo> friendList)
        {
            throw new System.NotImplementedException();
        }

        public void OnCreatedRoom()
        {
            UIManager.instance.Get<UILoadingPanel>()
                              .Hide();
        }

        public void OnCreateRoomFailed(short returnCode, string message)
        {
            UIManager.instance.Get<UILoadingPanel>()
                              .Hide();
        }

        public void OnJoinedRoom()
        {
            UIManager.instance.Get<UILoadingPanel>()
                              .Hide();
        }

        public void OnJoinRoomFailed(short returnCode, string message)
        {
            UIManager.instance.Get<UILoadingPanel>()
                              .Hide();
        }

        public void OnJoinRandomFailed(short returnCode, string message)
        {
            UIManager.instance.Get<UILoadingPanel>()
                              .Hide();
        }

        public void OnLeftRoom()
        {
            UIManager.instance.Get<UILoadingPanel>()
                              .Hide();
        }
    }
}