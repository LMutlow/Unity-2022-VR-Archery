using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Quiver : XRBaseInteractable
{
    [SerializeField] private GameObject arrowPrefab = null; // Ref to the arrow prefab

    /// <summary>
    /// Overrides awake
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
    }

    /// <summary>
    /// When selected create a new arrow and grab it with the selecting interactor (Hand)
    /// </summary>
    /// <param name="args"></param>
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        CreateArrow(args.interactorObject as XRBaseInteractor);
    }

    /// <summary>
    /// Creates an arrow and force selects it into the hand
    /// </summary>
    /// <param name="hand"></param>
    private void CreateArrow(XRBaseInteractor hand)
    {
        if (hand is HandInteractor)
        {
            if (hand.GetComponent<HandInteractor>().isSelecting)
            {
                GameObject newArrow = Instantiate(arrowPrefab, transform.position, Quaternion.identity);
                interactionManager.SelectEnter(hand as IXRSelectInteractor, newArrow.GetComponent<Arrow>());
            }
        }
    }
}