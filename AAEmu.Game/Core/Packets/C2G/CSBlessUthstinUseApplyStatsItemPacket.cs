﻿using AAEmu.Commons.Network;
using AAEmu.Game.Core.Network.Game;

namespace AAEmu.Game.Core.Packets.C2G
{
    public class CSBlessUthstinUseApplyStatsItemPacket : GamePacket
    {
        public CSBlessUthstinUseApplyStatsItemPacket() : base(CSOffsets.CSBlessUthstinUseApplyStatsItemPacket, 5)
        {
        }

        public override void Read(PacketStream stream)
        {
            _log.Debug("CSBlessUthstinUseApplyStatsItemPacket");
        }
    }
}