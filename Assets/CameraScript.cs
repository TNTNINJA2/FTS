using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraScript : MonoBehaviour
{
    [SerializeField] private Tilemap tileMap;
    [SerializeField] private Camera camera;
    [SerializeField] private float buffer;
    // Start is called before the first frame update
    void Start()
    {
        var (center, size) = CalculateOrthoSize();
        camera.transform.position = center;
        camera.orthographicSize = size;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private (Vector3 center, float size) CalculateOrthoSize()
    {
        tileMap.CompressBounds();
        var bounds = tileMap.localBounds;
        bounds.Expand(buffer);

        var vertical = bounds.size.y;
        var horizontal = bounds.size.x * camera.pixelHeight / camera.pixelWidth;

        var size = Mathf.Max(horizontal, vertical) * 0.5f;
        var center = bounds.center + new Vector3(0, 0, -10);

        return (center, size);
    }
}
