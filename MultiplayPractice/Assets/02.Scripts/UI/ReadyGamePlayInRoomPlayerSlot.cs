using UnityEngine;
using TMPro;

namespace MP.UI
{
    public class ReadyGamePlayInRoomPlayerSlot : MonoBehaviour
    {
        public bool isReady
        {
            set
            {
                _isReady.enabled = value;
            }
        }

        public string nickname
        {
            get => _nickname.text;
            set
            {
                _nickname.text = value;
            }
        }

        private TMP_Text _isReady;
        private TMP_Text _nickname;


        private void Awake()
        {
            _isReady = transform.Find("Text (TMP) - IsReady").GetComponent<TMP_Text>();
            _nickname = transform.Find("Text (TMP) - Nickname").GetComponent<TMP_Text>();
        }
    }
}