using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Hero : MonoBehaviour
{
    public Board board;

    public int health = 30;
    public int maxHealth = 30;

    public int damage = 0;
    public int armor = 0;
    
    public Weapon weapon;

    public SpriteRenderer spriteRenderer;
    public SpriteRenderer damageSpriteRenderer;

    public TMP_Text hpText;
    public TMP_Text damageText;

    public SpriteRenderer highlight;

    public Sprite highlightNormal;
    public Sprite highlightTarget;

    public SpriteRenderer weaponFrame;
    public SpriteRenderer weaponArt;
    public TMP_Text weaponDamage;
    public TMP_Text weaponDurability;

    public void SetHealth(int x)
    {
        health = x;
    }

    public void EquipWeapon(Card.Cardname c)
    {
        weapon = new Weapon(c);
        //DisplayWeapon();
    }

    public void DisplayWeapon()
    {
        //drop anim like creatures TODO
        weaponFrame.transform.localScale = Vector3.one;
        weaponDamage.text = weapon.damage.ToString();
        weaponDurability.text = weapon.durability.ToString();

        weaponArt.sprite = board.cardObject.GetComponent<Card>().cardSprites[(int)weapon.card];
    }

    public void DestroyWeapon()
    {
        weapon = null;
    }

    public void HideWeapon()
    {
        board.animationManager.LerpZoom(weaponFrame.gameObject, Vector3.zero, 10, 0.1f);
    }

    public void UpdateText(int hp=-1, int dmg = -1, int arm = -1)
    {
        int xHp = hp == -1 ? health : hp;
        int xDmg = hp == -1 ? damage : dmg;
        int xArmor = hp == -1 ? armor : arm;

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
            StartCoroutineCreature.txtBounce(armorText));
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

    public void Highlight(bool target=false)
    {
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
                    board.EndTargeting();
                    return;
                }
                board.TargetHero(this);
            }
        }
    }

    private void OnMouseDown()
    {
        if (board.targeting)
        {
            if (board.targetingHero == this)
            {
                //cancel by clicking on self
                board.EndTargeting();
                return;
            }

            board.TargetHero(this);
            return;
        }
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
    public Hero()
    {

    }
}
