using Google.Protobuf;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
public class NetworkManager
{
    ServerSession _session = new ServerSession();

    public void Send(IMessage sendBuff)
    {
        _session.Send(sendBuff);
    }
    public void Init ()
    {
        //Dns
        IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
        IPEndPoint endPoint = new IPEndPoint(ipAddress, 7777);

        Connector connector = new Connector();

        connector.Connect(endPoint,
            () => { return _session; },
            1);
    }


    public void Update()
    {
        List<PacketMessage> list = PacketQueue.Instance.PopAll();
        foreach (PacketMessage packet in list)
        {
            Action<PacketSession, IMessage> handler = PacketManager.Instance.GetPacketHandler(packet.Id);
            if (handler != null)
                handler.Invoke(_session, packet.Message);
        }
    }

    public void Disconnect()
    {
        if (_session != null)
            _session.Disconnect();

        _session = null;
    }


}
