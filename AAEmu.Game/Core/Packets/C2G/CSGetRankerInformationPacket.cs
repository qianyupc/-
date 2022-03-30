﻿using AAEmu.Commons.Network;
using AAEmu.Game.Core.Network.Game;

namespace AAEmu.Game.Core.Packets.C2G
{
    public class CSGetRankerInformationPacket : GamePacket
    {
        public CSGetRankerInformationPacket() : base(CSOffsets.CSGetRankerInformationPacket, 5)
        {
        }

        public override void Read(PacketStream stream)
        {
            _log.Debug("CSGetRankerInformationPacket");
        }
    }
}