using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(OrbitObject))]
public class OrbitObjectEditor : Editor {

    public override void OnInspectorGUI()
    {
        if (Application.isEditor && !Application.isPlaying) {
        //base.OnInspectorGUI();
        OrbitObject orbitObject = (OrbitObject)target;

        //Main Options
        orbitObject.parent = EditorGUILayout.ObjectField("Parent", orbitObject.parent, typeof(OrbitObject), true) as OrbitObject;
        orbitObject.orbit = (OrbitObject.OrbitType)EditorGUILayout.EnumPopup("Orbit Type", orbitObject.orbit);
        EditorGUILayout.Space();

        //Other Options
        if (orbitObject.orbit == OrbitObject.OrbitType.circle) CircleOptions(orbitObject);
        if (orbitObject.orbit == OrbitObject.OrbitType.ellipse) EllipseOptions(orbitObject);
        if (orbitObject.orbit == OrbitObject.OrbitType.custom) CustomOptions(orbitObject);

        if (GUI.changed) {
            EditorUtility.SetDirty(orbitObject);
        }
        }
    }

    private static void CircleOptions(OrbitObject orbitObject) {
        orbitObject.circleSpeed = EditorGUILayout.FloatField("Speed", orbitObject.circleSpeed);

        if (GUI.changed || orbitObject.changed) {
            Vector3 vector = orbitObject.transform.position;
            //get distance from sun
            if (orbitObject.parent != null) {
                vector -= orbitObject.parent.transform.position;
            }
            orbitObject.vectors = new List<OrbitalVector> {new OrbitalVector(vector, orbitObject.circleSpeed, OrbitalVector.VectorType.Circle)};
        }
    }

    private static void EllipseOptions(OrbitObject orbitObject) {
        orbitObject.ellipseSpeed = EditorGUILayout.FloatField("Speed", orbitObject.ellipseSpeed);
        orbitObject.ellipseWidth = EditorGUILayout.FloatField("Width", orbitObject.ellipseWidth);
        orbitObject.ellipseOffset = EditorGUILayout.FloatField("Offset", orbitObject.ellipseOffset);

        if (GUI.changed || orbitObject.changed) {
            // Create Main Vector
            Vector3 vector = orbitObject.transform.position;
            //get distance from sun
            if (orbitObject.parent != null) {
                vector -= orbitObject.parent.transform.position;
            }

            // Create Offset Vector
            Vector3 offsetVector = Vector3.ClampMagnitude(vector, orbitObject.ellipseOffset);
            vector -= offsetVector;

            // Create Inner Vector
            Vector3 innerVector = (vector - Vector3.ClampMagnitude(vector, orbitObject.ellipseWidth))/2;

            // Create Outer Vector
            Vector3 outerVector = vector - innerVector;

            orbitObject.vectors = new List<OrbitalVector> {
                new OrbitalVector(offsetVector, 0, OrbitalVector.VectorType.Offset),
                new OrbitalVector(innerVector, -orbitObject.ellipseSpeed, OrbitalVector.VectorType.InnerEllipse),
                new OrbitalVector(outerVector, orbitObject.ellipseSpeed, OrbitalVector.VectorType.OuterEllipse)
            };
        }
    }

    private static void CustomOptions(OrbitObject orbitObject) {
        orbitObject.showVectors = EditorGUILayout.Foldout(orbitObject.showVectors, "Custom", true);

        if (orbitObject.showVectors)
        {
            EditorGUI.indentLevel++;

            List<OrbitalVector> list = orbitObject.vectors;
            int size = Mathf.Max(1, EditorGUILayout.IntField("Amount", list.Count));

            while (size > list.Count)
            {
                list.Add(new OrbitalVector(new Vector3(0, 0, 0), 0, OrbitalVector.VectorType.Custom));
            }

            while (size < list.Count)
            {
                list.RemoveAt(list.Count - 1);
            }

            for (int i = 0; i < list.Count; i++)
            {
                if (i == 0) GUI.enabled = false;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Vector", GUILayout.MaxWidth(55));
                list[i].vector = EditorGUILayout.Vector3Field("", list[i].vector);
                if (i == 0) GUI.enabled = true;
                EditorGUILayout.LabelField("Speed", GUILayout.MaxWidth(55));
                list[i].speed = EditorGUILayout.FloatField("", list[i].speed, GUILayout.MaxWidth(50));
                EditorGUILayout.EndHorizontal();

                if (GUI.changed) {
                    list[i].fixBase();
                }
            }

    	    if (GUI.changed || orbitObject.changed) {
                Vector3 total = new Vector3(0, 0, 0);
                for (int i = 1; i < list.Count; i++) {
                    total += list[i].vector;
                }

                if (orbitObject.parent == null) {
                    list[0].vector = orbitObject.transform.position - total;
                }
                else {
                    list[0].vector = orbitObject.transform.position - (total + orbitObject.parent.transform.position);
                }
                list[0].fixBase();
                list[0].type = OrbitalVector.VectorType.Custom;
            }
            
            orbitObject.vectors = list;
            EditorGUI.indentLevel--;
        }
    }
}


