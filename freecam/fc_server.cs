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
		var obj = API.createObject(-1358020705, API.getEntityPosition(player), new Vector3(0.0,0.0,0.0)); //We create the object from server side, so later if we want to sync the object's position we can send the pos back here.
		PlayerObjectPairs.Add(player,obj);

		API.triggerClientEvent(player, "attachCamera", obj);

		API.freezePlayer(player, true);
		API.setEntityPosition(player,new Vector3(0.0,0.0,200.0));
		API.setEntityTransparency(player, 0);
	}
}
