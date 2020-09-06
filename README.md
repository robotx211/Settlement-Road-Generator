#SettlementRoadGenerator

SETUP

- The Unity project "TerrainTest - 2017 ver" must be run in Unity version 2017.4.5f1, which is the version provided by AppsAnywhere
- Do not attempt to rebuild this project in a different version of Unity
- The project can be loaded from the USB


INSTRUCTIONS FOR USE

 - Note - This is intended to be be used in EDIT MODE, do not use in PLAY MODE as the spawned building will not remain in the scene

- The Terrain Checker is the main component from which settlments can be generated
	- Before doing any generation, press the 'Reset Terrain Specific Values' button

- The MainTerrain comes starting as a premade terrain which a settlement can be generated on
- The MainTerrain can be modified, however there are some restrictions
	- There must be some edges left open on every side. This is because the system must generate 2 initial starting roads across the terrain in order to start
	- The size of the heightmap and alphamap resolutions must be within 1 of each other (e.g. 513 and 512)
	- There must be at least 2 textures assigned to the Terrain (MainTerrain starts with 4 attached), 1 of which should be the road texture
	- If a new Terrain is made, it must be reference as the Specified Terrain in the Terrain Checker

- Buildings are created by attaching a model to a parent class with the Building script attached (see Assets -> terrainTest -> Buildings for examples)
	- The Building component is used to set the building radius, height and spawning parameters
	- Please do not edit the attached Nav Mesh Modifier, this is used for the pathfinding (stops the buildings from cutting into the navmesh)
	- Building models should come with a 'foundation' built in (see examples) if their maximum steepness is more than 0
	- Completed building prefabs can be attached/removed from the generation process by adding/removing them from the Building List on the Terrain Checker

- Automated Terrain Texturing
	- Allows for texturing the terrain automatically
	- First, set the index of the road texture
	- Secondly, set the steepness bounds for each other texture (Reset Terrain Specific Values will automatically generate these with no blending)
	- While crossing bounds is ok and encourage, ensure that all angles are covered by at least 1 texture (if not, this process will not work)
	- Once the steepness bounds have been set, press the "Update Texture Steepness" button

- Road parameters must be set up before generation
	- Firstly, ensure the correct road texture index is selected
	- Secondly, select the 'Road' mode, and set the Full and Max Width, and the Max Road Steepness (Max width determines how wide the road is, while Full width is used
	  for the road visuals, setting how wide the road texture will be full)
	- Thirdly, in the Nav Mesh Surface component, select the Agent Type and open the Agent Settings. These values will be used for pathfinding the road on the navmesh.
	  Set the radius to be 1/2 the max width, the Height to be 100, the Step Height to be 1 and the Max Slope to be the same as the Max Road steepness.
	  (Unfortunately, these values must be set here as well as the Terrain Checker component, as Unity does not allow any way of editing this data via script)
	  If is advised to keep the Max Road Steepness quite low, but if it is increased, it may be necessary to increase the step height, as indicated in the warning popup.
	  This is to ensure the NavMesh is basked properly

- Generating a Settlement
	- Firstly, press the "Init Navmesh" and "Generate New Settlement Data" buttons. This will unlock the other buttons for use
	- Secondly, set the Max Section Distance in the Terrain Checker. This decides how far along a road a connection point should be checked. Lower numbers will
	  produce better results, but will take take longer to process
	- Thirdly, press the "Try Draw Cross Path" button to attempt to generate a vertical or horizontal (based on the Vertical Crossroad bool tickbox)
	  If a path is generated, it will be shown as a set of points in on the Terrain, otherwise an error message will be printed to the console, and you should try again.
	- At this point, you can draw the Roads by pressing the "Apply Road Textures From Settlement Data" 
	  (this will take some time, and may look like Unity has crashed, please leave it to draw)
	- Forthly, you can spawn buildings. Press the "Random Spawn Building" to spawn a building on the Terrain. This will find a position for the building to spawn. 
	  You can press this as much as you like.
	- Once you have 'spawned' all the buildings, press the "Add Buildings from Settlement Data" button, to create the buildings on the Terrain.
	- Then, press the "Try Connect Buildings To Road" button, to find paths to each building. these will be shown with more sets of points on the Terrain.
	- Once paths have been found, press the "Apply Road Textures From Settlement Data" to draw the roads to buildings

- Resetting the Settlement
	- To reset the settlement, you must
		- Press the "Init Navmesh" Button
		- Press the "Clear Settlement Data" Button
		- Press the "Delete Spawned Buildings" Button
	  