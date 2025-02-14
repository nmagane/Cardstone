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

        spriteRenderer.sortingLayerName = x;
        skull.sortingLayerName = x;
        highlight.sortingLayerName = x;
        damageSpriteRenderer.sortingLayerName = x;
        armorSpriteRenderer.sortingLayerName = x;
        damageText.GetComponent<MeshRenderer>().sortingLayerName = x;
        hpText.GetComponent<MeshRenderer>().sortingLayerName = x;
        armorText.GetComponent<MeshRenderer>().sortingLayerName = x;

        shadow.sortingLayer = s;
        shadow.spriteRenderer.sortingLayerName = s;
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
    public bool combo=false;
    public Weapon weapon;

    public SpriteRenderer spriteRenderer;
    public SpriteRenderer damageSpriteRenderer;
    public SpriteRenderer armorSpriteRenderer;

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
    public TMP_Text armorText;

    public SpriteRenderer highlight;

    public Sprite highlightNormal;
    public Sprite highlightArmor;
    public Sprite highlightTarget;
    public Sprite hightlightTargetArmor;

    public Sprite weaponDisabled;
    public Sprite weaponEnabled;

    public SpriteRenderer weaponFrame;
    public SpriteRenderer weaponArt;
    public SpriteRenderer weaponDamageSprite;
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

    public bool weaponActive = true;
    public void DisableWeapon()
    {
        weaponActive = false;
        weaponFrame.sprite = weaponDisabled;

        weaponArt.enabled = false;
        weaponDamage.transform.localScale = Vector3.zero;

        board.animationManager.LerpZoom(damageSpriteRenderer.gameObject, Vector3.zero, 5);
    }    

    public void EnableWeapon()
    {
        weaponActive = true;
        weaponFrame.sprite = weaponEnabled;

        weaponDamageSprite.enabled = false;
        weaponArt.enabled = true; 
        board.animationManager.LerpZoom(weaponDamage.gameObject, Vector3.one, 5, 0.2f);
        if (weaponFrame.transform.localScale !=Vector3.zero) board.animationManager.BounceZoom(weaponFrame.gameObject, 0.1f);
        //board.animationManager.LerpZoom(damageSpriteRenderer.gameObject, Vector3.zero, 10);
    }

    Coroutine popper = null;
    public void PopWeaponDamage()
    {
        weaponDamageSprite.enabled = true;
        popper = board.animationManager.LerpZoom(weaponDamage.gameObject, Vector3.one, 5, 0.2f);
    }
    public void HideWeaponDamage()
    {
        if (popper != null) StopCoroutine(popper);
        weaponDamageSprite.enabled = false;
        weaponDamage.transform.localScale = Vector3.zero;
    }

    public void DisplayWeapon()
    {
        newWep = true;
        if (weapon!=null)
        {
        //    HideWeapon();
        }
        
        if (hider!=null) StopCoroutine(hider);
        weaponDamage.text = weapon.damage.ToString();
        weaponDurability.text = weapon.durability.ToString();

        CheckTriggers();
        weaponArt.sprite = board.cardObject.GetComponent<Card>().cardSprites[(int)weapon.card];

        DropWeapon(8);
        //weaponFrame.transform.localScale = Vector3.one;
    }
    public bool newWep = false;
    Coroutine hider = null;
    public void DestroyWeapon()
    {
        weapon = null;
    }
    
    public void HideWeapon()
    {
        if (newWep)
        {
            newWep = false;
            return;
        }
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
        
        if (armorText.text!= xArmor.ToString())
        {
            StartCoroutine(Creature.txtBounce(armorText));
        }
        
        hpText.text = xHp.ToString();
        damageText.text = xDmg.ToString();
        armorText.text = xArmor.ToString();

        if (hp < maxHealth) 
            hpText.color = board.minionObject.GetComponent<Creature>().redText;
        else 
            hpText.color = board.minionObject.GetComponent<Creature>().baseText;

        if (weaponActive)
        {
            if (xDmg <= 0)
            {
                board.animationManager.LerpZoom(damageSpriteRenderer.gameObject, Vector3.zero, 5);
            }
            else
            {
                board.animationManager.LerpZoom(damageSpriteRenderer.gameObject, Vector3.one, 5, 0.2f);
            }
        }

        if (xArmor <= 0)
        {
            board.animationManager.LerpZoom(armorSpriteRenderer.gameObject, Vector3.zero, 5);
        }
        else
        {
            board.animationManager.LerpZoom(armorSpriteRenderer.gameObject, Vector3.one, 5, 0.2f);
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
        if (target)
        {
            highlight.sprite = armor>0? hightlightTargetArmor : highlightTarget;
        }
        else
        {
            highlight.sprite = armor>0? highlightArmor : highlightNormal;
        }
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

    public void DropWeapon(int delay = 10)
    {
        Vector3 framePos = new Vector3(-4.375f, -0.0625f, 0.230000004f);
        weaponFrame.transform.localPosition = framePos + new Vector3(0, 3);
        //c.shadow.elevation = 2;
        if (delay > 0)
        {
            weaponFrame.transform.localScale = Vector3.zero;
            board.animationManager.DelayedDropWeapon(delay, this);
            return;
        }
        //if (board.playerID == 100) Debug.Log("Dropping minion " + c.minion.card);
        weaponFrame.transform.localScale = Vector3.one * 1.15f;
        int F = 10;
        board.animationManager.LerpTo(weaponFrame.gameObject, framePos, F);
        if (dropper != null) StopCoroutine(dropper);
        dropper = StartCoroutine(_dropper(F));
        board.animationManager.LerpZoom(weaponFrame.gameObject, Vector3.one, F, 0);
    }
    public DropShadow weaponShadow;
    Coroutine dropper = null;
    IEnumerator _dropper(float f)
    {
        float e = weaponShadow.elevation;
        for (float i = 0; i < f; i++)
        {
            weaponShadow.elevation -= e / f;
            yield return Board.Wait(1);
        }
    }
}
