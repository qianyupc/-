﻿using AAEmu.Commons.Network;
using AAEmu.Game.Core.Network.Game;

namespace AAEmu.Game.Core.Packets.C2G
{
    public class CSGetSiegeAuctionBidCurrencyPacket : GamePacket
    {
        public CSGetSiegeAuctionBidCurrencyPacket() : base(CSOffsets.CSGetSiegeAuctionBidCurrencyPacket, 5)
        {
        }

        public override void Read(PacketStream stream)
        {
            _log.Debug("CSGetSiegeAuctionBidCurrencyPacket");
        }
    }
}