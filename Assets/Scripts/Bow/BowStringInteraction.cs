using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class BowStringInteraction : XRBaseInteractable
{
    public float pullAmount { get; private set; } = 0.0f; // The current pull amount of the pull interaction
    public Transform start, end; // The start and end transforms of the pull interaction
    public Action<float> OnRelease; // Event used when the bow string is released
    [SerializeField] Color restingColour = Color.white; // The default colour of the bow string
    [SerializeField] Color tensingColour = Color.white; // The tension colour of the bow string when its fully drawn back
    [SerializeField] float bowEmissiveIntensityTensing = 3f; // The intensity of the glow effect when the bow string is fully drawn back
    [SerializeField] Material bowStringMaterial = null; // The material of the bow string

    private XRBaseInteractor pullingInteractor = null; // Ref to the current pulling interaction (The Hand pulling the bow)
    private Notch notch = null; // Ref to the notch component
    private Bow bow; // Ref to the bow component
    private Vector4 toEmissiveColour = Color.white; // Temp colour as a vec 4 to circumvent a unity bug with emission intensity
    private float emisssiveIntensity = 0; // Temp value for the lerped emission intensity
    private bool isNotched = false; // Checks if the bow currently has an arrow loaded

    /// <summary>
    /// Gets references
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        bow = GetComponentInParent<Bow>();
        notch = GetComponent<Notch>();
    }

    /// <summary>
    /// Resets colour on disable
    /// </summary>
    protected override void OnDisable()
    {
        base.OnDisable();
        ResetColour();
    }

    /// <summary>
    /// Sets the hand interactor to the pulling interactor var when the pull interaction begins
    /// </summary>
    /// <param name="args"></param>
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        pullingInteractor = args.interactorObject as XRBaseInteractor;
    }

    /// <summary>
    /// Fires the bow / resets materials / checks if the bow was drawn far enough to fire when the hand is released
    /// </summary>
    /// <param name="args"></param>
    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        if (GetComponentInParent<Bow>().isSelected)
        {
            if (pullAmount > 0.25f)
            {
                OnRelease?.Invoke(pullAmount);
                OnRelease = null;
                isNotched = false;
            }
        }
        pullingInteractor = null;
        pullAmount = 0f;
        ResetColour();
    }

    /// <summary>
    /// Update function for the pull interactor
    /// </summary>
    /// <param name="updatePhase"></param>
    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        base.ProcessInteractable(updatePhase);
        if(updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
        {
            if(isSelected && GetComponentInParent<Bow>().isSelected) 
            {
                Vector3 pullPosition = pullingInteractor.transform.position;
                pullAmount = CalculatePull(pullPosition);
                AlterLineEmissive(pullAmount);
            }
        }
    }

    /// <summary>
    /// Calculates the pull interactor distance and returns a normalized value for power ranging from 0 - 1
    /// </summary>
    /// <param name="pullPosition"></param>
    /// <returns></returns>
    private float CalculatePull(Vector3 pullPosition)
    {
        Vector3 pullDirection = pullPosition - start.position;
        Vector3 targetDirection = end.position - start.position;
        float maxLength = targetDirection.magnitude;

        targetDirection.Normalize();
        float pullValue = Vector3.Dot(pullDirection, targetDirection) / maxLength;
        
        return Mathf.Clamp(pullValue, 0, 1);
    }
    /// <summary>
    /// Alters the emmisive property of the bow string
    /// </summary>
    /// <param name="lerpValue"></param>
    private void AlterLineEmissive(float lerpValue)
    {
        if (isNotched)
        {
            emisssiveIntensity = Mathf.Lerp(0, bowEmissiveIntensityTensing, pullAmount);
            toEmissiveColour = Color.Lerp(restingColour, tensingColour, pullAmount);
            toEmissiveColour = toEmissiveColour * Mathf.Pow(2, emisssiveIntensity);
            bowStringMaterial.SetColor("_EmissionColor", toEmissiveColour);
        }
    }

    /// <summary>
    /// Resets the colour of the material
    /// </summary>
    private void ResetColour()
    {
        Vector4 resetColor = restingColour * Mathf.Pow(2, 0);
        bowStringMaterial.SetColor("_EmissionColor", resetColor);
    }

    /// <summary>
    /// Sets the bow as notched
    /// </summary>
    public void BowNotched()
    {
        isNotched = true;
    }
}
