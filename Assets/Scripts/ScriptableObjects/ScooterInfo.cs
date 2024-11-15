using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScooterInfo", menuName = "Scooter")]
public class ScooterInfo : ScriptableObject {

    public Transform rider;
    public Transform body;
    public Transform frontWheel;
    public Transform rearWheel;

}