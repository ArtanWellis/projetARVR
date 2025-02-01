using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerVetement : MonoBehaviour
{
    public manageTenueV2 manageTenue; // Référence au script attaché au Canvas

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("triggerVetement"))
        {
            Debug.Log("Player est dans le trigger");
            manageTenue.SetIsInTrigger(true); // Indique que le Player est dans le trigger
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("triggerVetement"))
        {
            Debug.Log("Player est sorti du trigger");
            manageTenue.SetIsInTrigger(false); // Indique que le Player est sorti du trigger
        }
    }
}
