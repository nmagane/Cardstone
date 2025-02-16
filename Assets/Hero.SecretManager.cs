using System.Collections.Generic;
using UnityEngine;

public partial class Hero
{
    public GameObject secretAnchor;
    public List<SecretDisplay> secrets = new List<SecretDisplay>();
    public void AddSecret(Card.Cardname card, bool noAnim=false)
    {
        SecretDisplay s = Instantiate(secretObject).GetComponent<SecretDisplay>();

        s.hero = this;
        s.Set(card);
        s.transform.localScale = Vector3.zero;
        s.transform.parent = secretAnchor.transform;
        s.init = false;

        secrets.Add(s);

        if (!noAnim) OrderSecrets();
    }
    public SecretDisplay RemoveAt(int x, bool noAnim=false)
    {
        SecretDisplay s = secrets[x];
        secrets.Remove(s);
        if (!noAnim)  OrderSecrets();
        return s;
    }
    public bool HasSecret(Card.Cardname card)
    {
        foreach (SecretDisplay s in secrets)
        {
            if (s.card == card) return true;
        }
        return false;
    }

    float dist = 1f;
    public void OrderSecrets()
    {
        float count = secrets.Count;
        float x = 0; 
        float offset = -((count - 1) / 2f * dist);
        int i = 0;
        foreach (var s in secrets)
        {
            float y = 1.25f;
            if (count==5)
            {
                if (i == 0 || i == 4)
                    y = 0.875f;
            }
            x = offset + dist * i++;
            Vector3 pos = new Vector3(x, y,-1);
            MoveSecret(s, pos);
        }
    }

    public void MoveSecret(SecretDisplay s, Vector3 location)
    {
        if (s.init==false)
        {
            s.transform.localPosition = location;
            board.animationManager.LerpZoom(s.gameObject, Vector3.one, 7, 0.1f);
            s.init = true;
            return;
        }
        float bounce = 0.1f;

        if (Vector3.Distance(s.transform.localPosition, location) < 0.2f)
        {
            bounce = 0;
        }

        board.animationManager.LerpTo(s, location, 5, bounce);
    }
}
