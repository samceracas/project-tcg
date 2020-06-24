using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneTimeEffectScript : MonoBehaviour
{
    private void Start()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (GetComponentInParent<SpriteRenderer>() == null)
        {
            Debug.Log("No parent sprite renderer found!");
            return;
        }

        UnitScript parentSpriteRenderer = GetComponentInParent<UnitScript>();
        transform.localScale = GetRenderedSize(parentSpriteRenderer.Sprite).pixel / GetRenderedSize(spriteRenderer).pixel;
        transform.localScale *= 0.65f;
        transform.localPosition = Vector3.zero;
    }

    public (Vector2 world, Vector2 screen, Vector2 pixel) GetRenderedSize(SpriteRenderer spriteRenderer)
    {
        //get world space size (this version operates on the bounds of the object, so expands when rotating)
        //Vector3 world_size = GetComponent<SpriteRenderer>().bounds.size;

        //get world space size (this version handles rotating correctly)
        Vector2 sprite_size = spriteRenderer.sprite.rect.size;
        Vector2 local_sprite_size = sprite_size / GetComponent<SpriteRenderer>().sprite.pixelsPerUnit;
        Vector3 world_size = local_sprite_size;
        world_size.x *= transform.lossyScale.x;
        world_size.y *= transform.lossyScale.y;

        //convert to screen space size
        Vector3 screen_size = 0.5f * world_size / Camera.main.orthographicSize;
        screen_size.y *= Camera.main.aspect;

        //size in pixels
        Vector3 in_pixels = new Vector3(screen_size.x * Camera.main.pixelWidth, screen_size.y * Camera.main.pixelHeight, 0) * 0.5f;

        return (world: world_size, screen: screen_size, pixel: in_pixels);
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
