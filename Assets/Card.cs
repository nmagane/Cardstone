using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Device;

public class Card : MonoBehaviour
{
    public Board board;
    public Board.HandCard card;

    public new TMP_Text name;
    public TMP_Text text;
    public TMP_Text manaCost;
    public SpriteRenderer icon;
    public enum Cardname
    {
        F0,
        F1,
        F2,
        F3,
        F4,
        F5,
        F6,
        F7,
        F8,
        F9,
        F10,
        F11,
        F12,
        F13,
        F14,
        F15,
    }
    void Start()
    {
        
    }

    public void Set(Board.HandCard card)
    {
        name.text = card.card.ToString();
    }
    void Update()
    {
        
    }
    public Vector3 GetMousePos()
    {
        return (Camera.main.ScreenToWorldPoint(Input.mousePosition) - new Vector3(0, 0, Camera.main.ScreenToWorldPoint(Input.mousePosition).z));
    }

    Vector3 OP = new Vector3();
    private void OnMouseDown()
    {
        OP = transform.localPosition;
        offset = this.transform.position - GetMousePos();
    }

    private void OnMouseDrag()
    {
        transform.position = DragPos();
    }

    private void OnMouseUp()
    {
        transform.localPosition = OP;
    }

    Vector3 offset;
    public Vector3 DragPos()
    {

        return GetMousePos() + offset;
    }
}
