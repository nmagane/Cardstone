using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretDisplay : MonoBehaviour
{
    public Card.Cardname card;
    public Hero hero;
    public Board board => hero.board;
    public SpriteRenderer icon;
    public Sprite[] secretIcons;
    public Sprite[] secretTriggerIcons;
    public bool init = false;
    Card.Class classType = Card.Class.Neutral;
    public void Set(Card.Cardname c)
    {
        Database.CardInfo cardInfo = Database.GetCardData(c);
        card = c;
        classType = cardInfo.classType;
        icon.sprite = secretIcons[(int)cardInfo.classType];
    }

    private void OnMouseExit()
    {
        hoverTimer = 0;
        hero.board.HideHoverTip();
    }

    int hoverTimer = 0;
    private void OnMouseOver()
    {
        if (hoverTimer < 30)
        {
            hoverTimer++;
            if (hoverTimer == 30)
                hero.board.ShowHoverTip(gameObject,card);
        }
    }

    public IEnumerator TriggerAnim()
    {
        board.secretPopup.sprite = secretTriggerIcons[(int)classType];
        yield return board.animationManager.LerpZoom(board.secretPopup.gameObject, Vector3.one, 5, 0.2f);
        yield return Board.Wait(60);
        board.animationManager.LerpZoom(this.gameObject, Vector3.zero, 5, 0);
        yield return board.animationManager.LerpZoom(board.secretPopup.gameObject, Vector3.zero, 5, 0);
        hero.OrderSecrets();
        Destroy(this.gameObject);

    }
}
