using System.Collections;
using UnityEngine;

public class RunnerCamera : MonoBehaviour
{
    public static RunnerCamera Instance { get; private set; }

    [Header("速度")]
    [SerializeField] private float scrollSpeed = 4f;

    [Header("跟随")]
    [SerializeField] private Transform player;
    [SerializeField] private float     verticalSmooth = 5f;

    public float CurrentSpeed => scrollSpeed;

    private Camera cam;

    void Awake()
    {
        Instance = this;
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (player == null) return;

        // 水平：跟随玩家X（玩家自动移动，摄像机跟着走）
        float newX = player.position.x + 2f;

        // 垂直：平滑跟随
        float newY = Mathf.Lerp(transform.position.y, player.position.y, verticalSmooth * Time.deltaTime);

        transform.position = new Vector3(newX, newY, transform.position.z);
    }

    public float RightEdgeX => transform.position.x + cam.orthographicSize * cam.aspect;
    public float LeftEdgeX  => transform.position.x - cam.orthographicSize * cam.aspect;

    // 屏幕震动
    public void Shake(float strength = 0.2f, float duration = 0.15f)
    {
        StartCoroutine(ShakeRoutine(strength, duration));
    }

    IEnumerator ShakeRoutine(float strength, float duration)
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            float decay = 1f - t;
            Vector3 offset = (Vector3)Random.insideUnitCircle * strength * decay;
            transform.position += offset;
            yield return null;
        }
    }
}