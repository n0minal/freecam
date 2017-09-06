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

public class FreeCam : Script
{
	private Dictionary<Client, bool> freeCamActive = new Dictionary<Client, bool>();
	private Dictionary<Client, bool> freeCamControlsDisabled = new Dictionary<Client, bool>();

	public void setFreeCamState(Client player, bool active){
		if(active){
			freeCamActive.Add(player, true);
			API.setEntityTransparency(player, 0);
			API.triggerClientEvent(player, "setFreeCamState", true);
			API.freezePlayer(player, true);
		}
		else{
			freeCamActive.Remove(player);
			API.setEntityTransparency(player, 255);
			API.triggerClientEvent(player, "setFreeCamState", false);
			API.freezePlayer(player, false);
		}		
	}

	public void toggleFreecamControls(Client player, bool toggle)
	{
		if(toggle && !freeCamControlsDisabled.ContainsKey(player))
		{
			freeCamControlsDisabled.Add(player, true);
			API.triggerClientEvent(player, "toggleFreecamControls", true);
		}
		else
		{
			freeCamControlsDisabled.Remove(player);
			API.triggerClientEvent(player, "toggleFreecamControls", false);
		}
	}

	public bool isFreecamControlsEnabled(Client player)
	{
		if(freeCamControlsDisabled.ContainsKey(player) == true) return true;
		return false;
	}

	public bool isFreecamActive(Client player)
	{
		if(freeCamActive.ContainsKey(player)) return true;
		return false;
	}

	[Command("freecam")]
	public void FreecamToggle(Client player) 
	{
		if(isFreecamActive(player) == true){
			setFreeCamState(player, false);
		}
		else{
			setFreeCamState(player, true);
		}
	}

	[Command("controls")]
	public void ControlsToggle(Client player) 
	{
		if(isFreecamControlsEnabled(player)){
			toggleFreecamControls(player, false);
		}
		else{
			toggleFreecamControls(player, true);
		}
	}
}
