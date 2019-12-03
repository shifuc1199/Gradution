Thanks for buying the Ferr2D Terrain Tool for Unity3D! We hope your experience with it is the best ever, and should it ever be less than that, please drop us a note and let us know!

If you see Gizmo icon related warnings, and/or the handles aren't displaying properly, please delete the Ferr folder, and re-install the plugin!

For documentation and tutorial videos go here! Or check out the quickstart guide and reference in the same folder as this file.
http://ferrlib.com/tool/ferr2d

We can always be reached either by email, or on twitter!
support@simbryocorp.com
@koujaku

Also, big thanks to Kelde for providing some sweet materials~ Check out his stuff!
@KaiElde
http://artbyelde.wordpress.com/

MENUS
GameObject->Create Ferr2D->Create Physical 2D Terrain (Ctrl+T)
	Creates a pre-built terrain object in the scene, with collider options on. It's the easiest way of creating a terrain object.

GameObject->Create Ferr2D->Create Decorative 2D Terrain (Ctrl+Shift+T)
	Same as previous, but with colliders off.
	
GameObject->Create Ferr2D->Create Terrain Material
	This creates a basic empty terrain material prefab. Hook up some materials to it, define some sides, and go!

Tools->Ferr->2D Terrain->Rebuild Ferr2D terrain in scene
	Forces all terrain in the scene to rebuild their meshes. Great if you've modified terrain materials with one scene open, but want to update the meshes in another scene.

Tools->Ferr->2D Terrain->Update scene Ferr2D objs with new material assets
	Looks for old format terrain materials (created in 1.0.9 or earlier) in the current scene, creates new formats when needed, and relinks objects with the new material assets!

Edit->Preferences->Ferr
	Various configuration options for visual and default values.

KEY CONTROLS
SHIFT+Click: add control point
C    +Click: change control point mode
ALT  +Click: removes control points
CTRL +Click: snap move points
Ctrl +R:     toggle smart snap
Ctrl +L:     toggle segment lock mode

VERSION LOG

v2.0.2 2018-4
+Bug fixes
 -Fixed physics materials occasionally getting mis-assigned
 -Gizmos were showing up in incorrect locations on Infinite Terrain Chunks

v2.0.1 2018-3
+Bug fixes
 -Fixed an infinite loop if body was set to zero
 -Fixed inverted edges being flipped the wrong direction
 -Help overlay was missing a small piece of text

v2.0 2018-3
+Terrain
 -Total re-write of core path code
 -Added control point modes: sharp, bezier, arc
 -Better curved edges, less texture distortion
 -Mutli-select now works without requiring hot-keys
 -Edit->Preferences Update Only On Release option for performance with complicated terrains
 -Streamlined the Ferr2DT_PathTerrain inspector
 -More than 4 edges are now available through Edge Overrides
 -Inner caps, and per-cap settings
 -Edge collider settings have been moved to the Terrain Material
 -Physics materials per-edge
 -A collection of different collider cap types
 -Legacy system for working with v1.12 terrains, 100% compatible
+Demos
 -Added new art assets and Terrain Materials from Kai Elde
 -Redid the Lighting demo scene
 -Runtime Edit demo was improved to show add/remove/edit
 -Infinite Terrain demo was re-written using a significantly better prefab system
 -Removed some really old demo files
+Bug Fixes
 -All closing segment issues should be gone
 -Fixed a bug where Applying a prefab would revert scene-linked properties

 UPDATING: This is a large update, please delete old Ferr2D and re-install plugin! It is backwards compatible with terrain objects, v1.x terrains will behave exactly as they did before, and will have an "Upgrade" button. New terrains can exist side by side with old ones.

v1.12 2017-9
+Terrain
 -Fixed UV glitch on extremely large or distant terrains
 -Improvements to scene GUI rendering
 -Fixed bug with colliders not obeying the Inverted Fill Border settings
 -Fixed issue with colliders on unsmoothed terrains warping on extreme corners
+General
 -Updated FerrCommon, added namespaces for name collision protection
 -Added hooks for Ferr2D to behave nicely with the upcoming Ferr vertex painting tool

v1.11 2017-6
+Terrain
 -Added a Segment Lock Mode [Ctrl+L and scene menu] where you can override the random body texture segments
 -Edge texture slicing is now independant of path points, resulting in way less stretching
 -Path points now automatically compensate scale to keep the top and bottom of the edge parallel
 -New terrain material picker shows recently used, and utilizes Unity's object picker
 -Inverted fill now has controls for how far the outer area extends
 -Added a button for generating colliders in editor instead of at start
+Shaders
 -Improved falloff algorithm for lights
 -Added fog compatibility
+General
 -Fixed 5.6 warnings and updated minimum version to 5.3, initial 2017.1 compatibility check
 -Switched terrain materials to ScriptableObjects, old materials still work, but are phasing out
 -ComponentTracker is now gone, ScriptableObjects are king
 -Fixed various issues with undo and marking modified objects dirty
 -Changed namespace on Poly2Tri to avoid collisions with other tools, please modify if you already have a version in your project!

NOTE: Materials that were created in v1.0.9 or earlier will need to be upgraded to appear in the material selector. Visit each scene with terrain objects, and choose the menu item "Tools->Ferr->2D Terrain->Update scene Ferr2D objs with new material assets". This will automatically create new material assets and connect terrain objects to them! You can also create new materials through the "Create Updated Material Object" button on the old assets, and relink them manually.

v1.10 2016-9
+Terrain
 -Added vertex color modes for angle gradients, edge distance gradients, and paint preservation
 -UI now shows edge overrides and scale tabs at the same time
 -Added multi-selection-edit, only for the inspector
+Shaders
 -Added vertex lit Ferr2D shaders
 -Switched lighting example scene to use vertex lit shaders
+General
 -Added support for external terrain materials
 -Improved undo support
 -Updated FerrCommon
 
v1.0.9 2015-6
+Terrain
 -Added inner elbow caps (uncheck simple in material editor)
 -Added split fill feature, which adds verts to enable vertex lighting and vertex painting
 -Added EdgeCollider2D support
 -Added UsedByEffector toggle
 -Colliders will now update with the terrain whenever a collider is present
+Shaders
 -Added lit wavy shader
 -Updated shaders for Unity 5
+General
 -Ferr2D is now directory independant
 -Improved handle control and performance
 -Switched triangulation library to poly2tri
 -Removed legacy code for Unity < v4.3
 -Removed JSON support
 -Updated FerrCommon
+Bug fixes
 -Fixed component tracker occasionally collecting duplicate items
 -Fixed a bug where empty edge materials would occasionally cause crashes
 -Fixed a bug with snap settings going crazy on first time use

v1.0.8 2014-10
+Path Terrain
 -Added per-node path scaling
 -Added scene toolbar for improved UI experience
 -Added a parallax edge tilt option
 -Added lightmap UV support
 -Added sharp collider corners option
 -Terrain objects now use a single draw call on terrain objects that only use 1 material
 -Added options for 2D Sort Layer and Order in Layer
 -Default values are now editable in preferences for PPU and Smoothing
 -Terrain collision mesh now updates while playing if modified in the editor
+Path
 -Added multi select and edit
 -Improved snapping (Ctrl is now snapping, delete is attached to Alt)
 -Added smart snapping, for snapping to other path point axes
 -Improved path point handle movement in non-2D views
+Shaders
 -Added lit shaders + Lighting demo scene
 -Added wavy shaders for underwater or wind effects
 -Added tinted unlit shaders
 -Improved unlit shader performance
+Editor
 -Faster build time due to improved component finding
 -Better prefab mesh saving
 -Bug fixes and small improvements to the material editor
 -Textures can now be resized without destrying the atlas
+Demos
 -Added a blob shadow component
 -Better loop seams on demo materials
 -Added FerrLib logo
 
v1.0.7 2014-3
+Improved smoothed terrain support, it's very useable now =D
+Improved triangulation performance, and no more weird holes!
+Basic snapping features can be found in Edit->Preferences->Ferr->Snap Mode, but it's a little early. I do not promise it's mind-blowing.
+Path Terrain
 -Fixed weird collider generation issues, colliders should now behave far more predictably!
 -Fixed prefab issues, terrain objects now save and load their meshes properly when they are prefab objects! ('Assets->Prebuild Ferr2D Terrain' to force save)
 -Fixed an issue with 2D physics materials not getting assigned correctly.
 -Added smoothSphereCollisions option for 3D colliders.                                                    COLLIDERS->Smooth Sphere Collisions
 -Added an edge split option, to reduce texture stretching around corners.                                 VISUALS->Split Middle
 -Added option to create tangents on terrain meshes for normal mapping (warning: very slow in editor only) VISUALS->Create Tangents
 -Added some options for when randomization occurs. Per segment, vs. per quad.                             VISUALS->Randomize by World Coordinates.
+Path
 -Editing on scaled/rotated path is much improved. Not perfect, but better.


v1.0.6 2013-11
+Added 4.3 support for the new 2D Physics system
+Added 4.3 support for the new Undo system
+Added a new example scene demonstrating how to make real-time edits to the terrain
+Added a cool JSON parser, improvements still in line for this later
+Added JSON save/load to Ferr2DT_PathTerrain, Ferr2DT_TerrainMaterial, and Ferr2D_Path
+Path Terrain
 -Added option to switch between 3D colliders and 2D colliders when using Unity 4.3+
 -Added a slider to adjust how Ferr2DT stretches texture segments
 -Added AddAutoPoint method for inserting points easily to the path, see the new example scene for reference
+Path
 -Tweaked handle size to work slightly better in orthographic view, more work on this due later
 
 
v1.0.5 2013-10
+Path Terrain
 -Added edge material overrides
 -Tweaked stretch algorithm, instead of stretching between a scale of 1 and 2, it now stretches between 0.5 and 1.5
 -Changed draw order of materials for better compatability with transparent fill materials
 -Added preference menu for showing or hiding terrain mesh lines
 -Added AddPoint method for easier procedural terrain
 -Fixed a collider glitch with Inverted fill modes
 -Fixed the Split Corners toggle, should now work
 -Fixed a glitch with the smooth split field not displaying properly
+Path
 -Updated icons for better visibility against same color backgrounds
 -Added preference menu for icon scaling
+Terrain Materials
 -Fixed bug with an overzealous simple mode
+Assets
 -Updated Ferr/Unlit Textured Vertex Color shader to properly use Unity's UV settings

v1.0.4 2013-10
+Terrain Materials
 -Added context menu to the project window for creating new Terrain Materials!
 -Moved the Terrain Material editor into its own windo, various and sundry improvements there.
 -Added a 'simple' mode to the TerrainMaterial editor, should be easier to create common case materials
+Path Terrain
 -Added physics material slot, since MeshColliders are generated at runtime
 -Added new fill types, Fill Only Closed, and Fill Only Skirt
 -Added option for disabling collider meshes along specific directions
 -Collider mesh no longer includes faces that point down +Z and -Z, as they're not important to 2D collisions, and it's much faster without\
+Path
 -Path GUI now scales properly with GameObject
 -Fixed an issue with button hotspots being too large when zoomed in

v1.0 2013-10
Release! Wooh! Have fun with it!