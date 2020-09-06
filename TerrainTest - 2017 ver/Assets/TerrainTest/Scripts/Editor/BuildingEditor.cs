using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor (typeof(Building))]
public class BuildingEditor : Editor {

	public override void OnInspectorGUI ()
	{
		Building baseScript = (Building)target;

		DrawDefaultInspector ();
	}
}
