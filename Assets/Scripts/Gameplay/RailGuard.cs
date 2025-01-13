using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Plugins.Core.PathCore;
using UnityEngine;
using UnityEngine.Splines;

public class RailGuard : MonoBehaviour {

    public SplineContainer splineContainer;

    private Spline _spline;


    private void Awake() {

        splineContainer = GetComponentInChildren<SplineContainer>();

        _spline = splineContainer.Splines[0];



    }


    // Start is called before the first frame update
    void Start() {
        
        
    }

    // Update is called once per frame
    void Update() {
        
        
    }
}