﻿using AAEmu.Commons.Network;
using AAEmu.Game.Core.Network.Game;

namespace AAEmu.Game.Core.Packets.C2G
{
    public class CSICSGoodsListRequestPacket : GamePacket
    {
        public CSICSGoodsListRequestPacket() : base(CSOffsets.CSIcsGoodsListRequestPacket, 5)
        {
        }

        public override void Read(PacketStream stream)
        {
            _log.Debug("CSICSGoodsListRequestPacket");
        }
    }
}