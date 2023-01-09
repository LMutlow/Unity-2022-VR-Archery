using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Sirenix.OdinInspector;

public class Arrow : XRGrabInteractable
{
    public bool inAir = false; // Keeps track if the arrow is currently in flight
    [SerializeField] private float speed = 1000f; // Speed of the arrow
    
    private Rigidbody rb; // Ref to the rigidbody

    /// <summary>
    /// Gets refrences
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Calculates arrow downwards rotation to get preferred arrow flight 
    /// </summary>
    private void FixedUpdate()
    {
        if (inAir)
        {
            if (rb.velocity.magnitude > 0)
            {
                if (Quaternion.LookRotation(rb.velocity) != Quaternion.Euler(Vector3.zero))
                {
                    rb.rotation = Quaternion.LookRotation(rb.velocity);
                }
            }
        }
    }

    /// <summary>
    /// Uses Physics to fire the arrow
    /// </summary>
    /// <param name="power"></param>
    public void Fire(float power)
    {
        inAir = true;
        interactionLayers = 0;
        Vector3 force = transform.forward * power * speed;
        rb.AddForce(force, ForceMode.Impulse);
    }

    /// <summary>
    /// Removes grabbable and physics components when the object hits the passed GameObject and reparents the arrow to that object to simulate the arrow peicing the hit object
    /// </summary>
    /// <param name="hitObject"></param>
    public void ObjectHit(GameObject hitObject)
    {
        Destroy(GetComponent<XRGrabInteractable>());
        Destroy(GetComponent<Rigidbody>());
        transform.parent = hitObject.transform;
    }
}