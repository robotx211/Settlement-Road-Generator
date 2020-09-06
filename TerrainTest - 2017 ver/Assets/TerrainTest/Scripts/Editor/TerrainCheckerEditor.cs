using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor (typeof(TerrainChecker))]
public class TerrainCheckerEditor : Editor
{

	public override void OnInspectorGUI ()
	{
		TerrainChecker baseScript = (TerrainChecker)target;

		//to show list of buildings
		DrawDefaultInspector ();

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		//2 modes
		string[] modeChoices = new string[2];
		modeChoices [0] = "Building";
		modeChoices [1] = "Road";
		baseScript.mode = EditorGUILayout.Popup ("Mode", baseScript.mode, modeChoices);

		if (baseScript.mode == 0) {
			//if in building mode

			string[] buildingChoices = new string[baseScript.buildingList.Count];
			for (int i = 0; i < baseScript.buildingList.Count; i++) {

				buildingChoices [i] = baseScript.buildingList [i].name;

			}
			baseScript.chosenBuilding = EditorGUILayout.Popup ("\tBuilding to Place", baseScript.chosenBuilding, buildingChoices);

			EditorGUILayout.LabelField ("\tBuilding Size: " + baseScript.buildingList [baseScript.chosenBuilding].GetComponent<Building> ().getBaseSize ());

			//baseScript.acceptableSteepness = EditorGUILayout.Slider ("\tAcceptable Steepness", baseScript.acceptableSteepness, 0, 90);

		} else if (baseScript.mode == 1) {
			//if in road mode

			if (baseScript.fullRoadWidth < 0.0f) {
				baseScript.fullRoadWidth = 0.0f;
			}

			if (baseScript.maxRoadWidth < baseScript.fullRoadWidth) {
				baseScript.maxRoadWidth = baseScript.fullRoadWidth;
			}

			baseScript.fullRoadWidth = EditorGUILayout.FloatField ("Full Road Width: ", baseScript.fullRoadWidth);
			baseScript.maxRoadWidth = EditorGUILayout.FloatField ("Max Road Width: ", baseScript.maxRoadWidth);

			baseScript.maxRoadSteepness = EditorGUILayout.Slider ("Max Road Steepness: " ,baseScript.maxRoadSteepness, 0.0f, 90.0f);
		}

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Reset Terrain Specific Values")) {

			baseScript.ResetTerrainSpecificValues ();

		}

		baseScript.roadTextureIndex = EditorGUILayout.IntSlider ("Road Texture Index :", baseScript.roadTextureIndex, 0, baseScript.terrainTexCount - 1);

		EditorGUILayout.LabelField ("Note - Only 2 textures can be blended together");

		for (int i = 0; i < baseScript.terrainTexCount - 1; i++) {
			baseScript.textureSteepnessRules [i].minAngle = EditorGUILayout.FloatField ("Texture " + (i + 1) + " Min Steepness Bounds", baseScript.textureSteepnessRules [i].minAngle);
			baseScript.textureSteepnessRules [i].maxAngle = EditorGUILayout.FloatField ("Texture " + (i + 1) + " Max Steepness Bounds", baseScript.textureSteepnessRules [i].maxAngle);
			EditorGUILayout.MinMaxSlider ("Texture " + (i + 1) + " Steepness Bounds", ref baseScript.textureSteepnessRules [i].minAngle, ref baseScript.textureSteepnessRules [i].maxAngle, 0.0f, 90.0f);
		}


		if (GUILayout.Button ("Update Steepness Texture")) {

			baseScript.UpdateTerrainSteepnessTexture (baseScript.specifiedTerrain);

		}

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Generate New Settlement Data"))
		{
			baseScript.GenerateNewSettlementData ();
		}

		using (new EditorGUI.DisabledScope(baseScript.HasSettlementData () == false))
		{
//			if (GUILayout.Button ("Write Data To CSV"))
//			{
//				baseScript.WriteDataToCSV ();
//			}
			if (GUILayout.Button ("Clear Settlement Data"))
			{
				baseScript.ClearSettlementData ();
			}
		}

		EditorGUILayout.Space ();

		if (GUILayout.Button ("Apply Road Textures From Settlement Data"))
		{
			baseScript.ApplyRoadsFromData ();
		}
		if (GUILayout.Button ("Apply Buildings From Settlement Data"))
		{
			baseScript.ApplyBuildingsFromData ();
		}




		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		using (new EditorGUI.DisabledScope(baseScript.HasSettlementData () == false))
		{
//			if (GUILayout.Button ("Try Random Spawn Building"))
//			{
//				baseScript.TryRandomSpawnBuilding();
//			}
			if (GUILayout.Button ("Random Spawn Building"))
			{
				baseScript.RandomSpawnBuilding();
			}
		}

		using (new EditorGUI.DisabledScope(baseScript.unconnectedBuildings.Count == 0))
		{
			if (GUILayout.Button ("Try Connect Buildings To Road"))
			{
				baseScript.TryConnectBuildings();
			}

		}

		using (new EditorGUI.DisabledScope (baseScript.unconnectedBuildings.Count == 0 && baseScript.connectedBuildings.Count == 0)) 
		{
			if (GUILayout.Button ("Delete Spawned Buildings"))
			{
				baseScript.InitSpawnedBuildings();
			}
		}
			
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		if (GUILayout.Button ("Init Navmesh"))
		{
			baseScript.InitNavmeshPF ();
		}

		using (new EditorGUI.DisabledScope (baseScript.HasSettlementData () == false))
		{
//			if (GUILayout.Button ("Navmesh Pathfind Test"))
//			{
//				baseScript.TestNavmeshPathfinding ();
//			}

//			if (GUILayout.Button ("Clear Navmesh Path"))
//			{
//				baseScript.ClearTestPath ();
//			}
		}

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		baseScript.debugYAxisBool = EditorGUILayout.Toggle ("Vertical Crossroad: ", baseScript.debugYAxisBool);

		baseScript.maxSectionDistance = EditorGUILayout.FloatField ("Max Section Distance: ", baseScript.maxSectionDistance);


		using (new EditorGUI.DisabledScope(baseScript.HasSettlementData () == false))
		{
			if (GUILayout.Button ("Try Draw Cross Path"))
			{
				baseScript.TryDrawCrossRoad (baseScript.debugYAxisBool);
			}
		}

	}


}
