﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Prototype.NetworkLobby;

public class NetworkHook : LobbyHook
{
    public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer)
    {
        //base.OnLobbyServerSceneLoadedForPlayer(manager, lobbyPlayer, gamePlayer);
        LobbyPlayer lobby = lobbyPlayer.GetComponent<LobbyPlayer>();
        PlayerScript player = gamePlayer.GetComponent<PlayerScript>();
        player.PlayerName = lobby.playerName;
    }
}
