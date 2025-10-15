using UnityEngine;

public class MiniMapFollow : MonoBehaviour
{
    public RectTransform miniMapImage;
    public RectTransform playerIcon;
    public Transform playerWorldTransform;

    public Vector2 worldBottomLeft;
    public Vector2 worldTopRight;

    private Vector2 minWorld;
    private Vector2 maxWorld;
    private Vector2 mapSize;

    private void Start()
    {
        minWorld = new Vector2(
            Mathf.Min(worldBottomLeft.x, worldTopRight.x),
            Mathf.Min(worldBottomLeft.y, worldTopRight.y)
        );

        maxWorld = new Vector2(
            Mathf.Max(worldBottomLeft.x, worldTopRight.x),
            Mathf.Max(worldBottomLeft.y, worldTopRight.y)
        );

        mapSize = miniMapImage.rect.size;
    }

    private void Update()
    {
        Vector3 playerPos = playerWorldTransform.position;

        float normalizedX = Mathf.InverseLerp(minWorld.x, maxWorld.x, playerPos.x);
        float normalizedY = Mathf.InverseLerp(minWorld.y, maxWorld.y, playerPos.z); // Z porque mundo 3D

        normalizedY = 1f - normalizedY; // si es necesario
        normalizedX = 1f - normalizedX; // si es necesario

        float offsetX = (normalizedX - 0.5f) * mapSize.x * miniMapImage.localScale.x;
        float offsetY = (normalizedY - 0.5f) * mapSize.y * miniMapImage.localScale.y;

        miniMapImage.anchoredPosition = new Vector2(-offsetX, -offsetY);

        // Obtener la rotación del jugador
        float playerYaw = playerWorldTransform.eulerAngles.y;

        // Ajustar según que el norte apunta al -Z en el mundo
        float iconRotation = -(playerYaw - 180f);

        // Aplicar rotación al ícono en el eje Z
        playerIcon.localRotation = Quaternion.Euler(0f, 0f, iconRotation);
    }
}