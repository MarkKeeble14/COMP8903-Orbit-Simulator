using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RocketEngine))]
public class RocketEngineControl : MonoBehaviour
{
    private RocketEngine engine;
    [SerializeField] private KeyCode controlEnable = KeyCode.Space;
    [SerializeField] private KeyCode controlThrustPercentDown = KeyCode.LeftControl;
    [SerializeField] private KeyCode controlThrustPercentUp = KeyCode.LeftShift;
    [SerializeField] private KeyCode controlRefuel = KeyCode.R;

    [SerializeField] private bool allowTurnEngineOff;
    [SerializeField] private float adjustThrustPercentRate = 1f;
    [SerializeField] private float rotationSpeed = 5.0f;

    [SerializeField] private StateTypeDisplay rocketEnabledDisplay;

    private void Awake()
    {
        // 
        engine = GetComponent<RocketEngine>();

        if (engine.Active)
            rocketEnabledDisplay.On();
        else
            rocketEnabledDisplay.Off();
    }

    // Update is called once per frame
    void Update()
    {
        // On/Off
        if (Input.GetKeyDown(controlEnable))
        {
            if (allowTurnEngineOff)
            {
                engine.Active = !engine.Active;
            }
            else
            {
                engine.Active = true;
            }
            if (engine.Active)
                rocketEnabledDisplay.On();
            else
                rocketEnabledDisplay.Off();
        }

        // Thrust
        if (Input.GetKey(KeyCode.LeftControl))
        {
            engine.AdjustThrustPercent(Time.deltaTime * -adjustThrustPercentRate);
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            engine.AdjustThrustPercent(Time.deltaTime * adjustThrustPercentRate);
        }

        // Fuel
        if (Input.GetKey(controlRefuel))
        {
            engine.FullyRefuel();
        }

        // Rotation
        if (Input.GetKey(KeyCode.W))
        {
            engine.AdjustRotation(RotType.PITCH, Time.deltaTime * rotationSpeed);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            engine.AdjustRotation(RotType.PITCH, Time.deltaTime * -rotationSpeed);
        }
        // 
        if (Input.GetKey(KeyCode.A))
        {
            engine.AdjustRotation(RotType.YAW, Time.deltaTime * rotationSpeed);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            engine.AdjustRotation(RotType.YAW, Time.deltaTime * -rotationSpeed);
        }
        //
        if (Input.GetKey(KeyCode.Q))
        {
            engine.AdjustRotation(RotType.ROLL, Time.deltaTime * rotationSpeed);
        }
        else if (Input.GetKey(KeyCode.E))
        {
            engine.AdjustRotation(RotType.ROLL, Time.deltaTime * -rotationSpeed);
        }
    }
}
