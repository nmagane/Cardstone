public partial class Server
{
    void Wrath(CastInfo spell)
    {
        if (spell.choice == 0)
        {
            DamageTarget(3, spell);
        }
        if (spell.choice == 1)
        {
            DamageTarget(1, spell);
            spell.match.ResolveTriggerQueue(ref spell);
            Draw(spell.player);
        }
    }
    
    void Ancient_of_War(CastInfo spell)
    {
        if (spell.choice == 0)
        {
            AddAura(spell.match, spell.minion, new Aura(Aura.Type.Damage, 5));
        }
        if (spell.choice == 1)
        {
            AddAura(spell.match, spell.minion, new Aura(Aura.Type.Health, 5));
            AddAura(spell.match, spell.minion, new Aura(Aura.Type.Taunt));
        }
    }
    void Keeper_of_the_Grove(CastInfo spell)
    {
        if (spell.choice == 0)
        {
            DamageTarget(2, spell);
        }
        if (spell.choice == 1)
        {
            SilenceMinion(spell);
            TransformMinion(spell.match, spell.minion, Card.Cardname.Sheep);
        }
    }
}
