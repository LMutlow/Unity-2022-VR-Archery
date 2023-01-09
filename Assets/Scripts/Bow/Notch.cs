using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class Notch : MonoBehaviour
{
    [SerializeField] private XRInteractionManager interactionManager = null; // Ref to the interaction manager
    [SerializeField] private GameObject arrowPrefab = null; // Ref to the arrow prefab
    [SerializeField] private GameObject placeHolderArrow = null; // Ref to the placeholder notched arrow
    private BowStringInteraction pullInteraction; // Ref to the bow string pull interaction 
    private bool isArrowKnocked = false; // Checks for the arrow being loaded into the bow 

    /// <summary>
    /// Gets refrences
    /// </summary>
    private void Awake()
    {
        pullInteraction = GetComponent<BowStringInteraction>();
    }

    /// <summary>
    /// Handles the transition from grabbing an arrow and loading it into the bow for the seamless arrow held to bow string being pulled in one motion
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Arrow>() != null && GetComponentInParent<Bow>().isSelected) // Checks if the object is an arrow and the bow component is held
        {
            if (other.GetComponent<Arrow>().interactorsSelecting.OfType<HandInteractor>().Any()) // Checks if theres a hand holding the arrow
            {
                if (!other.GetComponent<Arrow>().inAir && !isArrowKnocked) // Checks if the arrow is in flight or if an arrow is alread loaded
                {
                    // Activates the placeholder arrow
                    placeHolderArrow.SetActive(true);
                    
                    // Force drops the hand holding the arrow and destroys that arrow
                    HandInteractor hand = other.GetComponent<Arrow>().interactorsSelecting[0] as HandInteractor;
                    interactionManager.SelectCancel(hand, other.GetComponent<Arrow>() as IXRSelectInteractable);
                    Destroy(other.gameObject);

                    // Force selects the bow string pull interaction
                    interactionManager.SelectEnter(hand, pullInteraction as IXRSelectInteractable);
                    pullInteraction.OnRelease += ReleaseArrow;
                    pullInteraction.BowNotched();

                    // Sets the bow to loaded
                    isArrowKnocked = true;
                }
            }
        }
    }

    /// <summary>
    /// Spans a new physics arrow in the bows firing position and disables the placeholder arrow and unloads the bow
    /// </summary>
    /// <param name="power"></param>
    private void ReleaseArrow(float power)
    {
        GameObject newArrow = Instantiate(arrowPrefab, placeHolderArrow.transform.position, placeHolderArrow.transform.rotation);
        newArrow.GetComponent<Arrow>().Fire(power);
        placeHolderArrow.SetActive(false);
        isArrowKnocked = false;
    }
}
