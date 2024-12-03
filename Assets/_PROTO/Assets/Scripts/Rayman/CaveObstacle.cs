using UnityEngine;

public class CaveObstacle : MonoBehaviour
{
    [System.Flags]
    public enum Setup { Top = 1, Bottom = 2, Item = 4 }

    public GameObject stalactite;
    public GameObject stalagmite;
    public GameObject item;
    public Setup setup;
    public float passageHeight;

    public void Randomize(float minHeight, float maxHeight, float topChance, float bottomChance, float itemChance)
    {
        passageHeight = Random.Range(minHeight, maxHeight);
        setup = 0;
        if (topChance > 0f && Random.Range(0f, 1f) <= topChance) setup |= Setup.Top;
        if (bottomChance > 0f && Random.Range(0f, 1f) <= bottomChance) setup |= Setup.Bottom;
        if (itemChance > 0f && Random.Range(0f, 1f) <= itemChance) setup |= Setup.Item;
        UpdateSetup();
    }

    private void UpdateSetup()
    {
        if (stalactite != null)
        {
            if (setup.HasFlag(Setup.Top))
            {
                stalactite.SetActive(true);
                Vector3 topPos = stalactite.transform.localPosition;
                topPos.y = passageHeight / 2f;
                stalactite.transform.localPosition = topPos;
            }
            else stalactite.SetActive(false);
        }


        if (stalagmite != null)
        {
            if (setup.HasFlag(Setup.Bottom))
            {
                stalagmite.SetActive(true);
                Vector3 botPos = stalagmite.transform.localPosition;
                botPos.y = -passageHeight / 2f;
                stalagmite.transform.localPosition = botPos;
            }
            else stalagmite.SetActive(false);
        }


        if (item != null)
        {
            if (setup.HasFlag(Setup.Item))
            {
                item.SetActive(true);
            }
            else item.SetActive(false);
        }
    }
}
