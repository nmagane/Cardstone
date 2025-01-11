using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardSide : MonoBehaviour
{
    public Board board;
    private void OnMouseEnter()
    {
        board.hoveredSide = this;
    }
    private void OnMouseExit()
    {
        board.hoveredSide = null;
    }
}
