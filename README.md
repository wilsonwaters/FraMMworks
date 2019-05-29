# FraMMworks
FraMMWorks is a visual programming environment for creating complex video/audio processing applications from a collection of basic building blocks.
![FraMMWorks screenshot](docs/topologyScreenshot.PNG?raw=true "FraMMworks screenshot")
This (mock-up*) screenshot shows the the FraMMWorks "topology editor" - the window where you design your application.

On the left is the "toolbox" which contains various basic building blocks. On the right is the current processing graph. Tools can be dragged onto the graph and joined together to form complex applications.

This particular application is an automated doorbell - it rings the doorbell whenever someone (well, actually, something with a human looking face) stands at my door. Six steps are required:

1. Read video from a camera
2. Perform face detection
3. Split the video stream
4. Display the detected face
5. Detect presence of video
6. Switch on the USB powered relay to sound the doorbell
7. The interactive display helps you construct this processing chain with real-time feedback so you can fine tune parameters. The result is then saved as a stand-alone application which runs as a normal program.

Take a look at the users tutorial for more information of how to create your own applications.

# Usage
## Main Window
![FraMMWorks main window](docs/mainWindow.png?raw=true "FraMMworks main window")
The main window allows you to control playback and shows details from any plugins you are currently using. In the screenshot above two plugins are displaying details. A "filmstrip" display and a simple video display.

* Open existing projects through File->Open menu item.
* Show or hide Topology window through the View->Topology Designer menu.
* Show or hide the debugging messages window through View->DebugConsole?
* A processing mode may be selected depending on the indented use.
  * Processing mode ignores the target frame rate and processes each frame as fast as possible.
  * Live mode attempts to force the output frame rate to be exactly as requested. Frames will be dropped if the system cant process fast enough.
* The top half of the window displays graphical output from any tool in your tool chain which has this functionality.
  * The tools each get a display area (if required) which is automatically arranged by FraMMWorks. If you have a lot of tools, you may need to scroll the window to find its display area.
* The bottom half of the window allows you to control processing with play, stop, pause and navigation controls.

## Tool Chain Editor
![FraMMWorks screenshot](docs/topologyScreenshot.PNG?raw=true "FraMMworks screenshot")
This window is where plugin "chains" are created.

* The left pane is the toolbox which contains tools from all FraMMWorks plugins which were found on your system.
* The right pane is the tool-chain drawing area.
* Drag tools from toolbox to drawing area.
* Each tool has a number of input and/or output "pins".
* Link tools together by dragging one tool's output pin to another tool's input pin.
* Each tool may be double clicked to display its configurable items.

## Testing Tool Chains
After a tool chain has been created in the editor it can be run from the main window.

* Click on the play button to start processing.
* If nothing happens, open the debugging window to see the problem (i.e. if you have connected a video output pin to an audio input pin an error will be displayed).
* Processing progress will be indicated on the timebar.
* If any of the tools on your tool-chain need a display area, it will be shown in the top half of the main window.
* Modify a tools configurable items in the tool chain editor and watch the resulting change in the main window.

## Saving Application
Once your tool chain is working acceptably you can save it to an XML file through the File->Save menu item. This can be loaded later.

Eventually, you will be able to "Save as application" which will combine all tools and configuration into a single executable. This can then be run as a standalone application.

# Design
![FraMMWorks flow](docs/FraMMworks-flow.png?raw=true "FraMMworks flow")

![FraMMWorks class diagram](docs/FraMMworks-class.png?raw=true "FraMMworks class")

# License
Licensed under GNU GENERAL PUBLIC LICENSE v3.0.
Copyright Wilson Waters 2009
