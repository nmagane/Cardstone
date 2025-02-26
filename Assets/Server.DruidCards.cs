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
}
