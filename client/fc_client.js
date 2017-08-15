var d = Date.now();
var CameraObject = null;
var cam = null;
var wdown = false;
var sdown = false;
var adown = false;
var ddown = false;
var shiftdown = false;
var altdown = false;
var freecamMode = false;
var toggleControl = true;
var lastPos = null;

var res = API.getScreenResolution();
var infoBrowser = API.createCefBrowser(res.Width, res.Height, true);
API.waitUntilCefBrowserInit(infoBrowser);
API.setCefBrowserPosition(infoBrowser, 0, 0);
API.loadPageCefBrowser(infoBrowser, "frontend.html");


API.onKeyDown.connect(function (sender, e) 
{
	if (e.KeyCode === Keys.W)wdown = true;
	if (e.KeyCode === Keys.A)adown = true;
	if (e.KeyCode === Keys.S)sdown = true;
	if (e.KeyCode === Keys.D)ddown = true;
});

API.onKeyUp.connect(function (sender, e) 
{
	if (e.KeyCode === Keys.W)wdown = false;
	if (e.KeyCode === Keys.A)adown = false;
	if (e.KeyCode === Keys.S)sdown = false;
	if (e.KeyCode === Keys.D)ddown = false;
});


API.onUpdate.connect(function() 
{
	//send updated data to frontend
	var player = API.getLocalPlayer();
	var pos = API.getEntityPosition(player);
	var rot = API.getEntityRotation(player);
	var dimension = API.getEntityDimension(player);
	var camPos = API.getCameraPosition(cam);
	var camRot = API.getGameplayCamRot();
	var camDir = API.getGameplayCamDir();

	if(freecamMode == true && toggleControl == true)
	{
		API.disableControlThisFrame(16);
		API.disableControlThisFrame(17);
		API.disableControlThisFrame(26);
		API.disableControlThisFrame(24);

		if(cam != null)
		{
			var targetPos = null;
			var pos2 = null;

			var multiplier = 1;

			if(API.isControlJustPressed(19)){
				multiplier = 5.0;
			}
			else if(API.isControlJustPressed(21)){
				multiplier = 0.5;
			}

			if(wdown == true)
			{
				targetPos = Vector3.Lerp(camPos, camPos.Add(camDir.Multiply(multiplier)), 1.0);
			}
			if(sdown == true)
			{
				if(targetPos != null) camPos = targetPos;
				targetPos = Vector3.Lerp(camPos, camPos.Subtract(camDir.Multiply(multiplier)), 1.0);
			}
			if(adown == true)
			{
				if(targetPos != null) camPos = targetPos;
				pos2 = getPositionInFront(multiplier, camPos, camRot.Z, 90);
				targetPos = Vector3.Lerp(camPos, pos2, 1.0);
			}
			if(ddown == true)
			{
				if(targetPos != null)camPos = targetPos;
				pos2 = getPositionInFront(multiplier, camPos, camRot.Z, -90);
				targetPos = Vector3.Lerp(camPos, pos2, 1.0);
			}

			if(targetPos != null)
			{
				API.setCameraPosition(cam, targetPos);
			}

			API.setEntityRotation(API.getLocalPlayer(),new Vector3(0.0,0.0,camRot.Z));

			if(camRot != API.getCameraRotation(cam))
			{
				API.setCameraRotation(cam, camRot);
			}
			//update camera focus data
			if(lastPos != null && camPos != null && lastPos.DistanceTo(camPos) > 100.0) API.callNative("_SET_FOCUS_AREA", camPos.X, camPos.Y, camPos.Z, lastPos.X, lastPos.Y, lastPos.Z);
			lastPos = camPos;
		}
	}
	//send updated data to front-end
	var updateData = {
		"dimension": dimension,
		"pos": pos,
		"rot": rot,
		"camPos": camPos,
		"camRot": camRot,
		"camDir": camDir,
	}
	var obj = JSON.stringify(updateData);
	API.resourceCall("update", obj);
});


API.onServerEventTrigger.connect(function (name, args) 
{
	if(name == "setFreeCamState")
	{
		var active = args[0];
		var player = API.getLocalPlayer();

		if(active){
			cam = API.createCamera(API.getEntityPosition(player), API.getEntityRotation(player));
			freecamMode = true;
			API.setActiveCamera(cam);
			API.callNative("DISPLAY_RADAR", false);
		}
		else{
			cam = null;
			freecamMode = false;
			API.setActiveCamera(null);
			API.callNative("DISPLAY_RADAR",true);
			API.callNative("SET_FOCUS_ENTITY",API.getLocalPlayer());
		}		
	}
	if(name == "toggleFreecamControls")
	{
		toggleControl = args[0];
	}
});

//By Don. Ported from C# to JS
function clampAngle(angle)
{
	return (angle + Math.ceil(-angle / 360) * 360);
}

function getPositionInFront(range, pos, zrot, plusangle)
{
	var angle = clampAngle(zrot) * (Math.PI / 180);
	plusangle = (clampAngle(plusangle) * (Math.PI / 180));

	pos.X += (range * Math.sin(-angle-plusangle));
	pos.Y += (range * Math.cos(-angle-plusangle));
	return pos;
}