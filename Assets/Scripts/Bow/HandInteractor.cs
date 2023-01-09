using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class HandInteractor : XRDirectInteractor
{
    [SerializeField] private HandType handType = HandType.RIGHT; // Is the hand a left or right hand
    private XRIDefaultInputActions input = null; // Class ref for input controlls

    public bool isSelecting { get; private set; } = false; // Checks if the hand isSelecting an object or not

    /// <summary>
    /// Gets refrences
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        input = new XRIDefaultInputActions();
    }

    /// <summary>
    /// Subscribes to input events
    /// </summary>
    protected override void OnEnable()
    {
        base.OnEnable();
        input.Enable();

        if (handType == HandType.RIGHT)
        {
            input.XRIRightHandInteraction.Select.performed += OnSelectPerformed;
            input.XRIRightHandInteraction.Select.canceled += OnSelectCancelled;
        }
        else 
        {
            input.XRILeftHandInteraction.Select.performed += OnSelectPerformed;
            input.XRILeftHandInteraction.Select.canceled += OnSelectCancelled;
        }

    }

    /// <summary>
    /// Unsubscribes to input events
    /// </summary>
    protected override void OnDisable()
    {
        base.OnDisable();
        input.Disable();

        if (handType == HandType.RIGHT)
        {
            input.XRIRightHandInteraction.Select.performed -= OnSelectPerformed;
            input.XRIRightHandInteraction.Select.canceled -= OnSelectCancelled;
        }
        else
        {
            input.XRILeftHandInteraction.Select.performed -= OnSelectPerformed;
            input.XRILeftHandInteraction.Select.canceled -= OnSelectCancelled;
        }
    }

    /// <summary>
    /// Sets the iSelecting var to true when the hand begins interacting with an interacable
    /// </summary>
    /// <param name="callbackContext"></param>
    private void OnSelectPerformed(InputAction.CallbackContext callbackContext)
    {
        isSelecting = true;
    }

    /// Sets the iSelecting var to false when the hand stops interacting with an interacable
    private void OnSelectCancelled(InputAction.CallbackContext callbackContext) 
    {
        isSelecting = false;

    }
}

/// <summary>
/// The hand type enum
/// </summary>
public enum HandType
{
    LEFT,
    RIGHT
}