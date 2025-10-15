using UnityEngine;

public class Map : MonoBehaviour
{
    private RectTransform mapImage;
    public RectTransform playerIcon;

    public Transform playerTransform;

    public Vector2 worldBottomLeft;
    public Vector2 worldTopRight;

    private Vector2 minWorld;
    private Vector2 maxWorld;

    private void Start()
    {
        mapImage = GetComponent<RectTransform>();

        minWorld = new Vector2(
            Mathf.Min(worldBottomLeft.x, worldTopRight.x),
            Mathf.Min(worldBottomLeft.y, worldTopRight.y)
        );

        maxWorld = new Vector2(
            Mathf.Max(worldBottomLeft.x, worldTopRight.x),
            Mathf.Max(worldBottomLeft.y, worldTopRight.y)
        );
    }

    void Update()
    {
        Vector3 playerPos = playerTransform.position;

        float normalizedX = Mathf.InverseLerp(minWorld.x, maxWorld.x, playerPos.x);
        float normalizedY = Mathf.InverseLerp(minWorld.y, maxWorld.y, playerPos.z);

        normalizedY = 1f - normalizedY;
        normalizedX = 1f - normalizedX;

        float mapWidth = mapImage.rect.width;
        float mapHeight = mapImage.rect.height;

        float iconX = (normalizedX - 0.5f) * mapWidth;
        float iconY = (normalizedY - 0.5f) * mapHeight;
        playerIcon.anchoredPosition = new Vector2(iconX, iconY);

        // Obtener la rotación del jugador
        float playerYaw = playerTransform.eulerAngles.y;

        // Ajustar según que el norte apunta al -Z en el mundo
        float iconRotation = -(playerYaw - 180f);

        // Aplicar rotación al ícono en el eje Z
        playerIcon.localRotation = Quaternion.Euler(0f, 0f, iconRotation);
    }
}
