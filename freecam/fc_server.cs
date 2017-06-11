using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared.Math;
using System.Threading;
//using System.Xml;

public class FreeCam : Script
{
	public Dictionary<Client, NetHandle> PlayerObjectPairs = new Dictionary<Client, NetHandle>(); //For future versions
	public Dictionary<Client, bool> freecamControlsDisabled = new Dictionary<Client, bool>();

	public FreeCam()
	{
		API.onPlayerFinishedDownload += onPlayerDownloaded;
		API.onPlayerDisconnected += OnPlayerDisconnectedHandler;
	}

	private void OnPlayerDisconnectedHandler(Client player, string reason)
	{

		PlayerObjectPairs.Remove(player);
	}

	public void onPlayerDownloaded(Client player)//first spawn
	{
		//startFreecam(player);
	}
	public void startFreecam(Client player)
	{
		if(PlayerObjectPairs.ContainsKey(player))stopFreecam(player);
		var obj = API.createObject(-1358020705, API.getEntityPosition(player), new Vector3(0.0,0.0,0.0)); //We create the object from server side, so later if we want to sync the object's position we can send the pos back here.
		PlayerObjectPairs.Add(player,obj);

		API.triggerClientEvent(player, "startFreecam", obj);

		API.freezePlayer(player, true);
		API.setEntityPosition(player,new Vector3(0.0,0.0,200.0));
		API.setEntityTransparency(player, 0);
	}
	public void stopFreecam(Client player)
	{
		if(PlayerObjectPairs.ContainsKey(player))
		{
			API.deleteEntity(PlayerObjectPairs[player]);
			PlayerObjectPairs.Remove(player);

			API.triggerClientEvent(player, "stopFreecam");

			API.freezePlayer(player, false);
			API.setEntityPosition(player,new Vector3(0.0,0.0,80.0));
			API.setEntityTransparency(player, 255);
		}
	}
	public void ToggleFreecamControls(Client player, bool toggle)
	{
		if(toggle == true)
		{
			if(!freecamControlsDisabled.ContainsKey(player))
			{
				freecamControlsDisabled.Add(player,true);
				API.triggerClientEvent(player, "toggleFreecamControls",true);
			}
		}
		else
		{
			if(freecamControlsDisabled.ContainsKey(player))
			{
				freecamControlsDisabled.Remove(player);
				API.triggerClientEvent(player, "toggleFreecamControls",false);
			}
		}
	}
	public bool isFreecamControlsEnabled(Client player)
	{
		return !freecamControlsDisabled.ContainsKey(player);
	}
	public bool isFreecamOn(Client player)
	{
		return PlayerObjectPairs.ContainsKey(player);
	}
	[Command("freecam")]
	public void FreecamToggle(Client player) 
	{
		if(isFreecamOn(player))stopFreecam(player);
		else startFreecam(player);
	}
	[Command("controls")]
	public void ControlsToggle(Client player) 
	{
		if(isFreecamControlsEnabled(player))ToggleFreecamControls(player, true);
		else ToggleFreecamControls(player, false);
	}

}
