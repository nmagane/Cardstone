using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropShadow : MonoBehaviour
{
    SpriteRenderer parent;
    public SpriteRenderer spriteRenderer;
    public Vector3 offset;
    GameObject shadow;
    public bool enableCustomColor = false;
    public Color customColor;
    public int layerGap = 1;
    public string sortingLayer = "shadow";
    void Awake()
    {
        parent = GetComponent<SpriteRenderer>();
        shadow = new GameObject("dropshadow_"+this.gameObject.name);
        spriteRenderer = shadow.AddComponent<SpriteRenderer>();
        spriteRenderer.enabled = parent.enabled;
        spriteRenderer.sortingLayerName = sortingLayer;
        spriteRenderer.sortingOrder = parent.sortingOrder- layerGap;
        spriteRenderer.material = Camera.main.GetComponentInChildren<AnimationManager>().shadowMat;
        spriteRenderer.color = new Color(0.2f, 0.2f, 0.2f,parent.color.a);
        if (enableCustomColor)
        {
            spriteRenderer.color = new Color(customColor.r, customColor.g, customColor.b, parent.color.a);
        }
        spriteRenderer.maskInteraction = parent.maskInteraction;
        spriteRenderer.drawMode = parent.drawMode;
        shadow.transform.parent = this.transform;
        spriteRenderer.transform.localEulerAngles = Vector3.zero;
        shadow.transform.localScale = Vector3.one;
        if (spriteRenderer.drawMode == SpriteDrawMode.Tiled || spriteRenderer.drawMode == SpriteDrawMode.Sliced)
            spriteRenderer.size = parent.size;
    }
    public float elevation = 0.3f;
    public void SetOffset()
    {
        float y = transform.position.y;
        float x = transform.position.x;
        float v = (9 + y) / 18;
        float h = (14+x) / 28;
        offset = new Vector3(h * elevation, v * -elevation);
    }
    void Update()
    {
        SetOffset();
        shadow.transform.position = this.transform.position + offset;
        spriteRenderer.sprite = parent.sprite;
        spriteRenderer.enabled = parent.enabled;
        if (enableCustomColor)
        {
            spriteRenderer.color = new Color(customColor.r, customColor.g, customColor.b, parent.color.a*customColor.a);
        }
        else
            spriteRenderer.color = new Color(0.2f, 0.2f, 0.2f, parent.color.a);

        spriteRenderer.flipX = parent.flipX;
        spriteRenderer.flipY = parent.flipY;
        spriteRenderer.sortingOrder = parent.sortingOrder - layerGap;
        if (spriteRenderer.drawMode==SpriteDrawMode.Tiled || spriteRenderer.drawMode==SpriteDrawMode.Sliced)
            spriteRenderer.size = parent.size;
    }
}
