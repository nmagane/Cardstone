using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking.Types;

public class Creature : MonoBehaviour
{
    public void SetSortingOrder(int x)
    {
        x = x * 10;
        battlecrySprite.sortingOrder = x - 1;
        spriteRenderer.sortingOrder = x+1;
        highlight.sortingOrder = x+1;
        tauntSprite.sortingOrder = x;
        shieldSprite.sortingOrder = x+2;
        triggerSprite.sortingOrder = x+3;
        deathrattleSprite.sortingOrder = x+3;
        testname.GetComponent<MeshRenderer>().sortingOrder = x + 3;
        health.GetComponent<MeshRenderer>().sortingOrder = x + 3;
        damage.GetComponent<MeshRenderer>().sortingOrder = x + 3;
    }
    public bool isElevated = false;
    public void SetElevated(bool elevated)
    {
        isElevated = elevated;
        string x = elevated ? "creatureElevated" : "creature";
        string s = elevated ? " shadowCreatureElevated" : "shadowCreature";
        battlecrySprite.sortingLayerName = x;
        highlight.sortingLayerName = x;
        spriteRenderer.sortingLayerName = x;
        tauntSprite.sortingLayerName = x;
        shieldSprite.sortingLayerName = x;
        triggerSprite.sortingLayerName = x;
        deathrattleSprite.sortingLayerName = x;
        testname.GetComponent<MeshRenderer>().sortingLayerName = x;
        health.GetComponent<MeshRenderer>().sortingLayerName = x;
        damage.GetComponent<MeshRenderer>().sortingLayerName = x;
    }
    public TMP_Text testname;
    public TMP_Text health, damage;
    public SpriteRenderer spriteRenderer;
    public DropShadow shadow;
    public SpriteRenderer tauntSprite;
    public SpriteRenderer shieldSprite;
    public SpriteRenderer triggerSprite;
    public SpriteRenderer deathrattleSprite;
    public SpriteRenderer battlecrySprite;

    public SpriteRenderer highlight;
    public Sprite highlightNormal;
    public Sprite highlightTaunt;

    public Board board;

    public Minion minion;
    public int index => minion.index;
    public bool preview = false;
    public bool init = false;
    public void Set(Minion c)
    {
        minion = c;
        SetSortingOrder(minion.index);

        Database.CardInfo info = Database.GetCardData(c.card);
        testname.text = info.name;

        dmg = c.damage;
        hp = c.health;
        health.text = c.health.ToString();
        damage.text = c.damage.ToString();

        if (minion.HasAura(Aura.Type.Taunt))
            EnableTaunt();
        if (minion.HasAura(Aura.Type.Shield))
            EnableShield();
    }
    public bool IsFriendly()
    {
        if (board.enemyMinions.Contains(minion))
            return false;
        else
            return true;
    }
    void Start()
    {
        StartCoroutine(floater());
    }
    public Coroutine TriggerBattlecry()
    {
        return StartCoroutine(cryer());
    }
    public Coroutine TriggerTrigger()
    {
        return StartCoroutine(trigAnim());
    }
    IEnumerator trigAnim()
    {
        yield return board.animationManager.LerpZoom(triggerSprite.gameObject, Vector3.one*1.2f, 20);
        yield return board.animationManager.LerpZoom(triggerSprite.gameObject, Vector3.one, 20);
    }
    IEnumerator cryer()
    {
        yield return board.animationManager.LerpZoom(battlecrySprite.gameObject, Vector3.one, 30);
        board.animationManager.LerpZoom(battlecrySprite.gameObject, Vector3.zero, 30);
    }

    public Color baseText;
    public Color greenText;
    public Color redText;
    int dmg = 0;
    int hp = 0;
    public IEnumerator txtBounce(TMP_Text text)
    {
        int frames = 5;
        for (float i=0;i<frames;i++)
        {
            text.transform.localScale += Vector3.one * 0.15f;
            yield return AnimationManager.Wait(1);
        }
        for (float i=0;i<frames;i++)
        {
            text.transform.localScale += Vector3.one * -0.15f; 
            yield return AnimationManager.Wait(1);
        }
    }
    void Update()
    {
        if (dmg!=minion.damage)
        {
            StartCoroutine(txtBounce(damage));
        }
        if (hp!=minion.health)
        {
            StartCoroutine(txtBounce(health));
        }
        dmg = minion.damage;
        hp = minion.health;
        damage.text = minion.damage.ToString();
        health.text = minion.health.ToString();

        //====================
        if (minion.health < minion.maxHealth)
            health.color = redText;
        else if (minion.health > minion.baseHealth && minion.health == minion.maxHealth)
        {
            health.color = greenText;
        }
        else
            health.color = baseText;

        //=====================
        if (minion.damage > minion.baseDamage)
        {
            damage.color = greenText;
        }
        else
            damage.color = baseText;

        //=====================
        CheckTriggers();
    }

    public void Highlight()
    {
        if (isElevated) return;

        if (tauntSprite.enabled)
            highlight.sprite = highlightTaunt;
        else
            highlight.sprite = highlightNormal;

        highlight.enabled = true;
    }
    public void Unhighlight()
    {
        highlight.enabled = false;
    }
    public void EnableTaunt()
    {
        tauntSprite.enabled = true;
        board.animationManager.LerpZoom(tauntSprite.gameObject, Vector3.one, 10, 0.1f);
    }
    public void DisableTaunt()
    {
        tauntSprite.enabled = false;
        board.animationManager.LerpZoom(tauntSprite.gameObject, Vector3.zero, 10, 0.1f);
    }

    public void EnableShield()
    {
        board.animationManager.LerpZoom(shieldSprite.gameObject, Vector3.one, 10, 0.1f);
    }
    public void DisableShield()
    {
        board.animationManager.LerpZoom(shieldSprite.gameObject, Vector3.zero, 5, 0.1f);
    }

    public void CheckTriggers()
    {
        if (minion.triggers.Count>0)
        {
            bool d = false;
            foreach (Trigger t in minion.triggers)
            {
                if (t.type == Trigger.Type.Deathrattle)
                {
                    deathrattleSprite.enabled = true;
                    d = true;
                }
            }
            if (d == false)
                triggerSprite.enabled = true;
        }
        else
        {
            triggerSprite.enabled = false;
            deathrattleSprite.enabled = false;
        }
    }

    private void OnMouseOver()
    {
        if (preview) return;
        if (board.targeting && board.dragTargeting)
        {
            if (Input.GetMouseButtonUp(0))
            {
                if (board.targetingMinion == minion)
                {
                    //cancel by releasing on self
                    board.EndTargeting(true);
                    return;
                }
                board.TargetMinion(minion);
            }
        }
        //TODO: timer for tooltip to show up
    }
    private void OnMouseEnter()
    {
        board.hoveredMinion = this;
    }
    private void OnMouseExit()
    {
        board.hoveredMinion = null;
    }

    int dragCounter = 0;
    int dragTime = 8;
    private void OnMouseDrag()
    {
        if (preview) return;
        if (board.currTurn == false) return;
        if (board.targetingMinion==minion)
        {
            if (dragCounter < dragTime) dragCounter++;
            if (dragCounter >= dragTime)
            {
                if (Vector3.Distance(Card.GetMousePos(), clickPos) > 0.2f)
                {
                    board.dragTargeting = true;
                    //Debug.Log("drag");
                }
            }
        }

    }
    private void OnMouseUp()
    {
        if (preview) return; 
        if (board.currTurn == false) return;
        dragCounter = 0;
        if (board.dragTargeting && board.targetingMinion==minion)
        {
            //cancel by LETTING GO OVER NOTHING
            if (board.hoveredMinion==null && board.hoveredHero==null) 
                board.EndTargeting(true);
        }
    }

    Vector3 clickPos = Vector3.zero;
    private void OnMouseDown()
    {
        if (preview) return; 
        if (board.currTurn == false) return;
        if (board.targeting)
        {
            if (board.targetingMinion == minion)
            {
                //cancel by clicking on self
                board.EndTargeting(true);
                return;
            }

            board.TargetMinion(minion);
            return;
        }

        if (IsFriendly() == false) return;
        if (minion.canAttack == false) return;
        board.StartTargetingAttack(minion);
        clickPos = Card.GetMousePos();
    }

    public Vector3 boardPos;

    public bool floatEnabled = true;
    public IEnumerator floater()
    {
        int i = 0;
        float freq = 2F;// 1.5f;
        while (true)
        {
            if (floatEnabled)
            {
                float ang = freq * i * Mathf.PI / 180;
                transform.localScale = Vector3.one * ((1.025f + 0.01f * Mathf.Sin(ang)));
                //transform.localEulerAngles = new Vector3(0, 0, 0.15F * Mathf.Sin(0.5f * ang));
                shadow.elevation = (0.4f + 0.1f * Mathf.Sin(ang));
                i++;
                if (i == 360) i = 0;
                yield return null;
            }
            else
            {
                i = 90;
                //transform.localEulerAngles = Vector3.zero;//Vector3.Lerp(transform.localEulerAngles,Vector3.zero,0.25f);
                yield return null;
            }
        }
    }
}
