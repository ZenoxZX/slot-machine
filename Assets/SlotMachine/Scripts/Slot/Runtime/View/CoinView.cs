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
        private Vector2 m_BoundsHalf;
        private float m_Elapsed;
        private float m_Duration;
        private float m_FrameTimer;
        private int m_FrameIndex;
        private float m_RotationSpeed;
        private float m_FadeTimer;
        private bool m_IsFading;
        private bool m_Active;

        public bool IsActive => m_Active;

        public void Initialize(CoinVFXData data)
        {
            m_Data = data;
            m_RectTransform = (RectTransform)transform;
        }

        public void Launch(Vector2 startPosition, float angle, float speed, float duration, Vector2 boundsHalf)
        {
            float rad = angle * Mathf.Deg2Rad;
            m_Velocity = new Vector2(Mathf.Cos(rad) * speed, Mathf.Sin(rad) * speed);
            m_BoundsHalf = boundsHalf;
            m_Elapsed = 0f;
            m_Duration = duration;
            m_FrameTimer = 0f;
            m_FrameIndex = 0;
            m_RotationSpeed = Random.Range(m_Data.RotationSpeedMin, m_Data.RotationSpeedMax);
            m_FadeTimer = 0f;
            m_IsFading = false;
            m_Active = true;

            m_RectTransform.anchoredPosition = startPosition;
            m_RectTransform.localScale = Vector3.one * m_Data.StartScale;
            m_RectTransform.localEulerAngles = Vector3.zero;
            m_Image.enabled = true;
            m_Image.color = Color.white;

            if (m_Data.CoinFrames.Length > 0)
                m_Image.sprite = m_Data.CoinFrames[0];
        }

        public void Tick(float deltaTime)
        {
            if (!m_Active)
                return;

            m_Elapsed += deltaTime;

            // Fade out phase
            if (m_IsFading)
            {
                m_FadeTimer += deltaTime;
                float fadeT = Mathf.Clamp01(m_FadeTimer / m_Data.FadeOutDuration);
                m_Image.color = new Color(1f, 1f, 1f, 1f - fadeT);

                if (fadeT >= 1f)
                {
                    Deactivate();
                    return;
                }
            }
            else if (m_Elapsed >= m_Duration)
            {
                m_IsFading = true;
                m_FadeTimer = 0f;
            }

            float t = m_Elapsed / m_Duration;

            // Velocity integration (gravity on Y)
            m_Velocity.y -= m_Data.Gravity * deltaTime;

            Vector2 pos = m_RectTransform.anchoredPosition;
            pos.x += m_Velocity.x * deltaTime;
            pos.y += m_Velocity.y * deltaTime;

            // Bounce off screen bounds
            if (pos.x > m_BoundsHalf.x || pos.x < -m_BoundsHalf.x)
            {
                m_Velocity.x = -m_Velocity.x;
                pos.x = Mathf.Clamp(pos.x, -m_BoundsHalf.x, m_BoundsHalf.x);
            }

            if (pos.y > m_BoundsHalf.y || pos.y < -m_BoundsHalf.y)
            {
                m_Velocity.y = -m_Velocity.y;
                pos.y = Mathf.Clamp(pos.y, -m_BoundsHalf.y, m_BoundsHalf.y);
            }

            m_RectTransform.anchoredPosition = pos;

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
