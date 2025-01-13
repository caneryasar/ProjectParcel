using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventArchive : MonoBehaviour {


    #region Input
    
    public event Action<Vector2> OnMoveInput;
    public event Action<bool> OnJumpInput;
    public event Action<bool> OnWheelieInput;

    public void InvokeOnMoveInput(Vector2 direction) { OnMoveInput?.Invoke(direction); }
    public void InvokeOnJumpInput(bool pressed) { OnJumpInput?.Invoke(pressed); }
    public void InvokeOnWheelieInput(bool pressed) { OnWheelieInput?.Invoke(pressed); }

    #endregion
    
    public event Action OnDeliveryPickup, OnDeliveryDropoff;
    public event Action<Transform> OnCheckpointChange;
    public event Action OnGameStart;
    public event Action OnGameOver;
    public event Action OnGettingReadyToStart;
    
    
    public void InvokeOnDeliveryPickup() { OnDeliveryPickup?.Invoke(); }
    public void InvokeOnGameStart() { OnGameStart?.Invoke(); }
    
    public void InvokeOnGameOver() { OnGameOver?.Invoke(); }
    public void InvokeOnGettingReadyToStart() { OnGettingReadyToStart?.Invoke(); }
    public void InvokeOnDeliveryDropoff() { OnDeliveryDropoff?.Invoke(); }
    public void InvokeOncheckpointChange(Transform newPoint) { OnCheckpointChange?.Invoke(newPoint); }
    
}

public class InputEvents {

}