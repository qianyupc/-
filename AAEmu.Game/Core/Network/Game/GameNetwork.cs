using System;
using System.Net;
using AAEmu.Commons.Network.Type;
using AAEmu.Commons.Utils;
using AAEmu.Game.Core.Packets.C2G;
using AAEmu.Game.Core.Packets.Proxy;
using AAEmu.Game.Models;
using NLog;

namespace AAEmu.Game.Core.Network.Game
{
    public class GameNetwork : Singleton<GameNetwork>
    {
        private Server _server;
        private GameProtocolHandler _handler;
        private static Logger _log = LogManager.GetCurrentClassLogger();

        private GameNetwork()
        {
            _handler = new GameProtocolHandler();

            // World
            RegisterPacket(0x000, 1, typeof(X2EnterWorldPacket));
            RegisterPacket(0x001, 1, typeof(CSLeaveWorldPacket));
            RegisterPacket(0x002, 1, typeof(CSCancelLeaveWorldPacket));
            RegisterPacket(0x004, 1, typeof(CSCreateExpeditionPacket));
            RegisterPacket(0x005, 1, typeof(CSChangeExpeditionSponsorPacket));
            RegisterPacket(0x006, 1, typeof(CSChangeExpeditionRolePolicyPacket));
            RegisterPacket(0x007, 1, typeof(CSChangeExpeditionMemberRolePacket));
            RegisterPacket(0x008, 1, typeof(CSChangeExpeditionOwnerPacket));
            RegisterPacket(0x009, 1, typeof(CSRenameExpeditionPacket));
            RegisterPacket(0x00b, 1, typeof(CSDismissExpeditionPacket));
            RegisterPacket(0x00c, 1, typeof(CSInviteToExpeditionPacket));
            RegisterPacket(0x00d, 1, typeof(CSReplyExpeditionInvitationPacket));
            RegisterPacket(0x00e, 1, typeof(CSLeaveExpeditionPacket));
            RegisterPacket(0x00f, 1, typeof(CSKickFromExpeditionPacket));
            RegisterPacket(0x011, 1, typeof(CSUpdateDominionTaxRatePacket));
            RegisterPacket(0x012, 1, typeof(CSUpdateNationalTaxRatePacket));
            RegisterPacket(0x014, 1, typeof(CSFactionImmigrationInvitePacket));
            RegisterPacket(0x015, 1, typeof(CSFactionImmigrationInviteReplyPacket));
            RegisterPacket(0x016, 1, typeof(CSFactionImmigrateToOriginPacket));
            RegisterPacket(0x017, 1, typeof(CSFactionKickToOriginPacket));
            RegisterPacket(0x018, 1, typeof(CSFactionDeclareHostilePacket));
            RegisterPacket(0x019, 1, typeof(CSFamilyInviteMemberPacket));
            RegisterPacket(0x01a, 1, typeof(CSFamilyReplyInvitationPacket));
            RegisterPacket(0x01b, 1, typeof(CSFamilyLeavePacket));
            RegisterPacket(0x01c, 1, typeof(CSFamilyKickPacket));
            RegisterPacket(0x01d, 1, typeof(CSFamilyChangeTitlePacket));
            RegisterPacket(0x01e, 1, typeof(CSFamilyChangeOwnerPacket));
            RegisterPacket(0x01f, 1, typeof(CSListCharacterPacket));
            RegisterPacket(0x020, 1, typeof(CSRefreshInCharacterListPacket));
            RegisterPacket(0x021, 1, typeof(CSCreateCharacterPacket));
            RegisterPacket(0x022, 1, typeof(CSEditCharacterPacket));
            RegisterPacket(0x023, 1, typeof(CSDeleteCharacterPacket));
            RegisterPacket(0x024, 1, typeof(CSSelectCharacterPacket));
            RegisterPacket(0x025, 1, typeof(CSSpawnCharacterPacket));
            RegisterPacket(0x026, 1, typeof(CSCancelCharacterDeletePacket));
            RegisterPacket(0x027, 1, typeof(CSNotifyInGamePacket));
            RegisterPacket(0x028, 1, typeof(CSNotifyInGameCompletedPacket));
            RegisterPacket(0x029, 1, typeof(CSEditorGameModePacket));
            RegisterPacket(0x02a, 1, typeof(CSChangeTargetPacket));
            RegisterPacket(0x02b, 1, typeof(CSRequestCharBriefPacket));
            RegisterPacket(0x02c, 1, typeof(CSSpawnSlavePacket));
            RegisterPacket(0x02d, 1, typeof(CSDespawnSlavePacket));
            RegisterPacket(0x02e, 1, typeof(CSDestroySlavePacket));
            RegisterPacket(0x02f, 1, typeof(CSBindSlavePacket));
            RegisterPacket(0x030, 1, typeof(CSDiscardSlavePacket));
            RegisterPacket(0x031, 1, typeof(CSChangeSlaveTargetPacket));
            RegisterPacket(0x032, 1, typeof(CSChangeSlaveNamePacket));
            RegisterPacket(0x033, 1, typeof(CSRepairSlaveItemsPacket));
            RegisterPacket(0x034, 1, typeof(CSTurretStatePacket));
            RegisterPacket(0x035, 1, typeof(CSChangeSlaveEquipmentPacket));
            RegisterPacket(0x036, 1, typeof(CSDestroyItemPacket));
            RegisterPacket(0x037, 1, typeof(CSSplitBagItemPacket));
            RegisterPacket(0x038, 1, typeof(CSSwapItemsPacket));
            RegisterPacket(0x03a, 1, typeof(CSRepairSingleEquipmentPacket));
            RegisterPacket(0x03b, 1, typeof(CSRepairAllEquipmentsPacket));
            RegisterPacket(0x03d, 1, typeof(CSSplitCofferItemPacket));
            RegisterPacket(0x03e, 1, typeof(CSSwapCofferItemsPacket));
            RegisterPacket(0x03f, 1, typeof(CSExpandSlotsPacket));
            RegisterPacket(0x045, 1, typeof(CSDepositMoneyPacket));
            RegisterPacket(0x046, 1, typeof(CSWithdrawMoneyPacket));
            RegisterPacket(0x04c, 1, typeof(CSResurrectCharacterPacket));
            RegisterPacket(0x04d, 1, typeof(CSSetForceAttackPacket));
            RegisterPacket(0x050, 1, typeof(CSStartSkillPacket));
            RegisterPacket(0x052, 1, typeof(CSStopCastingPacket));
            RegisterPacket(0x053, 1, typeof(CSRemoveBuffPacket));
            RegisterPacket(0x054, 1, typeof(CSConstructHouseTaxPacket));
            RegisterPacket(0x061, 1, typeof(CSSendChatMessagePacket));
            RegisterPacket(0x063, 1, typeof(CSInteractNPCPacket));
            RegisterPacket(0x064, 1, typeof(CSInteractNPCEndPacket));
            RegisterPacket(0x088, 1, typeof(CSMoveUnitPacket));
            RegisterPacket(0x08a, 1, typeof(CSCreateSkillControllerPacket));
            RegisterPacket(0x08b, 1, typeof(CSActiveWeaponChangedPacket));
            RegisterPacket(0x092, 1, typeof(CSLearnSkillPacket));
            RegisterPacket(0x093, 1, typeof(CSLearnBuffPacket));
            RegisterPacket(0x094, 1, typeof(CSResetSkillsPacket));
            RegisterPacket(0x096, 1, typeof(CSSwapAbilityPacket));
            RegisterPacket(0x098, 1, typeof(CSSendMailPacket));
            RegisterPacket(0x099, 1, typeof(CSListMailPacket));
            RegisterPacket(0x09a, 1, typeof(CSListMailContinuePacket));
            RegisterPacket(0x09b, 1, typeof(CSReadMailPacket));
            RegisterPacket(0x09c, 1, typeof(CSTakeAttachmentItemPacket));
            RegisterPacket(0x09d, 1, typeof(CSTakeAttachmentMoneyPacket));
            RegisterPacket(0x09e, 1, typeof(CSPayChargeMoneyPacket));
            RegisterPacket(0x09f, 1, typeof(CSDeleteMailPacket));
            RegisterPacket(0x0a0, 1, typeof(CSReportSpamPacket));
            RegisterPacket(0x0a1, 1, typeof(CSReturnMailPacket));
            RegisterPacket(0x0aa, 1, typeof(CSBuyItemsPacket));
            RegisterPacket(0x0ab, 1, typeof(CSBuyCoinItemPacket));
            RegisterPacket(0x0ac, 1, typeof(CSSellItemsPacket));
            RegisterPacket(0x0ad, 1, typeof(CSListSoldItemPacket));
            RegisterPacket(0x0b2, 1, typeof(CSUpdateActionSlotPacket));
            RegisterPacket(0x0d1, 1, typeof(CSStartQuestContextPacket));
            RegisterPacket(0x0d2, 1, typeof(CSCompleteQuestContextPacket));
            RegisterPacket(0x0d3, 1, typeof(CSDropQuestContextPacket));
            RegisterPacket(0x0d4, 1, typeof(CSResetQuestContextPacket));
            RegisterPacket(0x0d5, 1, typeof(CSAcceptCheatQuestContextPacket));
            RegisterPacket(0x0da, 1, typeof(CSUsePortalPacket));
            RegisterPacket(0x0db, 1, typeof(CSDeletePortalPacket));
            RegisterPacket(0x0dc, 1, typeof(CSInstanceLoadedPacket));
            RegisterPacket(0x0c9, 1, typeof(CSUnbondDoodadPacket));
            RegisterPacket(0x0f2, 1, typeof(CSSaveTutorialPacket));
            RegisterPacket(0x0f5, 1, typeof(CSExecuteCraft));
            RegisterPacket(0x0f6, 1, typeof(CSChangeAppellationPacket));
            RegisterPacket(0x0fb, 1, typeof(CSSetLpManageCharacterPacket));
            RegisterPacket(0x0fc, 1, typeof(CSUpgradeExpertLimitPacket));
            RegisterPacket(0x0fd, 1, typeof(CSDowngradeExpertLimitPacket));
            RegisterPacket(0x0fe, 1, typeof(CSExpandExpertPacket));
            RegisterPacket(0x100, 1, typeof(CSSearchListPacket));
            RegisterPacket(0x101, 1, typeof(CSAddFriendPacket));
            RegisterPacket(0x102, 1, typeof(CSDeleteFriendPacket));
            RegisterPacket(0x103, 1, typeof(CSCharDetailPacket));
            RegisterPacket(0x104, 1, typeof(CSAddBlockedUserPacket));
            RegisterPacket(0x105, 1, typeof(CSDeleteBlockedUserPacket));
            RegisterPacket(0x10f, 1, typeof(CSNotifySubZonePacket));
            RegisterPacket(0x113, 1, typeof(CSRequestUIDataPacket));
            RegisterPacket(0x114, 1, typeof(CSSaveUIDataPacket));
            RegisterPacket(0x115, 1, typeof(CSBroadcastVisualOptionPacket));
            RegisterPacket(0x116, 1, typeof(CSRestrictCheckPacket));
            RegisterPacket(0x12e, 1, typeof(CSIdleStatusPacket));
            RegisterPacket(0x136, 1, typeof(CSPremiumServieceMsgPacket));

            // Proxy
            RegisterPacket(0x000, 2, typeof(ChangeStatePacket));
            RegisterPacket(0x001, 2, typeof(FinishStatePacket));
            RegisterPacket(0x002, 2, typeof(FlushMsgsPacket));
            RegisterPacket(0x004, 2, typeof(UpdatePhysicsTimePacket));
            RegisterPacket(0x005, 2, typeof(BeginUpdateObjPacket));
            RegisterPacket(0x006, 2, typeof(EndUpdateObjPacket));
            RegisterPacket(0x007, 2, typeof(BeginBindObjPacket));
            RegisterPacket(0x008, 2, typeof(EndBindObjPacket));
            RegisterPacket(0x009, 2, typeof(UnbindPredictedObjPacket));
            RegisterPacket(0x00A, 2, typeof(RemoveStaticObjPacket));
            RegisterPacket(0x00B, 2, typeof(VoiceDataPacket));
            RegisterPacket(0x00C, 2, typeof(UpdateAspectPacket));
            RegisterPacket(0x00D, 2, typeof(SetAspectProfilePacket));
            RegisterPacket(0x00E, 2, typeof(PartialAspectPacket));
            RegisterPacket(0x00F, 2, typeof(SetGameTypePacket));
            RegisterPacket(0x010, 2, typeof(ChangeCVarPacket));
            RegisterPacket(0x011, 2, typeof(EntityClassRegistrationPacket));
            RegisterPacket(0x012, 2, typeof(PingPacket));
            RegisterPacket(0x013, 2, typeof(PongPacket));
            RegisterPacket(0x014, 2, typeof(PacketSeqChange));
            RegisterPacket(0x015, 2, typeof(FastPingPacket));
            RegisterPacket(0x016, 2, typeof(FastPongPacket));
        }

        public void Start()
        {
            var config = AppConfiguration.Instance.Network;
            _server = new Server(new IPEndPoint(config.Host.Equals("*") ? IPAddress.Any : IPAddress.Parse(config.Host), config.Port), 10);
            _server.SetHandler(_handler);
            _server.Start();

            _log.Info("Network started");
        }

        public void Stop()
        {
            if (_server.IsStarted)
                _server.Stop();
            
            _log.Info("Network stoped");
        }

        private void RegisterPacket(uint type, byte level, Type classType)
        {
            _handler.RegisterPacket(type, level, classType);
        }
    }
}
