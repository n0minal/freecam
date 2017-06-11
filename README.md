# freecam
This resource was created for the MapEditor but it works by itself too.

Exported functions:
startFreecam(Client player); //Start the freecam
stopFreecam(Client player); //Stop the freecam
ToggleFreecamControls(Client player, bool toggle); //The camera will stop moving, good for opening dialogs etc.
bool isFreecamControlsEnabled(Client player);
bool isFreecamOn(Client player);
NetHandle getFreecamObject(Client player); //Returns the object the player's cam attached to.

Controls:
W, A, S, D - Movement
Shift - Speed up the movement
Alt - Slow down the movement

Feel free to contribute.
