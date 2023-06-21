using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitObject : MonoBehaviour
{
    // Main Field
    public OrbitObject parent;
    public List<OrbitalVector> vectors = new List<OrbitalVector>();
    private Vector3 position = new Vector3(0,0,0);

    // Enum Field
    public enum OrbitType {circle, ellipse, custom}
    public OrbitType orbit;

    // Circle Field
    public float circleSpeed;

    // Ellipse Field
    public float ellipseSpeed;
    public float ellipseWidth;
    public float ellipseOffset;
    public float ellipseRotation;

    // Custom Field
    public bool showVectors = true;

    // Debug Field
    private Vector3 lastPosition;
    public bool changed { 
        get {
            if (this.transform.position != lastPosition) {
                lastPosition = this.transform.position;
                return true;
            }
            return false;
        }
    }

    void Start()
    {
        if (orbit == OrbitType.custom) {
            foreach (OrbitalVector vector in vectors) {
                vector.fixBase();
            }
        }
    }

    public Vector3 getVector() 
    {
        Vector3 total = new Vector3(0, 0, 0);
        foreach (OrbitalVector vec in vectors) {
            total += vec.vector;
        }

        if (parent == null) {
            return total;
            //return vectors + baseVector;
        }
        else {
            return total + parent.getVector();
        }
    }

    public void updatePosition(DateTime date) {
        double time = (date - new DateTime(2003, 1, 26)).TotalSeconds;
        foreach (OrbitalVector vector in vectors) {
            vector.setTime(time);
        }
    }

    public void setPosition() {
        this.transform.GetComponent<Rigidbody>().MovePosition(getVector());
    }
}
