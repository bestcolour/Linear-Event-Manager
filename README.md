# LEM NodeEditor
 Hi there! Linear Event Manager or LEM is a Unity Node Editor which allows for calling a series of methods arranged by a user via a node editor.

Quick Steps to setting up LEM. 
1) Download the 3 Folders and import them into your Unity's Asset folder
2) Create an "Editor" folder in you Asset folder if you don't already have one and place "LEM_Editor_Files" folder into it.
3) Create a gameobject and add the Linear Event Manager component to the gameobject.
4) Create another gameobject and add the Linear Event component to the object.
5) Press "Open" to load the Linear Event in the Node Editor
6) After saving the Linear Event, create any script to call a reference to the Initialize and Play the Linear Event you wish play.


NODE EDITOR SPECIFIC CONTROLS
Right Click: Opens search box to search for Effects for which the Linear Event you are currently editing will play.
Crlt + Click on searchbox option: Creates the selected option node but does'nt close the searchbox
Crlt + Q: Undo previous action (Does not Undo changes done to the fields displayed on the Node)
Crlt + W: Redo previous action (Does not Redo changes done to the fields displayed on the Node)
Crlt + S: Saves currently editing Linear Event (Depending on settings, see Settings section)
Crlt + C selected nodes: Copy
Crlt + X selected nodes: Cut
Crlt + V: Paste
Crlt + G: Group selected nodes
DeleteKey: Delete selected nodes
Scroll: Zooms in or out normally
Alt + Scroll: Zooms in or out slowly
Click + Drag: Select nodes within the dragged area
Alt + Click + Drag: Pan around the node editor


SETTINGS
Setting is a scriptableobject asset which can be created by right clicking and clicking on the "Create > NODELEM Settings" but by default the package should have one in the Editor Folder

Theme: Choose Dark im serious it looks better in my opinion.

Saving Modes: 
 1) DontSave = NodeEditor doesnt save itself at all,
 2) AlwaysSave = NodeEditor will always save itself whenever Editor loses windowfocus or before loading a new lienaar event
 3) SaveWhenCommandChange = NodeEditor will only save when there is a change in the editor base on its commands ie. Move Node, Cut, Paste, Create Node etc

History Length: Determines how much commands will be recorded in the Node Editor. Increase to have more undos and redos.

Show ToolBar: Set to true to show toolbar

Button Size: Size of the toolbar buttons.


Additional Notes: 
- Linear Events inherits from Monobehaviour hence it can also work in Prefabs. 
- There is a custom Attribute called "LinearDescription" written in the package which you could use to make LinearEvent referencing easier.


Future Fixes (if i ever get myself to do it) :
- Create a guide on how to make Custom Nodes in this Node Editor
- Fix the zooming of the node editor (right now its zoomed pivoted to the top left of the window screen)
- Add more effects like maybe text animations
- Maybe make the editor code more organized


Reflections (Skip if you want):
This 3 months of doing this while having other commitments to tend to has been interesting. This project gave me so much problems and I've stumbled onto roadblocks one after the other. But i've learnt a lot from this "exercise". I've learnt and tried out various design patterns and also took an appreciation to how the engine is designed. I guess the most important thing i've learnt is how to manage my time in order to not give up. Schedules are great but they are not a prison. Well its been fun learning all these, i may or may not come back to this. People may or may not see this project and use it but im glad i've done this project. Right, time to crack open this project and test it out :D

