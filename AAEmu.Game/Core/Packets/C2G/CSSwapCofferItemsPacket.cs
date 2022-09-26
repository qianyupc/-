﻿using AAEmu.Commons.Network;
using AAEmu.Game.Core.Network.Game;
using AAEmu.Game.Models.Game;
using AAEmu.Game.Models.Game.Items;

namespace AAEmu.Game.Core.Packets.C2G
{
    public class CSSwapCofferItemsPacket : GamePacket
    {
        public CSSwapCofferItemsPacket() : base(CSOffsets.CSSwapCofferItemsPacket, 5)
        {
        }

        public override void Read(PacketStream stream)
        {
            var fromItemId = stream.ReadUInt64();
            var toItemId = stream.ReadUInt64();

            stream.ReadByte();
            var fromSlotType = (SlotType)stream.ReadByte();
            stream.ReadByte();
            var fromSlot = stream.ReadByte();

            stream.ReadByte();
            var toSlotType = (SlotType)stream.ReadByte();
            stream.ReadByte();
            var toSlot = stream.ReadByte();

            var dbDoodadId = stream.ReadUInt64();

            _log.Debug($"SwapCofferItems, Item: {fromItemId} -> {toItemId}, SlotType: {fromSlotType} -> {toSlotType}, Slot: {fromSlot} -> {toSlot}, ItemContainerDbId: {dbDoodadId}");

            if (!Connection.ActiveChar.Inventory.SwapCofferItems(fromItemId, toItemId, fromSlotType, fromSlot, toSlotType, toSlot, dbDoodadId))
            {
                Connection.ActiveChar.SendErrorMessage(ErrorMessageType.CannotMoveSoulboundItemToCoffer); // Not sure what error to send here
            }
        }
    }
}
