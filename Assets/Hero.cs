using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Hero : MonoBehaviour
{
    public bool isElevated = false;
    public void SetElevated(bool elevated)
    {
        isElevated = elevated;
        string x = elevated ? "creatureElevated" : "top1";
        string s = elevated ? "shadowCreatureElevated" : "shadow";
        shadow.sortingLayer = s;

        spriteRenderer.sortingLayerName = x;
        skull.sortingLayerName = x;
        highlight.sortingLayerName = x;
        damageSpriteRenderer.sortingLayerName = x;
        damageText.GetComponent<MeshRenderer>().sortingLayerName = x;
        hpText.GetComponent<MeshRenderer>().sortingLayerName = x;
    }

    public Board board;

    public int health = 30;
    public int maxHealth = 30;

    public int damage = 0;
    public int armor = 0;

    bool _canAttack = false;
    public bool canAttack
    {
        get
        {
            return _canAttack && damage > 0;
        }
        set
        {
            _canAttack = value;
        }
    }
    public Weapon weapon;

    public SpriteRenderer spriteRenderer;
    public SpriteRenderer damageSpriteRenderer;

    public SpriteRenderer shadowSpriteRenderer;
    public DropShadow shadow;

    public float shadowElevation
    {
        get
        {
            return shadow.elevation;
        }
        set
        {
            shadow.elevation = value;
        }
    }

    public TMP_Text hpText;
    public TMP_Text damageText;

    public SpriteRenderer highlight;

    public Sprite highlightNormal;
    public Sprite highlightTarget;

    public SpriteRenderer weaponFrame;
    public SpriteRenderer weaponArt;
    public SpriteRenderer weaponDeathrattleSprite;
    public SpriteRenderer weaponTriggerSprite;
    public TMP_Text weaponDamage;
    public TMP_Text weaponDurability;

    public void SetHealth(int x)
    {
        health = x;
    }

    public void EquipWeapon(Card.Cardname c)
    {
        weapon = new Weapon(c);
    }

    public void DisplayWeapon()
    {
        if (weapon!=null)
        {
            HideWeapon();
        }
        //drop anim like creatures TODO
        if (hider!=null) StopCoroutine(hider);
        weaponFrame.transform.localScale = Vector3.one;
        weaponDamage.text = weapon.damage.ToString();
        weaponDurability.text = weapon.durability.ToString();

        CheckTriggers();
        weaponArt.sprite = board.cardObject.GetComponent<Card>().cardSprites[(int)weapon.card];
    }
    Coroutine hider = null;
    public void DestroyWeapon()
    {
        weapon = null;
    }
    
    public void HideWeapon()
    {
        if (weaponDeathrattleSprite.enabled) StartCoroutine(board.animationManager.deathrattleAnimWeapon(this));
        hider = StartCoroutine(wepHide());
    }
    IEnumerator wepHide()
    {
        yield return Board.Wait(10);
        board.animationManager.LerpZoom(weaponFrame.gameObject, Vector3.zero, 10, 0.1f);
    }
    public void UpdateText(int hp=-1, int dmg = -1, int arm = -1)
    {
        int xHp = hp == -1 ? health : hp;
        int xDmg = dmg == -1 ? damage : dmg;
        int xArmor = arm == -1 ? armor : arm;

        if (hpText.text!= xHp.ToString())
        {
            StartCoroutine(Creature.txtBounce(hpText));
        }
        if (damageText.text!= xDmg.ToString())
        {
            StartCoroutine(Creature.txtBounce(damageText));
        }
        /*
        if (armorText.text!= xArmor.ToString())
        {
            StartCoroutineCreature.txtBounce(armorText) );
        }
        */
        hpText.text = xHp.ToString();
        damageText.text = xDmg.ToString();

        if (hp < maxHealth) 
            hpText.color = board.minionObject.GetComponent<Creature>().redText;
        else 
            hpText.color = board.minionObject.GetComponent<Creature>().baseText;

        if (xDmg <= 0)
        {
            board.animationManager.LerpZoom(damageSpriteRenderer.gameObject, Vector3.zero, 10);
        }
        else
        {
            board.animationManager.LerpZoom(damageSpriteRenderer.gameObject, Vector3.one, 10, 0.1f);
        }
    }

    public void UpdateWeaponText(int dmg = -1, int dura = -1)
    {
        int xDura = dura;
        int xDmg = dmg;

        if (weaponDurability.text != xDura.ToString())
        {
            StartCoroutine(Creature.txtBounce(weaponDurability));
        }
        if (weaponDamage.text != xDmg.ToString())
        {
            StartCoroutine(Creature.txtBounce(weaponDamage));
        }

        weaponDurability.text = xDura.ToString();
        weaponDamage.text = xDmg.ToString();

        if (weapon == null) return;

        if (xDura < weapon.sentinel.baseHealth)
            weaponDurability.color = board.minionObject.GetComponent<Creature>().redText;
        else if (xDura > weapon.sentinel.baseHealth)
            weaponDurability.color = board.minionObject.GetComponent<Creature>().greenText;
        else
            weaponDurability.color = board.minionObject.GetComponent<Creature>().baseText;


        if (xDmg > weapon.sentinel.baseDamage)
            weaponDamage.color = board.minionObject.GetComponent<Creature>().greenText;
        else
            weaponDamage.color = board.minionObject.GetComponent<Creature>().baseText;
    }

    public void CheckTriggers()
    {
        if (weapon == null) return;
        if (weapon.triggers.Count > 0)
        {
            bool d = false;
            foreach (Trigger t in weapon.triggers)
            {
                if (t.type == Trigger.Type.Deathrattle)
                {
                    weaponDeathrattleSprite.enabled = true;
                    d = true;
                }
            }
            if (d == false)
                weaponTriggerSprite.enabled = true;
        }
        else
        {
            weaponTriggerSprite.enabled = false;
            weaponDeathrattleSprite.enabled = false;
        }
    }

    public void Highlight(bool target=false)
    {
        if (isElevated) return;
        highlight.enabled = true;
        if (target) highlight.sprite = highlightTarget;
        else highlight.sprite = highlightNormal;
    }

    public void Unhighlight()
    {
        highlight.enabled = false;
    }
    public SpriteRenderer skull;
    public void ShowSkull()
    {
        skull.enabled = true;
        skull.transform.localScale = Vector3.one * 1.5f;
        board.animationManager.LerpZoom(skull.gameObject, Vector3.one * 2, 5, 0.3f);
    }
    public void HideSkull()
    {
        skull.enabled = false;
    }
    void Update()
    {
        
    }


    private void OnMouseOver()
    {
        if (board.targeting && board.dragTargeting)
        {
            if (Input.GetMouseButtonUp(0))
            {
                if (board.targetingHero == this)
                {
                    //cancel by releasing on self
                    board.EndTargeting(true);
                    return;
                }
                board.TargetHero(this);
            }
        }
    }

    Vector3 clickPos = Vector3.zero;
    private void OnMouseDown()
    {
        if (board.targeting)
        {
            if (board.targetingHero == this)
            {
                //cancel by clicking on self
                board.EndTargeting(true);
                return;
            }

            board.TargetHero(this);
            return;
        }
        if (this != board.currHero) return;
        if (canAttack == false) return;
        board.StartTargetingSwing(this);
        clickPos = Card.GetMousePos();
    }

    private void OnMouseEnter()
    {
        if (board.targeting && highlight.enabled) //board.targetingHero !=this
        {
            board.ShowSkulls(this);
        }
        board.hoveredHero = this;
    }
    private void OnMouseExit()
    {
        board.hoveredHero = null;

        if (highlight.enabled)
            board.HideSkulls();
    }

    int dragCounter = 0;
    const int dragTime = 8;
    private void OnMouseDrag()
    {

        if (board.currTurn == false) return;
        if (board.disableInput) return;
        if (board.targetingHero == this)
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
    public Hero()
    {

    }
}
