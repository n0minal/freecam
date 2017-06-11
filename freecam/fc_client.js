var d = Date.now();
var CameraObject = null;
var cam = null;
var wdown = false;
var sdown = false;
var adown = false;
var ddown = false;
var shiftdown = false;
var altdown = false;

API.onKeyDown.connect(function (sender, e) 
{
	if (e.KeyCode === Keys.W)wdown = true;
	if (e.KeyCode === Keys.A)adown = true;
	if (e.KeyCode === Keys.S)sdown = true;
	if (e.KeyCode === Keys.D)ddown = true;

	if (e.KeyCode === Keys.RShiftKey ||e.KeyCode === Keys.ShiftKey)shiftdown = true;
	if (e.KeyCode === Keys.Menu ||e.KeyCode === Keys.RMenu)altdown = true;
});

API.onKeyUp.connect(function (sender, e) 
{
	if (e.KeyCode === Keys.W)wdown = false;
	if (e.KeyCode === Keys.A)adown = false;
	if (e.KeyCode === Keys.S)sdown = false;
	if (e.KeyCode === Keys.D)ddown = false;

	if (e.KeyCode === Keys.RShiftKey ||e.KeyCode === Keys.ShiftKey)shiftdown = false;
	if (e.KeyCode === Keys.Menu ||e.KeyCode === Keys.RMenu)altdown = false;
});

var lastPos = null;

API.onUpdate.connect(function() 
{
	API.disableControlThisFrame(16);
	API.disableControlThisFrame(17);
	API.disableControlThisFrame(26);
	API.disableControlThisFrame(24);

	var camRot = API.getGameplayCamRot();
	var camDir = API.getGameplayCamDir();

	if(cam != null)
	{
		var to = null;
		var pos2 = null;
		var camPos = API.getEntityPosition(CameraObject);

		//API.sendChatMessage(camPos.X+" "+camPos.Y+" "+camPos.Z);
		var multiply = 1;
		if(shiftdown == true) multiply = 3;
		if(altdown == true)multiply = 0.5;

		if(wdown == true)
		{
			to = Vector3.Lerp(camPos, camPos.Add(camDir.Multiply(multiply)), 1.0);
		}
		if(sdown == true)
		{
			if(to != null)camPos = to;
			to = Vector3.Lerp(camPos, camPos.Subtract(camDir.Multiply(multiply)), 1.0);
		}
		if(adown == true)
		{
			if(to != null)camPos = to;
			pos2 = GetPositionInFront(multiply,camPos,camRot.Z,90);
			to = Vector3.Lerp(camPos, pos2, 1.0);
		}
		if(ddown == true)
		{
			if(to != null)camPos = to;
			pos2 = GetPositionInFront(multiply,camPos,camRot.Z,-90);
			to = Vector3.Lerp(camPos, pos2, 1.0);
		}

		if(to != null && CameraObject != null)API.setEntityPosition(CameraObject,to); //GTMP BUG: If the camera goes out from the streaming distace of the object's spawn position->System.NullReferenceException

		API.setEntityRotation(API.getLocalPlayer(),new Vector3(0.0,0.0,camRot.Z));

		if(camRot != API.getCameraRotation(cam))
		{
			API.setCameraRotation(cam,camRot);
		}
		//By not a pope
		if(lastPos != null && camPos != null && lastPos.DistanceTo(camPos) > 100.0)API.callNative("_SET_FOCUS_AREA", camPos.X, camPos.Y, camPos.Z, lastPos.X, lastPos.Y, lastPos.Z);
		lastPos = camPos;
	}
});


API.onServerEventTrigger.connect(function (name, args) 
{
	if(name == "attachCamera")
	{
		CameraObject = args[0];
		cam = API.createCamera(API.getEntityPosition(API.getLocalPlayer()), new Vector3(0.0,0.0,0.0));
		API.attachCameraToEntity(cam,CameraObject,new Vector3(0.0,0.0,0.0)); //GTMP BUG: getting the camera position returns null, so i had to attach the camera to an object and move that instead
		API.setEntityCollisionless(CameraObject, true);
		API.setActiveCamera(cam);

		API.callNative("DISPLAY_RADAR",false);
	}
});

//By Don. Ported from C# to JS
function ClampAngle(angle)
{
	return (angle + Math.ceil(-angle / 360) * 360);
}

function GetPositionInFront(range, pos, zrot, plusangle)
{
	var angle = ClampAngle(zrot) * (Math.PI / 180);
	plusangle = (ClampAngle(plusangle) * (Math.PI / 180));

	pos.X += (range * Math.sin(-angle-plusangle));
	pos.Y += (range * Math.cos(-angle-plusangle));
	return pos;
}