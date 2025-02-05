public partial class AnimationManager
{
    [System.Serializable]
    public class AnimationInfo
    {
        Card.Cardname card;

        bool sourceIsHero;
        bool sourceIsFriendly;
        int sourceIndex;

        int target;
        bool targetIsFriendly;
        bool targetIsHero;

    }
    public void StartAnimation(AnimationInfo c)
    {

    }
}
