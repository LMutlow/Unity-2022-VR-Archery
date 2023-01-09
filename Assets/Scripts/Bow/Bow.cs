using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Bow : XRGrabInteractable
{
    public Transform notch; // Ref to the notch component
    
    private BowStringInteraction pullInteraction; // Ref to the bow string grabbable used for the pull interaction
    private LineRenderer lineRenderer; // Ref to the line renderer used for the bow string
    private Vector3 resetPosition = Vector3.zero; // The default position of the bow string

    /// <summary>
    /// Gets refrences for necessary components
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        pullInteraction = GetComponentInChildren<BowStringInteraction>();
        lineRenderer = GetComponentInChildren<LineRenderer>();
        resetPosition = notch.localPosition;
    }

    /// <summary>
    /// Overrides the process interactable function of the XR Grabbable to update the bows string
    /// </summary>
    /// <param name="updatePhase"></param>
    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        base.ProcessInteractable(updatePhase);
        if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
        {
            if (isSelected)
            {
                UpdateBow(pullInteraction.pullAmount);
            }
        }
    }

    /// <summary>
    /// Updates the bow string to follow the users pull interaction
    /// </summary>
    /// <param name="value"></param>
    private void UpdateBow(float value)
    {
        Vector3 linePosition = Vector3.forward * Mathf.Lerp(-0.23f, -0.5f, value);
        notch.localPosition = linePosition;
        lineRenderer.SetPosition(1, linePosition);
    }

    /// <summary>
    /// Sets the pull interactions interaction layer to Default when the bow is selected so now we can pull the bow string
    /// </summary>
    /// <param name="args"></param>
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        pullInteraction.interactionLayers = 1;
    }

    /// <summary>
    /// When the bow is deselected we reset the bow strings position and make the bow string interaction unavailble 
    /// </summary>
    /// <param name="args"></param>
    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        notch.localPosition = resetPosition;
        lineRenderer.SetPosition(1, resetPosition);
        pullInteraction.interactionLayers = 0;

        if (pullInteraction.interactorsSelecting.OfType<HandInteractor>().Any())
        {
            HandInteractor notchHand = pullInteraction.interactorsSelecting[0] as HandInteractor;
            interactionManager.CancelInteractorSelection(notchHand as IXRSelectInteractor);
        }
    }
}
