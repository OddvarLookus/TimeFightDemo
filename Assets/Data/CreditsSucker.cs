using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CreditsSucker : MonoBehaviour
{
    int currentCredits = 0;
    [SerializeField] float minSuckRadius, maxSuckRadius;
    [SerializeField] float maxPlayerSpeed;

    float suckRadius;

    PlayerController playerController;
    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSuckZone();
        SuckCredits();

    }
    void UpdateSuckZone()
    {
        float t = playerController.GetVelocity().magnitude / maxPlayerSpeed;
        suckRadius = Mathf.Lerp(minSuckRadius, maxSuckRadius, t);
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, minSuckRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxSuckRadius);
    }

}
