using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HistoryManager : MonoBehaviour
{
    public GameObject elementObject;
    public Board board;
    public List<HistoryElement> elements = new List<HistoryElement>();


    public void AddElement(Card.Cardname card, HistoryElement.Type type)
    {
        HistoryElement newElem = Instantiate(elementObject).GetComponent<HistoryElement>();
        newElem.transform.parent = this.transform;
        newElem.transform.localScale = Vector3.zero;
        newElem.transform.localPosition = new Vector3(0, 3.75f);
        newElem.board = board;
        newElem.Set(card);
        elements.Insert(0,newElem);
        if (elements.Count > 6)
        {
            Destroy(elements.Last().gameObject);
            elements.Remove(elements.Last());
        }

        int i = 0;
        board.animationManager.LerpZoom(newElem.gameObject, Vector3.one, 10);
        foreach (var elem in elements)
        {
            board.animationManager.LerpTo(elem,new Vector3(0, 3.75f - 1.5f * i++),6);
        }
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            AddElement((Card.Cardname)Random.Range(10, 30), HistoryElement.Type.Play);
        }
    }
}
