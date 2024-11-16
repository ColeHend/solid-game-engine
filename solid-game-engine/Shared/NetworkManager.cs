namespace solid_game_engine.Shared;

using System;
using System.Linq;
using Newtonsoft.Json;
using solid_game_engine.Shared.Enums;
using Steamworks;

public class NetworkData
{
	public string Data { get; set; }
	public NetworkType Type { get; set; }
}

public class NetworkManager
{
	public NetworkManager(uint appId)
	{
		SteamClient.Init(appId, true);
		
	}

	public void CreateLobby(int maxPlayers = 4)
	{
		SteamMatchmaking.CreateLobbyAsync(maxPlayers).ContinueWith(task =>
		{
			if (task.Result.HasValue)
			{
				var lobby = task.Result.Value;
				Console.WriteLine("Lobby created with ID: " + lobby.Id);
			}
			else
			{
				Console.WriteLine("Failed to create lobby.");
			}
		});
	}

	public void JoinLobby(SteamId lobbyId)
	{
		SteamMatchmaking.JoinLobbyAsync(lobbyId).ContinueWith(task =>
		{
			if (task.Result.HasValue)
			{
				Console.WriteLine("Successfully joined lobby.");
			}
			else
			{
				Console.WriteLine("Failed to join lobby.");
			}
		});
	}

	public void SessionRequest()
	{
		SteamNetworking.OnP2PSessionRequest = (steamid) =>
		{
			var friends = SteamFriends.GetFriends();
			if (friends.Any(x => x.Id == steamid))
			{
					SteamNetworking.AcceptP2PSessionWithUser(steamid);
			}
		};
	}

	public void ReadPackets(int channel = 0)
	{
		while (SteamNetworking.IsP2PPacketAvailable(channel))
		{
			var packet = SteamNetworking.ReadP2PPacket();
			if (packet.HasValue)
			{
					HandleMessageFrom(packet.Value.SteamId, packet.Value.Data);
			}
		}
	}

	public bool SendData(SteamId targetSteamId, byte[] mydata, int channel = 0)
	{
		return SteamNetworking.SendP2PPacket(targetSteamId, mydata, -1, channel);
	}

	private void HandleMessageFrom(SteamId steamid, byte[] data)
	{
		var dataString = System.Text.Encoding.UTF8.GetString(data);
		var netData = JsonConvert.DeserializeObject<NetworkData>(dataString);
		switch (netData.Type)
		{
			case NetworkType.Player:
				HandlePlayerMessage(steamid, netData);
				break;
			case NetworkType.Npc:
				HandleNpcMessage(steamid, netData);
				break;
		}
	}

	private void HandlePlayerMessage(SteamId steamid, NetworkData netData)
	{
		// Implement your message handling logic here
	}

	private void HandleNpcMessage(SteamId steamid, NetworkData netData)
	{
		// Implement your message handling logic here
	}
}
