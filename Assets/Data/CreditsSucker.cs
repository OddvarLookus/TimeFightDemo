using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CreditsSucker : MonoBehaviour
{
    int currentCredits = 0;

    [SerializeField] float suckRadius;



    // Update is called once per frame
    void Update()
    {

        SuckCredits();

    }

    void SuckCredits()
    {
        Collider[] collHits;
        collHits = Physics.OverlapSphere(transform.position, suckRadius, ~0, QueryTriggerInteraction.Collide);
        for (int i = 0; i < collHits.Length; i++) 
        {
            if (collHits[i].gameObject.TryGetComponent(out Credit credit))
            {
                credit.Attract(transform);
            }
        }

    }

    public void AddCredits(int _creditsToAdd)
    {
        currentCredits += _creditsToAdd;
        currentCredits = Mathf.Clamp(currentCredits, 0, int.MaxValue);

        GameUIManager.instance.SetCreditsLabel(currentCredits);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, suckRadius);
    }

}
