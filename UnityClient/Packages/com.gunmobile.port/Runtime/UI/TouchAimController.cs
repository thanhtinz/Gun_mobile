using UnityEngine;
using UnityEngine.EventSystems;

namespace GunMobile.UI
{
    /// <summary>
    /// Replaces PC keyboard angle/power with a right-hand drag pad.
    /// Drag around the pad: angle = atan2, power = distance clamped to 100.
    /// </summary>
    public sealed class TouchAimController : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [SerializeField] float _padRadius = 140f;
        [SerializeField] int _facing = 1;

        public float AngleDeg { get; private set; } = 45f;
        public float Power { get; private set; } = 50f;
        public bool Aiming { get; private set; }
        public bool FireReleased { get; private set; }

        RectTransform _rt;

        void Awake()
        {
            _rt = transform as RectTransform;
            if (_rt != null)
            {
                Vector2 size = MobileUiBootstrap.FingerButtonSize * 3.2f;
                _rt.sizeDelta = new Vector2(Mathf.Max(_rt.sizeDelta.x, size.x), Mathf.Max(_rt.sizeDelta.y, size.y));
                _padRadius = _rt.sizeDelta.x * 0.45f;
            }
        }

        public void SetFacing(int facing)
        {
            _facing = facing >= 0 ? 1 : -1;
        }

        public void ConsumeFire()
        {
            FireReleased = false;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Aiming = true;
            FireReleased = false;
            UpdateAim(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (Aiming)
            {
                UpdateAim(eventData);
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            UpdateAim(eventData);
            Aiming = false;
            FireReleased = true;
        }

        void UpdateAim(PointerEventData eventData)
        {
            if (_rt == null)
            {
                return;
            }

            Vector2 local;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_rt, eventData.position, eventData.pressEventCamera, out local);
            Vector2 delta = local - _rt.rect.center;
            if (delta.sqrMagnitude < 1f)
            {
                return;
            }

            float mag = Mathf.Min(delta.magnitude, _padRadius);
            Power = Mathf.Clamp(mag / _padRadius * 100f, 1f, 100f);
            float rad = Mathf.Atan2(delta.y, delta.x * _facing);
            AngleDeg = Mathf.Clamp(rad * Mathf.Rad2Deg, 0f, 90f);
        }
    }

    /// <summary>
    /// Left-thumb walk. PC used keyboard; mobile needs a holdable strip.
    /// </summary>
    public sealed class TouchMoveController : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        public int Direction { get; private set; }

        RectTransform _rt;

        void Awake() => _rt = transform as RectTransform;

        public void OnPointerDown(PointerEventData eventData) => UpdateDir(eventData);
        public void OnDrag(PointerEventData eventData) => UpdateDir(eventData);
        public void OnPointerUp(PointerEventData eventData) => Direction = 0;

        void UpdateDir(PointerEventData eventData)
        {
            Vector2 local;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_rt, eventData.position, eventData.pressEventCamera, out local);
            float x = local.x - _rt.rect.center.x;
            Direction = x > 12f ? 1 : x < -12f ? -1 : 0;
        }
    }
}
