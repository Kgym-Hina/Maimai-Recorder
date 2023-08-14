using System;
using OBSWebsocketDotNet.Communication;

namespace RecordAppBlazor.Function;

public static class Obs
{
    public static readonly OBSWebsocketDotNet.OBSWebsocket client = new OBSWebsocketDotNet.OBSWebsocket();
    private static bool isConnected;

    public static void InitializeObs()
    {
        client.Connected += delegate(object? sender, EventArgs args) { isConnected = true; };
        client.Disconnected += delegate(object? sender, ObsDisconnectionInfo info) { isConnected = false; };
        client.ConnectAsync("ws://localhost:4455", "");

        Console.Out.WriteLine("OBSWebsocketDotNet.OBSWebsocket initialized");
    }

}