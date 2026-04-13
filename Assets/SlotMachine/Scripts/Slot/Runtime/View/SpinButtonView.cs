using SlotMachine.MessagePipe.Pipes;
using SlotMachine.Slot.Messages;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace SlotMachine.Slot.View
{
    public class SpinButtonView : MonoBehaviour
    {
        [SerializeField] private Button m_Button;

        [Inject] private GamePipe m_Pipe;

        private void Start()
        {
            m_Button.onClick.AddListener(OnSpinButtonClicked);
            m_Pipe.SubscribeTo<SpinCompletedMessage>(OnSpinCompleted);
        }

        private void OnDestroy()
        {
            m_Button.onClick.RemoveListener(OnSpinButtonClicked);

            if (m_Pipe != null)
                m_Pipe.UnsubscribeFrom<SpinCompletedMessage>(OnSpinCompleted);
        }

        private void OnSpinButtonClicked()
        {
            m_Button.interactable = false;
            SpinRequestedMessage message = new();
            m_Pipe.Raise(in message);
        }

        private void OnSpinCompleted(ref SpinCompletedMessage _) => m_Button.interactable = true;
    }
}
