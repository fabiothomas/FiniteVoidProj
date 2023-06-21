using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PathRenderer : MonoBehaviour
{
    [Header("Selection: ")]
    public bool Vectors;
    public bool Orbits;
    public bool Paths;
    public int PathSteps;

    private OrbitObject[] objects;

    void Update()
    {
        objects = FindObjectsOfType<OrbitObject>();
        if (Vectors) { renderVectors(); }
        if (Orbits) { renderOrbits(); }
        if (Paths) { renderPaths(); }
    }

    // Blue
    public void renderVectors() {
        foreach (OrbitObject obj in objects) {

            Vector3[] drawPoints = new Vector3[obj.vectors.Count + 1];

            if (obj.parent != null) {
                drawPoints[0] = obj.parent.transform.position;
            }
            else {
                drawPoints[0] = new Vector3(0, 0, 0);
            }

            for (int i = 0; i < obj.vectors.Count; i++) {
                drawPoints[i+1] = drawPoints[i] + obj.vectors[i].vector;
            }

            // Draw
            for (int i = 0; i < drawPoints.Length - 1; i++) {
                Color color = Color.white;
                if (obj.vectors[i].type == OrbitalVector.VectorType.Circle) { color = new Color(0, 0, 1); }
                if (obj.vectors[i].type == OrbitalVector.VectorType.Offset) { color = new Color(0.5f, 0.5f, 1); }
                if (obj.vectors[i].type == OrbitalVector.VectorType.InnerEllipse) { color = new Color(0.25f, 0.25f, 1); }
                if (obj.vectors[i].type == OrbitalVector.VectorType.OuterEllipse) { color = new Color(0.75f, 0.75f, 1); }

                Debug.DrawLine(drawPoints[i], drawPoints[i+1], color);
            }
        }
    }

    //Green
    public void renderOrbits() {
        foreach (OrbitObject obj in objects) {

            Vector3 prev = new Vector3(0, 0, 0);
            if (obj.parent != null ) {
                prev = obj.parent.transform.position;
            }
            foreach (OrbitalVector vector in obj.vectors) {
                Vector3 vect = vector.vector + prev;
                Vector3[] drawPoints = new Vector3[360];

                for (int i = 0; i < drawPoints.Length; i++) {
                    drawPoints[i] = (Quaternion.Euler(0, i, 0) * vector.vector) + prev;
                }

                // Draw
                for (int i = 0; i < drawPoints.Length - 1; i++) {
                    Color color = Color.green;
                    if (vector.type == OrbitalVector.VectorType.Circle) { color = new Color(0, 1, 0); }
                    if (vector.type == OrbitalVector.VectorType.Offset) { color = new Color(0.25f, 1, 0.25f); }
                    if (vector.type == OrbitalVector.VectorType.InnerEllipse) { color = new Color(0.25f, 1, 0.25f); }
                    if (vector.type == OrbitalVector.VectorType.OuterEllipse) { color = new Color(0.25f, 1, 0.25f); }

                    Debug.DrawLine(drawPoints[i], drawPoints[i+1], color);
                }

                prev = vect;
            }
        }
    }

    // Red
    public void renderPaths() {
        foreach (OrbitObject obj in objects) {

            Vector3[] drawPoints = new Vector3[PathSteps];

            for (int i = 0; i < PathSteps; i++) {
                Vector3 pos = new Vector3(0, 0, 0);
                if (obj.parent != null) {
                    pos = obj.parent.transform.position;
                }
                foreach (OrbitalVector vector in obj.vectors) {
                    pos += Quaternion.AngleAxis(i * vector.speed * 0.01f, vector.getNormal()) * vector.vector;
                }
                drawPoints[i] = pos;
            }

            // Draw
            for (int i = 0; i < drawPoints.Length - 1; i++) {
                Color color = Color.red;

                Debug.DrawLine(drawPoints[i], drawPoints[i+1], color);
            }
        }
    }
}
