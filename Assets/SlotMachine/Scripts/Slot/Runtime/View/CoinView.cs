using SlotMachine.Slot.Data;
using UnityEngine;
using UnityEngine.UI;

namespace SlotMachine.Slot.View
{
    public class CoinView : MonoBehaviour
    {
        [SerializeField] private Image m_Image;

        private RectTransform m_RectTransform;
        private CoinVFXData m_Data;

        private Vector2 m_Velocity;
        private float m_Elapsed;
        private float m_Duration;
        private float m_FrameTimer;
        private int m_FrameIndex;
        private float m_RotationSpeed;
        private bool m_Active;

        public bool IsActive => m_Active;

        public void Initialize(CoinVFXData data)
        {
            m_Data = data;
            m_RectTransform = (RectTransform)transform;
        }

        public void Launch(Vector2 startPosition, float angle, float speed, float duration)
        {
            float rad = angle * Mathf.Deg2Rad;
            m_Velocity = new Vector2(Mathf.Cos(rad) * speed, Mathf.Sin(rad) * speed);
            m_Elapsed = 0f;
            m_Duration = duration;
            m_FrameTimer = 0f;
            m_FrameIndex = 0;
            m_RotationSpeed = Random.Range(m_Data.RotationSpeedMin, m_Data.RotationSpeedMax);
            m_Active = true;

            m_RectTransform.anchoredPosition = startPosition;
            m_RectTransform.localScale = Vector3.one * m_Data.StartScale;
            m_RectTransform.localEulerAngles = Vector3.zero;
            m_Image.enabled = true;

            if (m_Data.CoinFrames.Length > 0)
                m_Image.sprite = m_Data.CoinFrames[0];
        }

        public void Tick(float deltaTime)
        {
            if (!m_Active)
                return;

            m_Elapsed += deltaTime;

            if (m_Elapsed >= m_Duration)
            {
                Deactivate();
                return;
            }

            float t = m_Elapsed / m_Duration;

            // Parabolic trajectory
            float x = m_Velocity.x * m_Elapsed;
            float y = m_Velocity.y * m_Elapsed - 0.5f * m_Data.Gravity * m_Elapsed * m_Elapsed;
            Vector2 startPos = m_RectTransform.anchoredPosition;
            m_RectTransform.anchoredPosition = new Vector2(
                m_RectTransform.anchoredPosition.x + m_Velocity.x * deltaTime,
                m_RectTransform.anchoredPosition.y + (m_Velocity.y - m_Data.Gravity * m_Elapsed) * deltaTime);

            // Perspective scale
            float scale = Mathf.Lerp(m_Data.StartScale, m_Data.EndScale, t);
            m_RectTransform.localScale = Vector3.one * scale;

            // Z rotation
            Vector3 euler = m_RectTransform.localEulerAngles;
            euler.z += m_RotationSpeed * deltaTime;
            m_RectTransform.localEulerAngles = euler;

            // Sprite sheet animation
            if (m_Data.CoinFrames.Length > 1)
            {
                m_FrameTimer += deltaTime;
                float frameDuration = 1f / m_Data.FrameRate;

                if (m_FrameTimer >= frameDuration)
                {
                    m_FrameTimer -= frameDuration;
                    m_FrameIndex = (m_FrameIndex + 1) % m_Data.CoinFrames.Length;
                    m_Image.sprite = m_Data.CoinFrames[m_FrameIndex];
                }
            }
        }

        public void Deactivate()
        {
            m_Active = false;
            m_Image.enabled = false;
        }
    }
}
