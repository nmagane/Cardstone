using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDisplay : MonoBehaviour
{
    public Hero hero;

    private void OnMouseEnter()
    {
        if (hero.weaponActive) return;
        hero.PopWeaponDamage();
    }
    private void OnMouseExit()
    {
        hoverTimer = 0;
        hero.board.HideHoverTip();
        if (hero.weaponActive) return;
        hero.HideWeaponDamage();
    }

    int hoverTimer = 0;
    private void OnMouseOver()
    {
        if (hero.weapon == null) return;
        if (hoverTimer < 30)
        {
            hoverTimer++;
            if (hoverTimer == 30)
                hero.board.ShowHoverTip(hero.weaponFrame.gameObject, hero.weapon.card);
        }
    }
}
