using System;

namespace MHTriServer.Server.Packets
{
    public class PacketHandler
    {
        public virtual void HandleAnsConnection(NetworkSession session, AnsConnection ansConnection) => throw new NotImplementedException();

        public virtual void HandleAnsLineCheck(NetworkSession session, AnsLineCheck ansLineCheck) => throw new NotImplementedException();

        public virtual void HandleNtcCollectionLog(NetworkSession session, NtcCollectionLog collectionLog) => throw new NotImplementedException();

        public virtual void HandleNtcLayerChat(NetworkSession session, NtcLayerChat layerChat) => throw new NotImplementedException();

        public virtual void HandleReqAnnounce(NetworkSession session, ReqAnnounce reqAnnounce) => throw new NotImplementedException();

        public virtual void HandleReqAuthenticationToken(NetworkSession session, ReqAuthenticationToken reqAuthenticationToken) => throw new NotImplementedException();

        public virtual void HandleReqBinaryData(NetworkSession session, ReqBinaryData reqBinaryData) => throw new NotImplementedException();

        public virtual void HandleReqBinaryFoot(NetworkSession session, ReqBinaryFoot reqBinaryFoot) => throw new NotImplementedException();

        public virtual void HandleReqBinaryHead(NetworkSession session, ReqBinaryHead reqBinaryHead) => throw new NotImplementedException();

        public virtual void HandleReqBinaryVersion(NetworkSession session, ReqBinaryVersion reqBinaryVersion) => throw new NotImplementedException();

        public virtual void HandleReqBlackList(NetworkSession session, ReqBlackList reqBlackList) => throw new NotImplementedException();

        public virtual void HandleReqCircleCreate(NetworkSession session, ReqCircleCreate reqCircleCreate) => throw new NotImplementedException();

        public virtual void HandleReqCircleInfo(NetworkSession session, ReqCircleInfo reqCircleInfo) => throw new NotImplementedException();

        public virtual void HandleReqCircleInfoNoticeSet(NetworkSession session, ReqCircleInfoNoticeSet reqCircleInfoNoticeSet) => throw new NotImplementedException();

        public virtual void HandleReqCircleInfoSet(NetworkSession session, ReqCircleInfoSet reqCircleInfoSet) => throw new NotImplementedException();

        public virtual void HandleReqCircleLeave(NetworkSession session, ReqCircleLeave reqCircleLeave) => throw new NotImplementedException();

        public virtual void HandleReqCircleListLayer(NetworkSession session, ReqCircleListLayer reqCircleListLayer) => throw new NotImplementedException();

        public virtual void HandleReqCircleMatchEnd(NetworkSession session, ReqCircleMatchEnd reqCircleMatchEnd) => throw new NotImplementedException();

        public virtual void HandleReqCircleMatchOptionSet(NetworkSession session, ReqCircleMatchOptionSet reqCircleMatchOptionSet) => throw new NotImplementedException();

        public virtual void HandleReqCircleMatchStart(NetworkSession session, ReqCircleMatchStart reqCircleMatchStart) => throw new NotImplementedException();

        public virtual void HandleReqCommonKey(NetworkSession session, ReqCommonKey reqCommonKey) => throw new NotImplementedException();

        public virtual void HandleReqFmpInfo(NetworkSession session, ReqFmpInfo reqFmpInfo) => throw new NotImplementedException();

        public virtual void HandleReqFmpListData(NetworkSession session, ReqFmpListData reqFmpListData) => throw new NotImplementedException();

        public virtual void HandleReqFmpListFoot(NetworkSession session, ReqFmpListFoot reqFmpListFoot) => throw new NotImplementedException();

        public virtual void HandleReqFmpListHead(NetworkSession session, ReqFmpListHead reqFmpListHead) => throw new NotImplementedException();

        public virtual void HandleReqFmpListVersion(NetworkSession session, ReqFmpListVersion reqFmpListVersion) => throw new NotImplementedException();

        public virtual void HandleReqFriendList(NetworkSession session, ReqFriendList reqFriendList) => throw new NotImplementedException();

        public virtual void HandleReqLayerChildInfo(NetworkSession session, ReqLayerChildInfo reqLayerChildInfo) => throw new NotImplementedException();

        public virtual void HandleReqLayerChildListData(NetworkSession session, ReqLayerChildListData reqLayerChildListData) => throw new NotImplementedException();

        public virtual void HandleReqLayerChildListFoot(NetworkSession session, ReqLayerChildListFoot reqLayerChildListFoot) => throw new NotImplementedException();

        public virtual void HandleReqLayerChildListHead(NetworkSession session, ReqLayerChildListHead reqChildListHead) => throw new NotImplementedException();

        public virtual void HandleReqLayerCreateFoot(NetworkSession session, ReqLayerCreateFoot reqLayerCreateFoot) => throw new NotImplementedException();

        public virtual void HandleReqLayerCreateHead(NetworkSession session, ReqLayerCreateHead reqLayerCreateHead) => throw new NotImplementedException();

        public virtual void HandleReqLayerCreateSet(NetworkSession session, ReqLayerCreateSet reqLayerCreateSet) => throw new NotImplementedException();

        public virtual void HandleReqLayerDown(NetworkSession session, ReqLayerDown reqLayerDown) => throw new NotImplementedException();

        public virtual void HandleReqLayerEnd(NetworkSession session, ReqLayerEnd reqLayerEnd) => throw new NotImplementedException();

        public virtual void HandleReqLayerMediationList(NetworkSession session, ReqLayerMediationList reqLayerMediationList) => throw new NotImplementedException();

        public virtual void HandleReqLayerMediationLock(NetworkSession session, ReqLayerMediationLock reqLayerMediationLock) => throw new NotImplementedException();

        public virtual void HandleReqLayerStart(NetworkSession session, ReqLayerStart reqLayerStart) => throw new NotImplementedException();

        public virtual void HandleReqLayerUserList(NetworkSession session, ReqLayerUserList reqLayerUserList) => throw new NotImplementedException();

        public virtual void HandleReqLayerUserListData(NetworkSession session, ReqLayerUserListData reqLayerUserListData) => throw new NotImplementedException();

        public virtual void HandleReqLayerUserListFoot(NetworkSession session, ReqLayerUserListFoot reqLayerUserListFoot) => throw new NotImplementedException();

        public virtual void HandleReqLayerUserListHead(NetworkSession session, ReqLayerUserListHead reqLayerUserListHead) => throw new NotImplementedException();

        public virtual void HandleReqLmpConnect(NetworkSession session, ReqLmpConnect reqLmpConnect) => throw new NotImplementedException();

        public virtual void HandleReqLoginInfo(NetworkSession session, ReqLoginInfo reqLoginInfo) => throw new NotImplementedException();

        public virtual void HandleReqMaintenance(NetworkSession session, ReqMaintenance reqMaintenance) => throw new NotImplementedException();

        public virtual void HandleReqMediaVersionInfo(NetworkSession session, ReqMediaVersionInfo reqMediaVersionInfo) => throw new NotImplementedException();

        public virtual void HandleReqNoCharge(NetworkSession session, ReqNoCharge reqNoCharge) => throw new NotImplementedException();

        public virtual void HandleReqServerTime(NetworkSession session, ReqServerTime reqServerTime) => throw new NotImplementedException();

        public virtual void HandleReqShut(NetworkSession session, ReqShut reqShut) => throw new NotImplementedException();

        public virtual void HandleReqTerms(NetworkSession session, ReqTerms reqTerms) => throw new NotImplementedException();

        public virtual void HandleReqTermsVersion(NetworkSession session, ReqTermsVersion reqTermsVersion) => throw new NotImplementedException();

        public virtual void HandleReqTicketClient(NetworkSession session, ReqTicketClient reqTicketClient) => throw new NotImplementedException();

        public virtual void HandleReqUserBinaryNotice(NetworkSession session, ReqUserBinaryNotice reqUserBinaryNotice) => throw new NotImplementedException();

        public virtual void HandleReqUserBinarySet(NetworkSession session, ReqUserBinarySet reqUserBinarySet) => throw new NotImplementedException();

        public virtual void HandleReqUserListData(NetworkSession session, ReqUserListData reqUserListData) => throw new NotImplementedException();

        public virtual void HandleReqUserListFoot(NetworkSession session, ReqUserListFoot reqUserListFoot) => throw new NotImplementedException();

        public virtual void HandleReqUserListHead(NetworkSession session, ReqUserListHead reqUserListHead) => throw new NotImplementedException();

        public virtual void HandleReqUserObject(NetworkSession session, ReqUserObject reqUserObject) => throw new NotImplementedException();

        public virtual void HandleReqUserSearchInfoMine(NetworkSession session, ReqUserSearchInfoMine reqUserSearchInfoMine) => throw new NotImplementedException();

        public virtual void HandleReqUserSearchSet(NetworkSession session, ReqUserSearchSet reqUserSearchSet) => throw new NotImplementedException();

        public virtual void HandleReqUserStatusSet(NetworkSession session, ReqUserStatusSet reqUserStatusSet) => throw new NotImplementedException();

        public virtual void HandleReqVulgarityInfoLow(NetworkSession session, ReqVulgarityInfoLow reqVulgarityInfoLow) => throw new NotImplementedException();

        public virtual void HandleReqVulgarityLow(NetworkSession session, ReqVulgarityLow reqVulgarityLow) => throw new NotImplementedException();
    }
}
