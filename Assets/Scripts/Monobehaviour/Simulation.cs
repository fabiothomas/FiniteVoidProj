using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Simulation : MonoBehaviour
{
    private OrbitObject[] orbitObjects;

    void Start() {
        orbitObjects = FindObjectsOfType(typeof(OrbitObject)) as OrbitObject[];
    }

    void Update() {
        foreach (OrbitObject obj in orbitObjects) {
            obj.updatePosition(DateTime.UtcNow);
        }
    }

    void FixedUpdate() {
        foreach (OrbitObject obj in orbitObjects) {
            obj.setPosition();
        }
    }
}
