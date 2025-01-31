using System.Collections.Generic;
using System.Linq;
using Riptide;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;

[System.Serializable]
public class Hand
{
    //public List<Card.Cardname> cards;
    public List<HandCard> cards;

    public bool server = false;
    //public List<Card> cardObjects;
    public Board board;
    public Dictionary<HandCard, Card> cardObjects = new Dictionary<HandCard, Card>();
    public enum MulliganState
    {
        None,
        Waiting,
        Done,
    }
    public MulliganState mulliganMode = MulliganState.None;
    public bool enemyHand = false;
    public bool coinHand = false;
    public HandCard this[int index]
    {
        get
        {
            return cards[index];
        }

        set
        {
            cards[index] = value;

        }
    }
    public List<HandCard>.Enumerator GetEnumerator()
    {
        return cards.GetEnumerator();
    }
    public enum CardSource
    {
        Start,
        Deck,
        Board,
        EnemyDeck,
    }
    public HandCard Add(Card.Cardname x, int ind = -1, CardSource source = CardSource.Deck)
    {
        int index = ind == -1 ? Count() : ind;
        HandCard newCard = new HandCard(x, index);
        cards.Add(newCard);

        if (server)
        {
            OrderInds();
            return newCard;
        }

        Card c = board.CreateCard();
        c.board = board;
        c.Set(cards[index]);
        cardObjects.Add(cards[index], c);
        c.transform.parent = board.transform;
        
        switch(source)
        {
            case CardSource.Deck:
                c.transform.localPosition = (enemyHand) ? board.enemyDeck.transform.localPosition :board.deck.transform.localPosition;
                if (enemyHand == false)
                {
                    c.SetFlipped();
                    c.Flip();
                }
                break;
            case CardSource.Start:
                if (enemyHand)
                {
                    c.starter = true;
                }
                break;
        }

        OrderInds();



        return newCard;

    }
    public enum RemoveCardType
    {
        Play,
        PlayMinion,
        Discard,
    }
    public void RemoveAt(int x, RemoveCardType type = RemoveCardType.Play, Card.Cardname name = Card.Cardname.Coin, int pos = -1)
    {
        if (!server)
        {
            Card c = cardObjects[cards[x]];
            cardObjects.Remove(cards[x]);

            //board.DestroyObject(c);
            //todo: show cards custom mana cost if its changed by freezing trap/loatheb etc
            if (pos!=-1)
            {
                Vector3 p;
                float count = board.currMinions.minionObjects.Count + 1;
                float dist = 4.5f;
                float offset = -((count - 1) / 2f * dist);


                p = new Vector3(offset + dist * (pos), 1.5f+(enemyHand  ? 3 : -2.75f));

                if (c.hidden == false)
                    board.animationManager.PlayFade(c, p);
            }
            else
                FadeCard(c, enemyHand == false, type == RemoveCardType.Discard, name);
        }

        cards.RemoveAt(x);

        OrderInds();
    }

    public void Remove(HandCard c)
    {
        if (!server)
        {
            Card co = cardObjects[c];
            cardObjects.Remove(c);
            board.DestroyObject(co);
        }
        cards.Remove(c);
        OrderInds();
    }

    public void MulliganReplace(int index, Card.Cardname c)
    {
        cards[index].Set(c, index);
        board.animationManager.MulliganAnim(cardObjects[cards[index]], cards[index]);
        cardObjects[cards[index]].mulliganMark.enabled = false;
    }
    public void EndMulligan()
    {
        mulliganMode = MulliganState.Waiting;
        OrderInds();
    }
    public void ConfirmBothMulligans()
    {
        mulliganMode = MulliganState.Done;
        OrderInds();
    }
    public void OrderInds()
    {
        int i = 0;
        foreach (var c in cards)
        {
            c.index = i++;
        }
        if (server) return;

        if (mulliganMode != MulliganState.Done)
        {
            float count = cardObjects.Count;
            float dist = 6.5F;

            if (coinHand)
            {
                count--;
                var coinKVP = cardObjects.ElementAt(cards.Count - 1);

                coinKVP.Value.transform.localPosition = Vector3.zero;
                coinKVP.Value.transform.localScale = Vector3.zero;
            }

            Vector3 offset = new Vector3(-((count - 1) / 2f * dist), 0);

            foreach (var kvp in cardObjects)
            {
                Card c = kvp.Value;
                c.init = true;
                if (coinHand && c.card.card == Card.Cardname.Coin) continue;
                c.transform.localScale = Vector3.one * 1.5f;
                c.shadow.elevation = 1;
                c.transform.localPosition = offset + new Vector3(dist * (c.card.index), 0, 0);
            }

            return;
        }
        if (coinHand)
        {
            coinHand = false;
            var coinKVP = cardObjects.ElementAt(cards.Count - 1);
            coinKVP.Value.transform.localScale = Vector3.one;
        }
        float count2 = cardObjects.Count;
        float dist2 = 3.5f-0.15f*count2;
        float offset2 = -((count2 - 1) / 2f * dist2);

        //x2+y2=r2
        float radius = 40;
        foreach (var kvp in cardObjects)
        {
            if (enemyHand) break;
            Card c = kvp.Value;
            c.transform.localScale = Vector3.one;
            float x = offset2 + dist2*c.card.index;
            float y = -10 - (radius-Mathf.Sqrt(radius*radius-x*x));
            float angle = 180*Mathf.Acos(x/radius)/Mathf.PI;
            angle = angle-90;
            //c.transform.localPosition = 
            c.SetSortingOrder(c.card.index);
            MoveCard(c, new Vector3(x, y, -0.5f*c.card.index), new Vector3(0, 0, angle));
        }
        foreach (var kvp in cardObjects)
        {
            if (!enemyHand) break;
            Card c = kvp.Value;
            c.transform.localScale = Vector3.one;
            float x = offset2 + dist2*c.card.index;
            float y = 10 + (radius-Mathf.Sqrt(radius*radius-x*x));
            float angle = 180*Mathf.Asin(x/radius)/Mathf.PI;
            //angle = angle;
            //c.transform.localPosition = new Vector3(x, y, 0);
            c.SetSortingOrder(c.card.index);
            if (c.starter)
            {
                c.transform.localPosition = new Vector3(x, y, 0.5f * c.card.index);
                c.transform.localEulerAngles = new Vector3(0, 0, angle);
                c.starter = false;
            }
            else 
                MoveCard(c, new Vector3(x, y, -0.5f * c.card.index),new Vector3(0, 0, angle));
        }
        
    }
    public int Count()
    {
        return cards.Count;
    }
    public Hand()
    {
        cards = new List<HandCard>();
    }


    public void MoveCard(Card c, Vector3 location,Vector3 rotation)
    {
        c.handPos = location;
        c.handRot = rotation;
        int frames = 5;
        if (c.init==false)
        {
            c.init = true;
            frames += 10;
            if (enemyHand == false)
            {
                board.animationManager.DrawAnim(c.gameObject, new Vector3(13, -5, 0), location, 5, 5, 10,rotation);
                return;
            }
        }
        board.animationManager.LerpTo(c, location, frames, 0);
        c.ElevateTo(0.1f, frames);
        RotateCard(c, rotation);
        
    }
    public void RotateCard(Card c, Vector3 rotation)
    {
        board.animationManager.LerpRotate(c.gameObject, rotation, 5);
    }

    public void FadeCard(Card c, bool friendly, bool discard=false, Card.Cardname name =Card.Cardname.Coin)
    {
        c.noReturn = true;
        board.animationManager.FadeCard(c, friendly, discard, name);
    }
}
