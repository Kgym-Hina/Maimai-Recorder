using System;
using OBSWebsocketDotNet.Communication;
using RecordAppBlazor.Data;

namespace RecordAppBlazor.Function;

public static class Obs
{
    public static readonly OBSWebsocketDotNet.OBSWebsocket client = new OBSWebsocketDotNet.OBSWebsocket();
    private static bool isConnected;

    public static void InitializeObs()
    {
        client.Connected += delegate(object? sender, EventArgs args) { isConnected = true; };
        client.Disconnected += delegate(object? sender, ObsDisconnectionInfo info) { isConnected = false; };
        client.ConnectAsync(PropertiesManager.Properties.OBSWebsocketAddress, PropertiesManager.Properties.OBSWebsocketPassword);

        Console.Out.WriteLine("OBSWebsocketDotNet.OBSWebsocket initialized");
    }

}