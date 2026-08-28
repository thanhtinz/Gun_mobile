// Stub UnityEngine surface, only for compile-checking the client scripts outside Unity.
using System;
using System.Collections;
using System.Collections.Generic;

namespace UnityEngine
{
    public struct Vector2
    {
        public float x, y;
        public Vector2(float x, float y) { this.x = x; this.y = y; }
        public static Vector2 zero => new Vector2(0, 0);
        public static Vector2 one => new Vector2(1, 1);
        public float magnitude => 0f;
        public float sqrMagnitude => x * x + y * y;
        public Vector2 normalized => this;
        public static float Distance(Vector2 a, Vector2 b) => 0f;
        public static Vector2 Lerp(Vector2 a, Vector2 b, float t) => a;
        public static Vector2 operator +(Vector2 a, Vector2 b) => new Vector2(a.x + b.x, a.y + b.y);
        public static Vector2 operator -(Vector2 a, Vector2 b) => new Vector2(a.x - b.x, a.y - b.y);
        public static Vector2 operator *(Vector2 a, float b) => new Vector2(a.x * b, a.y * b);
        public static Vector2 operator /(Vector2 a, float b) => new Vector2(a.x / b, a.y / b);
    }

    public struct Vector3
    {
        public float x, y, z;
        public Vector3(float x, float y) { this.x = x; this.y = y; this.z = 0; }
        public Vector3(float x, float y, float z) { this.x = x; this.y = y; this.z = z; }
        public static Vector3 zero => new Vector3(0, 0, 0);
        public static Vector3 one => new Vector3(1, 1, 1);
        public static implicit operator Vector2(Vector3 v) => new Vector2(v.x, v.y);
        public static implicit operator Vector3(Vector2 v) => new Vector3(v.x, v.y, 0);
        public static Vector3 operator +(Vector3 a, Vector3 b) => new Vector3(a.x + b.x, a.y + b.y, a.z + b.z);
        public static Vector3 operator -(Vector3 a, Vector3 b) => new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
        public static Vector3 operator *(Vector3 a, float b) => new Vector3(a.x * b, a.y * b, a.z * b);
    }

    public struct Vector4 { public float x, y, z, w; public Vector4(float x, float y, float z, float w) { this.x = x; this.y = y; this.z = z; this.w = w; } }

    public struct Color
    {
        public float r, g, b, a;
        public Color(float r, float g, float b) { this.r = r; this.g = g; this.b = b; this.a = 1f; }
        public Color(float r, float g, float b, float a) { this.r = r; this.g = g; this.b = b; this.a = a; }
        public static Color white => new Color(1, 1, 1);
        public static Color black => new Color(0, 0, 0);
        public static Color clear => new Color(0, 0, 0, 0);
        public static Color red => new Color(1, 0, 0);
        public static Color green => new Color(0, 1, 0);
        public static Color blue => new Color(0, 0, 1);
        public static Color yellow => new Color(1, 1, 0);
        public static Color gray => new Color(.5f, .5f, .5f);
        public static Color cyan => new Color(0, 1, 1);
        public static Color magenta => new Color(1, 0, 1);
        public static Color operator *(Color a, float b) => new Color(a.r * b, a.g * b, a.b * b, a.a * b);
        public static Color Lerp(Color a, Color b, float t) => a;
    }

    public struct Color32
    {
        public byte r, g, b, a;
        public Color32(byte r, byte g, byte b, byte a) { this.r = r; this.g = g; this.b = b; this.a = a; }
        public static implicit operator Color(Color32 c) => new Color(c.r / 255f, c.g / 255f, c.b / 255f, c.a / 255f);
        public static implicit operator Color32(Color c) => new Color32(0, 0, 0, 255);
    }

    public struct Rect
    {
        public float x, y, width, height;
        public Rect(float x, float y, float width, float height) { this.x = x; this.y = y; this.width = width; this.height = height; }
        public float xMin => x; public float yMin => y; public float xMax => x + width; public float yMax => y + height;
        public Vector2 size => new Vector2(width, height);
        public Vector2 center => new Vector2(x + width / 2, y + height / 2);
        public static bool operator ==(Rect a, Rect b) => a.x == b.x && a.y == b.y && a.width == b.width && a.height == b.height;
        public static bool operator !=(Rect a, Rect b) => !(a == b);
        public override bool Equals(object o) => o is Rect r && this == r;
        public override int GetHashCode() => 0;
    }

    public struct Quaternion { public static Quaternion identity => new Quaternion(); public static Quaternion Euler(float x, float y, float z) => new Quaternion(); }

    public static class Mathf
    {
        public const float PI = 3.1415927f;
        public const float Infinity = float.PositiveInfinity;
        public const float Deg2Rad = 0.0174533f;
        public const float Rad2Deg = 57.29578f;
        public static float Sqrt(float f) => (float)Math.Sqrt(f);
        public static float Abs(float f) => Math.Abs(f);
        public static int Abs(int f) => Math.Abs(f);
        public static float Sin(float f) => (float)Math.Sin(f);
        public static float Cos(float f) => (float)Math.Cos(f);
        public static float Atan2(float y, float x) => (float)Math.Atan2(y, x);
        public static float Pow(float a, float b) => (float)Math.Pow(a, b);
        public static float Min(float a, float b) => Math.Min(a, b);
        public static int Min(int a, int b) => Math.Min(a, b);
        public static float Max(float a, float b) => Math.Max(a, b);
        public static int Max(int a, int b) => Math.Max(a, b);
        public static int Max(params int[] values) => 0;
        public static float Max(params float[] values) => 0f;
        public static int Min(params int[] values) => 0;
        public static float Min(params float[] values) => 0f;
        public static float Clamp(float v, float a, float b) => Math.Min(Math.Max(v, a), b);
        public static int Clamp(int v, int a, int b) => Math.Min(Math.Max(v, a), b);
        public static float Clamp01(float v) => Clamp(v, 0f, 1f);
        public static float Lerp(float a, float b, float t) => a + (b - a) * t;
        public static int RoundToInt(float f) => (int)Math.Round(f);
        public static int FloorToInt(float f) => (int)Math.Floor(f);
        public static int CeilToInt(float f) => (int)Math.Ceiling(f);
        public static float Floor(float f) => (float)Math.Floor(f);
        public static float Repeat(float t, float length) => t;
        public static float PingPong(float t, float length) => t;
        public static float MoveTowards(float a, float b, float d) => b;
        public static float Sign(float f) => f < 0 ? -1f : 1f;
    }

    public class Object
    {
        public string name { get; set; }
        public HideFlags hideFlags { get; set; }
        public static void Destroy(Object o) { }
        public static void DestroyImmediate(Object o) { }
        public static void DontDestroyOnLoad(Object o) { }
        public static T Instantiate<T>(T o) where T : Object => o;
        public static T Instantiate<T>(T o, Transform parent) where T : Object => o;
        public static T Instantiate<T>(T o, Transform parent, bool worldPositionStays) where T : Object => o;
        public static bool operator ==(Object a, Object b) => ReferenceEquals(a, b);
        public static bool operator !=(Object a, Object b) => !ReferenceEquals(a, b);
        public override bool Equals(object other) => ReferenceEquals(this, other);
        public override int GetHashCode() => 0;
        public static implicit operator bool(Object o) => !ReferenceEquals(o, null);
    }

    public enum HideFlags { None = 0, HideAndDontSave = 1 }
    public enum TextAnchor { UpperLeft, UpperCenter, UpperRight, MiddleLeft, MiddleCenter, MiddleRight, LowerLeft, LowerCenter, LowerRight }
    public enum FontStyle { Normal, Bold, Italic, BoldAndItalic }
    public enum ScreenOrientation { Portrait, LandscapeLeft, LandscapeRight, AutoRotation }
    public enum RuntimePlatform { Android, IPhonePlayer, WindowsEditor, WindowsPlayer, OSXEditor, LinuxPlayer }
    public enum FilterMode { Point, Bilinear, Trilinear }
    public enum TextureWrapMode { Repeat, Clamp }
    public enum TextureFormat { RGBA32, ARGB32, RGB24, ETC2_RGBA8, ETC_RGB4, ETC2_RGB, ETC2_RGBA1, DXT1, DXT5, ASTC_4x4 }
    public enum SystemLanguage { Unknown, Chinese, English, Vietnamese }
    public enum LogType { Error, Warning, Log, Exception }
    public enum KeyCode { None, Escape, Space, Return, A, D, W, S, LeftArrow, RightArrow, UpArrow, DownArrow }
    public enum SpriteMeshType { FullRect, Tight }

    public class Component : Object
    {
        public Transform transform { get; set; }
        public GameObject gameObject { get; set; }
        public T GetComponent<T>() where T : class => null;
        public T GetComponentInParent<T>() where T : class => null;
        public T GetComponentInChildren<T>() where T : class => null;
        public T[] GetComponentsInChildren<T>() where T : class => Array.Empty<T>();
        public T AddComponent<T>() where T : Component, new() => new T();
    }

    public class Behaviour : Component { public bool enabled { get; set; } public bool isActiveAndEnabled => enabled; }

    public class MonoBehaviour : Behaviour
    {
        public Coroutine StartCoroutine(IEnumerator routine) => null;
        public void StopCoroutine(Coroutine c) { }
        public void StopAllCoroutines() { }
        public static T FindFirstObjectByType<T>() where T : Object => null;
        public static T FindObjectOfType<T>() where T : Object => null;
        public void Invoke(string method, float time) { }
        public void CancelInvoke() { }
    }

    public class Coroutine { }
    public class WaitForSeconds { public WaitForSeconds(float s) { } }
    public class WaitForEndOfFrame { }

    public class GameObject : Object
    {
        public GameObject() { }
        public GameObject(string name) { this.name = name; }
        public GameObject(string name, params Type[] components) { this.name = name; }
        public Transform transform { get; set; } = new RectTransform();
        public bool activeSelf => true;
        public bool activeInHierarchy => true;
        public string tag { get; set; }
        public int layer { get; set; }
        public void SetActive(bool value) { }
        public T GetComponent<T>() where T : class => null;
        public T GetComponentInChildren<T>() where T : class => null;
        public T[] GetComponentsInChildren<T>() where T : class => Array.Empty<T>();
        public T AddComponent<T>() where T : Component, new() => new T { gameObject = this, transform = transform };
        public Component AddComponent(Type t) => null;
        public static GameObject Find(string name) => null;
    }

    public class Transform : Component, IEnumerable
    {
        public Transform parent { get; set; }
        public int childCount => 0;
        public Vector3 position { get; set; }
        public Vector3 localPosition { get; set; }
        public Vector3 localScale { get; set; }
        public Quaternion rotation { get; set; }
        public Quaternion localRotation { get; set; }
        public Transform GetChild(int index) => null;
        public Transform Find(string name) => null;
        public void SetParent(Transform p, bool worldPositionStays = true) { }
        public void SetAsLastSibling() { }
        public void SetAsFirstSibling() { }
        public void SetSiblingIndex(int index) { }
        public IEnumerator GetEnumerator() => Array.Empty<Transform>().GetEnumerator();
    }

    public class RectTransform : Transform
    {
        public Vector2 anchorMin { get; set; }
        public Vector2 anchorMax { get; set; }
        public Vector2 anchoredPosition { get; set; }
        public Vector2 sizeDelta { get; set; }
        public Vector2 pivot { get; set; }
        public Vector2 offsetMin { get; set; }
        public Vector2 offsetMax { get; set; }
        public Rect rect => new Rect(0, 0, 0, 0);
        public enum Axis { Horizontal, Vertical }
        public void SetSizeWithCurrentAnchors(Axis axis, float size) { }
    }

    public class Texture : Object { public FilterMode filterMode { get; set; } public TextureWrapMode wrapMode { get; set; } public int width => 0; public int height => 0; }

    public class Texture2D : Texture
    {
        public Texture2D(int width, int height) { }
        public Texture2D(int width, int height, TextureFormat format, bool mipChain) { }
        public Texture2D(int width, int height, TextureFormat format, int mipCount, bool linear) { }
        public Texture2D(int width, int height, TextureFormat format, bool mipChain, bool linear) { }
        public void LoadRawTextureData(byte[] data) { }
        public void LoadRawTextureData(IntPtr data, int size) { }
        public void Apply(bool updateMipmaps, bool makeNoLongerReadable) { }
        public bool LoadImage(byte[] data) => true;
        public bool LoadImage(byte[] data, bool markNonReadable) => true;
        public void SetPixels32(Color32[] colors) { }
        public Color32[] GetPixels32() => Array.Empty<Color32>();
        public void SetPixel(int x, int y, Color c) { }
        public Color GetPixel(int x, int y) => Color.white;
        public Color[] GetPixels() => Array.Empty<Color>();
        public Color[] GetPixels(int x, int y, int w, int h) => Array.Empty<Color>();
        public void SetPixels(Color[] colors) { }
        public void SetPixels(int x, int y, int w, int h, Color[] colors) { }
        public byte[] GetRawTextureData() => Array.Empty<byte>();
        public static byte[] EncodeToPNGStatic() => Array.Empty<byte>();
        public void Apply() { }
        public void Apply(bool updateMipmaps) { }
        public static Texture2D whiteTexture => null;
    }

    public class Sprite : Object
    {
        public Rect rect => new Rect(0, 0, 0, 0);
        public Texture2D texture => null;
        public Vector4 border => new Vector4(0, 0, 0, 0);
        public float pixelsPerUnit => 100f;
        public static Sprite Create(Texture2D texture, Rect rect, Vector2 pivot) => null;
        public static Sprite Create(Texture2D texture, Rect rect, Vector2 pivot, float pixelsPerUnit) => null;
        public static Sprite Create(Texture2D texture, Rect rect, Vector2 pivot, float pixelsPerUnit, uint extrude, SpriteMeshType meshType) => null;
        public static Sprite Create(Texture2D texture, Rect rect, Vector2 pivot, float pixelsPerUnit, uint extrude, SpriteMeshType meshType, Vector4 border) => null;
    }

    public class Font : Object { public static Font CreateDynamicFontFromOSFont(string name, int size) => null; public static Font CreateDynamicFontFromOSFont(string[] names, int size) => null; public static string[] GetOSInstalledFontNames() => Array.Empty<string>(); }
    public class Material : Object { public Material(Shader s) { } public Color color { get; set; } }
    public class Shader : Object { public static Shader Find(string name) => null; }
    public class Canvas : Behaviour { public static void ForceUpdateCanvases() { } public RenderMode renderMode { get; set; } public int sortingOrder { get; set; } public Camera worldCamera { get; set; } public bool pixelPerfect { get; set; } }
    public enum RenderMode { ScreenSpaceOverlay, ScreenSpaceCamera, WorldSpace }
    public class Camera : Behaviour { public static Camera main => null; public Color backgroundColor { get; set; } public float orthographicSize { get; set; } public bool orthographic { get; set; } public CameraClearFlags clearFlags { get; set; } public float nearClipPlane { get; set; } public float farClipPlane { get; set; } public int depth { get; set; } public int cullingMask { get; set; } }
    public class SpriteRenderer : Component { public Sprite sprite { get; set; } public Color color { get; set; } public int sortingOrder { get; set; } }

    public static class Application
    {
        public static string persistentDataPath => ".";
        public static string streamingAssetsPath => ".";
        public static string dataPath => ".";
        public static RuntimePlatform platform => RuntimePlatform.WindowsPlayer;
        public static bool isEditor => false;
        public static bool isBatchMode => false;
        public static int targetFrameRate { get; set; }
        public static SystemLanguage systemLanguage => SystemLanguage.English;
        public static void Quit() { }
    }

    public static class Debug
    {
        public static void Log(object message) { }
        public static void LogWarning(object message) { }
        public static void LogError(object message) { }
        public static void LogException(Exception e) { }
    }

    public static class Time
    {
        public static float deltaTime => 0.016f;
        public static float time => 0f;
        public static float realtimeSinceStartup => 0f;
        public static float timeScale { get; set; }
        public static float unscaledTime => 0f;
        public static float fixedDeltaTime => 0.02f;
        public static int frameCount => 0;
    }

    public static class Screen
    {
        public static int width => 1920;
        public static int height => 1080;
        public static ScreenOrientation orientation { get; set; }
        public static bool autorotateToLandscapeLeft { get; set; }
        public static bool autorotateToLandscapeRight { get; set; }
        public static bool autorotateToPortrait { get; set; }
        public static bool autorotateToPortraitUpsideDown { get; set; }
        public static Rect safeArea => new Rect(0, 0, 1920, 1080);
        public static void SetResolution(int w, int h, bool fullscreen) { }
        public static bool fullScreen { get; set; }
        public static float dpi => 96f;
        public static int sleepTimeout { get; set; }
    }

    public struct Touch { public int fingerId; public Vector2 position; public Vector2 deltaPosition; public TouchPhase phase; }
    public enum TouchPhase { Began, Moved, Stationary, Ended, Canceled }

    public static class Input
    {
        public static Vector3 mousePosition => Vector3.zero;
        public static bool GetMouseButton(int b) => false;
        public static bool GetMouseButtonDown(int b) => false;
        public static bool GetMouseButtonUp(int b) => false;
        public static bool GetKey(KeyCode k) => false;
        public static bool GetKeyDown(KeyCode k) => false;
        public static int touchCount => 0;
        public static Touch GetTouch(int i) => new Touch();
        public static Touch[] touches => Array.Empty<Touch>();
        public static float GetAxis(string name) => 0f;
    }

    public static class JsonUtility
    {
        public static string ToJson(object obj) => "{}";
        public static string ToJson(object obj, bool prettyPrint) => "{}";
        public static T FromJson<T>(string json) => default;
        public static void FromJsonOverwrite(string json, object target) { }
    }

    public static class PlayerPrefs
    {
        public static void SetString(string key, string value) { }
        public static string GetString(string key, string defaultValue = "") => defaultValue;
        public static void SetInt(string key, int value) { }
        public static int GetInt(string key, int defaultValue = 0) => defaultValue;
        public static void Save() { }
        public static bool HasKey(string key) => false;
        public static void DeleteKey(string key) { }
    }

    public static class Resources { public static T Load<T>(string path) where T : Object => null; public static T GetBuiltinResource<T>(string path) where T : Object => null; }
    public static class Random { public static int Range(int min, int max) => min; public static float Range(float min, float max) => min; public static float value => 0f; }

    public class SerializeField : Attribute { }
    public class RuntimeInitializeOnLoadMethodAttribute : Attribute { public RuntimeInitializeOnLoadMethodAttribute() { } public RuntimeInitializeOnLoadMethodAttribute(RuntimeInitializeLoadType t) { } }
    public enum RuntimeInitializeLoadType { AfterSceneLoad, BeforeSceneLoad, BeforeSplashScreen, SubsystemRegistration }
    public class RequireComponent : Attribute { public RequireComponent(Type t) { } }
    public class AddComponentMenu : Attribute { public AddComponentMenu(string s) { } }
}

namespace UnityEngine.Events
{
    public delegate void UnityAction();
    public delegate void UnityAction<T0>(T0 arg0);

    public class UnityEventBase { }

    public class UnityEvent : UnityEventBase
    {
        public void AddListener(UnityAction call) { }
        public void RemoveListener(UnityAction call) { }
        public void RemoveAllListeners() { }
        public void Invoke() { }
    }

    public class UnityEvent<T0> : UnityEventBase
    {
        public void AddListener(UnityAction<T0> call) { }
        public void RemoveListener(UnityAction<T0> call) { }
        public void RemoveAllListeners() { }
        public void Invoke(T0 arg0) { }
    }
}

namespace UnityEngine.EventSystems
{
    public class PointerEventData { public Vector2 position; public Vector2 delta; public int pointerId; public Camera pressEventCamera; public Camera enterEventCamera; }
    public interface IDragHandler { void OnDrag(PointerEventData eventData); }
    public interface IPointerDownHandler { void OnPointerDown(PointerEventData eventData); }
    public interface IPointerUpHandler { void OnPointerUp(PointerEventData eventData); }
    public interface IPointerClickHandler { void OnPointerClick(PointerEventData eventData); }
    public interface IBeginDragHandler { void OnBeginDrag(PointerEventData eventData); }
    public interface IEndDragHandler { void OnEndDrag(PointerEventData eventData); }
    public class EventSystem : UIBehaviour { }
    public class UIBehaviour : MonoBehaviour { }
    public class StandaloneInputModule : UIBehaviour { }
    public class BaseRaycaster : UIBehaviour { }
}

namespace UnityEngine.UI
{
    using UnityEngine.Events;

    public class Graphic : UnityEngine.EventSystems.UIBehaviour
    {
        public Color color { get; set; }
        public bool raycastTarget { get; set; }
        public RectTransform rectTransform => null;
        public void SetAllDirty() { }
    }

    public class MaskableGraphic : Graphic { }

    public class Image : MaskableGraphic
    {
        public Sprite sprite { get; set; }
        public Type type { get; set; }
        public bool preserveAspect { get; set; }
        public float fillAmount { get; set; }
        public FillMethod fillMethod { get; set; }
        public Material material { get; set; }
        public new enum Type { Simple, Sliced, Tiled, Filled }
        public enum FillMethod { Horizontal, Vertical, Radial90, Radial180, Radial360 }
        public void SetNativeSize() { }
    }

    public class RawImage : MaskableGraphic { public Texture texture { get; set; } public Rect uvRect { get; set; } }

    public class Text : MaskableGraphic
    {
        public string text { get; set; }
        public int fontSize { get; set; }
        public Font font { get; set; }
        public FontStyle fontStyle { get; set; }
        public TextAnchor alignment { get; set; }
        public bool resizeTextForBestFit { get; set; }
        public int resizeTextMinSize { get; set; }
        public int resizeTextMaxSize { get; set; }
        public bool supportRichText { get; set; }
        public HorizontalWrapMode horizontalOverflow { get; set; }
        public VerticalWrapMode verticalOverflow { get; set; }
        public float lineSpacing { get; set; }
        public float preferredHeight => 0f;
        public float preferredWidth => 0f;
    }

    public enum HorizontalWrapMode { Wrap, Overflow }
    public enum VerticalWrapMode { Truncate, Overflow }

    public class Selectable : UnityEngine.EventSystems.UIBehaviour
    {
        public bool interactable { get; set; }
        public Graphic targetGraphic { get; set; }
        public ColorBlock colors { get; set; }
    }

    public struct ColorBlock { public Color normalColor, highlightedColor, pressedColor, selectedColor, disabledColor; public float colorMultiplier, fadeDuration; }

    public class Button : Selectable { public ButtonClickedEvent onClick { get; } = new ButtonClickedEvent(); public class ButtonClickedEvent : UnityEvent { } }

    public class Toggle : Selectable { public bool isOn { get; set; } public ToggleEvent onValueChanged { get; } = new ToggleEvent(); public class ToggleEvent : UnityEvent<bool> { } }

    public class Slider : Selectable
    {
        public float value { get; set; }
        public float minValue { get; set; }
        public float maxValue { get; set; }
        public SliderEvent onValueChanged { get; } = new SliderEvent();
        public class SliderEvent : UnityEvent<float> { }
    }

    public class InputField : Selectable
    {
        public string text { get; set; }
        public Text textComponent { get; set; }
        public Text placeholder { get; set; }
        public int characterLimit { get; set; }
        public ContentType contentType { get; set; }
        public SubmitEvent onEndEdit { get; } = new SubmitEvent();
        public OnChangeEvent onValueChanged { get; } = new OnChangeEvent();
        public enum ContentType { Standard, IntegerNumber, DecimalNumber, Alphanumeric, Name, Password }
        public class SubmitEvent : UnityEvent<string> { }
        public class OnChangeEvent : UnityEvent<string> { }
        public void ActivateInputField() { }
    }

    public class ScrollRect : UnityEngine.EventSystems.UIBehaviour
    {
        public RectTransform content { get; set; }
        public RectTransform viewport { get; set; }
        public bool horizontal { get; set; }
        public bool vertical { get; set; }
        public float verticalNormalizedPosition { get; set; }
        public float horizontalNormalizedPosition { get; set; }
        public MovementType movementType { get; set; }
        public Scrollbar verticalScrollbar { get; set; }
        public float scrollSensitivity { get; set; }
        public enum MovementType { Unrestricted, Elastic, Clamped }
    }

    public class Scrollbar : Selectable { public float value { get; set; } public float size { get; set; } }
    public class Mask : UnityEngine.EventSystems.UIBehaviour { public bool showMaskGraphic { get; set; } }
    public class RectMask2D : UnityEngine.EventSystems.UIBehaviour { }
    public class CanvasGroup : Component { public float alpha { get; set; } public bool interactable { get; set; } public bool blocksRaycasts { get; set; } }
    public class GraphicRaycaster : UnityEngine.EventSystems.BaseRaycaster { }

    public class LayoutElement : UnityEngine.EventSystems.UIBehaviour
    {
        public float preferredHeight { get; set; }
        public float preferredWidth { get; set; }
        public float minHeight { get; set; }
        public float minWidth { get; set; }
        public float flexibleHeight { get; set; }
        public float flexibleWidth { get; set; }
        public bool ignoreLayout { get; set; }
    }

    public class LayoutGroup : UnityEngine.EventSystems.UIBehaviour { public RectOffset padding { get; set; } public TextAnchor childAlignment { get; set; } }

    public class HorizontalOrVerticalLayoutGroup : LayoutGroup
    {
        public float spacing { get; set; }
        public bool childForceExpandWidth { get; set; }
        public bool childForceExpandHeight { get; set; }
        public bool childControlWidth { get; set; }
        public bool childControlHeight { get; set; }
    }

    public class VerticalLayoutGroup : HorizontalOrVerticalLayoutGroup { }
    public class HorizontalLayoutGroup : HorizontalOrVerticalLayoutGroup { }

    public class GridLayoutGroup : LayoutGroup
    {
        public Vector2 cellSize { get; set; }
        public Vector2 spacing { get; set; }
        public Constraint constraint { get; set; }
        public int constraintCount { get; set; }
        public enum Constraint { Flexible, FixedColumnCount, FixedRowCount }
    }

    public class ContentSizeFitter : UnityEngine.EventSystems.UIBehaviour
    {
        public FitMode horizontalFit { get; set; }
        public FitMode verticalFit { get; set; }
        public enum FitMode { Unconstrained, MinSize, PreferredSize }
    }

    public class CanvasScaler : UnityEngine.EventSystems.UIBehaviour
    {
        public ScaleMode uiScaleMode { get; set; }
        public Vector2 referenceResolution { get; set; }
        public ScreenMatchMode screenMatchMode { get; set; }
        public float matchWidthOrHeight { get; set; }
        public float referencePixelsPerUnit { get; set; }
        public enum ScaleMode { ConstantPixelSize, ScaleWithScreenSize, ConstantPhysicalSize }
        public enum ScreenMatchMode { MatchWidthOrHeight, Expand, Shrink }
    }

    public class Outline : UnityEngine.EventSystems.UIBehaviour { public Color effectColor { get; set; } public Vector2 effectDistance { get; set; } }
    public class Shadow : UnityEngine.EventSystems.UIBehaviour { public Color effectColor { get; set; } public Vector2 effectDistance { get; set; } }
    public class LayoutRebuilder { public static void ForceRebuildLayoutImmediate(RectTransform rt) { } }
}

namespace UnityEngine
{
    public class RectOffset
    {
        public RectOffset() { }
        public RectOffset(int left, int right, int top, int bottom) { this.left = left; this.right = right; this.top = top; this.bottom = bottom; }
        public int left { get; set; }
        public int right { get; set; }
        public int top { get; set; }
        public int bottom { get; set; }
    }
}

namespace UnityEngine
{
    public class TooltipAttribute : System.Attribute { public TooltipAttribute(string s) { } }
    public class DisallowMultipleComponentAttribute : System.Attribute { }
    public class HeaderAttribute : System.Attribute { public HeaderAttribute(string s) { } }
    public class RangeAttribute : System.Attribute { public RangeAttribute(float min, float max) { } }
}

namespace UnityEngine.Networking
{
    public class UnityWebRequest : System.IDisposable
    {
        public string url { get; set; }
        public bool isDone => true;
        public bool isNetworkError => false;
        public bool isHttpError => false;
        public Result result => Result.Success;
        public string error => null;
        public DownloadHandler downloadHandler { get; set; }
        public float downloadProgress => 1f;
        public long responseCode => 200;
        public enum Result { InProgress, Success, ConnectionError, ProtocolError, DataProcessingError }
        public static UnityWebRequest Get(string uri) => new UnityWebRequest();
        public UnityWebRequestAsyncOperation SendWebRequest() => new UnityWebRequestAsyncOperation();
        public void Dispose() { }
    }

    public class UnityWebRequestAsyncOperation : UnityEngine.AsyncOperation { }
    public class DownloadHandler { public byte[] data => System.Array.Empty<byte>(); public string text => ""; }
    public class DownloadHandlerFile : DownloadHandler { public DownloadHandlerFile(string path) { } public bool removeFileOnAbort { get; set; } }
}

namespace UnityEngine
{
    public class AsyncOperation : System.Collections.IEnumerator
    {
        public bool isDone => true;
        public float progress => 1f;
        public object Current => null;
        public bool MoveNext() => false;
        public void Reset() { }
    }
}

namespace UnityEngine
{
    public class CanvasRenderer : Component { }
    public static class RectTransformUtility
    {
        public static bool ScreenPointToLocalPointInRectangle(RectTransform rect, Vector2 screenPoint, Camera cam, out Vector2 localPoint) { localPoint = Vector2.zero; return true; }
        public static bool RectangleContainsScreenPoint(RectTransform rect, Vector2 screenPoint, Camera cam) => true;
    }
    public static class SleepTimeout { public const int NeverSleep = -1; public const int SystemSetting = -2; }
}

namespace UnityEngine
{
    public static class ResourcesBuiltinExtensions { }
}

namespace UnityEngine
{
    public static class SystemInfo
    {
        public static bool SupportsTextureFormat(TextureFormat format) => true;
        public static string deviceModel => "stub";
        public static string operatingSystem => "stub";
        public static int graphicsMemorySize => 1024;
    }
    public enum CameraClearFlags { Skybox, SolidColor, Depth, Nothing }
}
