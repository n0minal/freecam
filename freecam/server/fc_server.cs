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
	private Dictionary<Client, NetHandle> _playerObjectPairs = new Dictionary<Client, NetHandle>();
	private Dictionary<Client, bool> _freecamControlsDisabled = new Dictionary<Client, bool>();

	public FreeCam()
	{
		API.onPlayerFinishedDownload += onPlayerDownloaded;
		API.onPlayerDisconnected += OnPlayerDisconnectedHandler;
		API.onClientEventTrigger += OnClientEvent;
	}

	public void OnClientEvent(Client player, string eventName, params object[] arguments)
	{
		if (eventName == "setFreecamObjectPositionTo")//object movement sync
		{
			Vector3 to = (Vector3)arguments[0];
			if(_playerObjectPairs.ContainsKey(player))API.setEntityPosition(_playerObjectPairs[player], to);
		}
	}

	private void OnPlayerDisconnectedHandler(Client player, string reason)
	{
		_playerObjectPairs.Remove(player);
	}

	private void onPlayerDownloaded(Client player)//first spawn
	{
		//startFreecam(player);
	}

	public void startFreecam(Client player)
	{
		if(_playerObjectPairs.ContainsKey(player))stopFreecam(player);
		NetHandle obj = API.createObject(-1358020705, API.getEntityPosition(player), new Vector3(0.0, 0.0, 0.0)); //We create the object from server side, so later if we want to sync the object's position we can send the pos back here.
		_playerObjectPairs.Add(player, obj);

		API.triggerClientEvent(player, "startFreecam", obj);

		API.freezePlayer(player, true);
		API.setEntityPosition(player, new Vector3(0.0, 0.0, 200.0));
		API.setEntityTransparency(player, 0);
	}

	public void stopFreecam(Client player)
	{
		if(_playerObjectPairs.ContainsKey(player))
		{
			API.deleteEntity(_playerObjectPairs[player]);
			_playerObjectPairs.Remove(player);

			API.triggerClientEvent(player, "stopFreecam");

			API.freezePlayer(player, false);
			API.setEntityPosition(player, new Vector3(0.0, 0.0, 80.0));
			API.setEntityTransparency(player, 255);
		}
	}

	public void toggleFreecamControls(Client player, bool toggle)
	{
		if(toggle && !_freecamControlsDisabled.ContainsKey(player))
		{
			_freecamControlsDisabled.Add(player, true);
			API.triggerClientEvent(player, "toggleFreecamControls", true);
		}
		else
		{
			_freecamControlsDisabled.Remove(player);
			API.triggerClientEvent(player, "toggleFreecamControls", false);
		}
	}

	public bool isFreecamControlsEnabled(Client player)
	{
		return !_freecamControlsDisabled.ContainsKey(player);
	}

	public bool isFreecamOn(Client player)
	{
		return _playerObjectPairs.ContainsKey(player);
	}

	public NetHandle getFreecamObject(Client player)
	{
		if(_playerObjectPairs.ContainsKey(player))return _playerObjectPairs[player];
		else return new NetHandle();
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
		if(isFreecamControlsEnabled(player))toggleFreecamControls(player, true);
		else toggleFreecamControls(player, false);
	}

}
