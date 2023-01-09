using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowTip : MonoBehaviour
{
    private Arrow arrow = null; // Ref to the arrow component

    /// <summary>
    /// Gets the ref to the arrow component
    /// </summary>
    private void Awake()
    {
        arrow = GetComponentInParent<Arrow>();  
    }

    /// <summary>
    /// Arrow tips on trigger enter used to detect when the arrow tip has hit an object
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != arrow.gameObject && arrow.inAir)
        {
            arrow.ObjectHit(other.gameObject);
            if(other.GetComponentInParent<Rigidbody>())
            {
                other.GetComponentInParent<Rigidbody>().AddForce(GetComponentInParent<Rigidbody>().velocity, ForceMode.Impulse);
            }
        }
    }
}