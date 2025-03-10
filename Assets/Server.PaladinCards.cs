public partial class Server
{
    public void Argent_Protector(CastInfo spell)
    {
        if (spell.targetMinion == null) return;
        AddAura(spell.match, spell.targetMinion, new Aura(Aura.Type.Shield));
    }
}
