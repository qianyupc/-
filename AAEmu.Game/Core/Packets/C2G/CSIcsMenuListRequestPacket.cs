﻿using AAEmu.Commons.Network;
using AAEmu.Game.Core.Network.Game;

namespace AAEmu.Game.Core.Packets.C2G
{
    public class CSICSMenuListRequestPacket : GamePacket
    {
        public CSICSMenuListRequestPacket() : base(CSOffsets.CSIcsMenuListRequestPacket, 5)
        {
        }

        public override void Read(PacketStream stream)
        {
            _log.Debug("CSICSMenuListRequestPacket");
        }
    }
}