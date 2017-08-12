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
		API.freezePlayer(player, active ? true : false);
		API.setEntityTransparency(player, active ? 0 : 255);
		API.triggerClientEvent(player, "setFreeCamState", active ? true : false);
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
		return freeCamControlsDisabled.ContainsKey(player);
	}

	public bool isFreecamActive(Client player)
	{
		return freeCamActive.ContainsKey(player);
	}

	[Command("freecam")]
	public void FreecamToggle(Client player) 
	{
		setFreeCamState(player, isFreecamActive(player) ? false : true);
	}

	[Command("control")]
	public void ControlsToggle(Client player) 
	{
		toggleFreecamControls(player, isFreecamControlsEnabled(player) ? false : true);
	}
}
