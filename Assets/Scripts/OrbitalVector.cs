using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OrbitalVector
{
    public Vector3 baseVector;
    public Vector3 vector;
    public float speed;
    public VectorType type; 

    public enum VectorType {Custom, Circle, InnerEllipse, OuterEllipse, Offset}

    public OrbitalVector(Vector3 vector, float speed) {
        this.baseVector = vector;
        this.vector = vector;
        this.speed = speed;
        this.type = VectorType.Custom;

        this.getNormal();
    }

    public OrbitalVector(Vector3 vector, float speed, VectorType type) {
        this.baseVector = vector;
        this.vector = vector;
        this.speed = speed;
        this.type = type;

        this.getNormal();
    }

    public void fixBase() {
        baseVector = vector;
    }

    public void setTime(double time) {
        double timed = (time % 360.0) * speed;
        vector = Quaternion.AngleAxis((float) timed, this.getNormal()) * baseVector;
        Debug.Log($"{type}: {speed}");
    }

    public Vector3 getNormal() {
        return Vector3.Cross(Vector3.Cross(this.baseVector, Vector3.up), this.baseVector).normalized;
    }
}
