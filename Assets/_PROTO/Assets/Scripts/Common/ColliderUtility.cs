using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderUtility : MonoBehaviour
{
    public int triggerEnterCount;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        triggerEnterCount++;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        triggerEnterCount--;
    }
}
