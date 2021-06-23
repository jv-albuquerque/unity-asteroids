using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericUtilities
{
    public Bounds MainCameraBounds()
    {
        Camera camera = Camera.main;

        float screenAspect = (float)Screen.width / (float)Screen.height;
        float cameraHeight = camera.orthographicSize * 2;

        Bounds bounds = new Bounds(
            camera.transform.position,
            new Vector3(cameraHeight * screenAspect, cameraHeight, 0)
            );

        return bounds;
    }

    public void WrapFromScreenEdge(Transform transform, float marginToWrap = 0)
    {
        Vector2 bounds = MainCameraBounds().extents;
        bounds += new Vector2(marginToWrap, marginToWrap);

        Vector2 pos = transform.position;

        if(pos.x < -bounds.x)
        {
            transform.position = new Vector2(bounds.x, pos.y);
        }
        else if(pos.x > bounds.x)
        {
            transform.position = new Vector2(-bounds.x, pos.y);
        }

        if(pos.y < -bounds.y)
        {
            transform.position = new Vector2(pos.x, bounds.y);
        }
        else if(pos.y > bounds.y)
        {
            transform.position = new Vector2(pos.x, -bounds.y);
        }
    }

    public float GetScreenWrapOffset(Renderer render)
    {
        Vector2 size = render.bounds.size;
        size /= 2; //this is made, because the pivot is in the center of the ship
        return size.x > size.y ? size.x : size.y;
    }

    public Vector2 Rotate(Vector2 v, float degrees)
    {
        float rad = degrees * Mathf.Deg2Rad;

        return new Vector2(
            v.x * Mathf.Cos(rad) - v.y * Mathf.Sin(rad),
            v.x * Mathf.Sin(rad) + v.y * Mathf.Cos(rad)
        );
    }
}
