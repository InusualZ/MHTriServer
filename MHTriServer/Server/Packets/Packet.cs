using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace MHTriServer.Server.Packets
{
    public abstract class Packet
    {
        private static readonly Dictionary<uint, Func<uint, ushort, ushort, Packet>> m_PacketFactoryMap =
            new Dictionary<uint, Func<uint, ushort, ushort, Packet>>();

        public ushort Size { get; protected set; }

        public ushort Counter { get; protected set; }

        // NOTE: Technically this is wrong since the packet id consist only of 3 bytes, the 4 bytes is a custom one that is always 0
        public uint ID { get; protected set; }

        protected Packet(uint id) => (ID, Counter, Size) = (id, 0, 0);

        protected Packet(uint id, ushort size, ushort counter) => (ID, Counter, Size) = (id, counter, size);

        public virtual void Serialize(ExtendedBinaryWriter writer)
        {
            writer.Write(Size);
            writer.Write(Counter);
            writer.Write(ID);
        }

        public virtual void Deserialize(ExtendedBinaryReader reader)
        {
            // Packet Specific
        }

        public override string ToString()
        {
            return $"{GetType().Name}({ID:X8}; {Size} bytes)";
        }

        static Packet()
        {
            var constructorTypes = new[] { typeof(uint), typeof(ushort), typeof(ushort) };
            Func<uint, ushort, ushort, T> CreateFunc<T>() where T : Packet
            {
                var ctr = typeof(T).GetConstructor(constructorTypes);
                var idParameter = Expression.Parameter(typeof(uint));
                var sizeParameter = Expression.Parameter(typeof(ushort));
                var counterParameter = Expression.Parameter(typeof(ushort));
                var @params = new ParameterExpression[] { idParameter, sizeParameter, counterParameter };
                var creatorExpression = Expression.Lambda<Func<uint, ushort, ushort, T>>(Expression.New(ctr, @params), @params);
                return creatorExpression.Compile();
            }

            void RegisterWith<T>(params uint[] ids) where T : Packet
            {
                var func = CreateFunc<T>();
                foreach(var id in ids)
                {
                    m_PacketFactoryMap.Add(id, func);
                }
            }

            RegisterWith<AnsAuthenticationToken>(AnsAuthenticationToken.PACKET_ID);
            RegisterWith<AnsBinaryData>(AnsBinaryData.PACKET_ID);
            RegisterWith<AnsBinaryFoot>(AnsBinaryFoot.PACKET_ID);
            RegisterWith<AnsBinaryHead>(AnsBinaryHead.PACKET_ID);
            RegisterWith<AnsBinaryVersion>(AnsBinaryVersion.PACKET_ID);
            RegisterWith<AnsBlackList>(AnsBlackList.PACKET_ID);
            RegisterWith<AnsCircleInfoNoticeSet>(AnsCircleInfoNoticeSet.PACKET_ID);
            RegisterWith<AnsConnection>(AnsConnection.PACKET_ID);
            RegisterWith<AnsFmpInfo>(AnsFmpInfo.PACKET_ID, AnsFmpInfo.PACKET_ID_FMP);
            RegisterWith<AnsFmpListData>(AnsFmpListData.PACKET_ID, AnsFmpListData.PACKET_ID_FMP);
            RegisterWith<AnsFmpListFoot>(AnsFmpListFoot.PACKET_ID, AnsFmpListFoot.PACKET_ID_FMP);
            RegisterWith<AnsFmpListHead>(AnsFmpListHead.PACKET_ID, AnsFmpListHead.PACKET_ID_FMP);
            RegisterWith<AnsFriendList>(AnsFriendList.PACKET_ID);
            RegisterWith<AnsFmpListVersion>(AnsFmpListVersion.PACKET_ID, AnsFmpListVersion.PACKET_ID_FMP);
            RegisterWith<AnsLayerChildInfo>(AnsLayerChildInfo.PACKET_ID);
            RegisterWith<AnsLayerChildListData>(AnsLayerChildListData.PACKET_ID);
            RegisterWith<AnsLayerChildListFoot>(AnsLayerChildListFoot.PACKET_ID);
            RegisterWith<AnsLayerChildListHead>(AnsLayerChildListHead.PACKET_ID);
            RegisterWith<AnsLayerDown>(AnsLayerDown.PACKET_ID);
            RegisterWith<AnsLayerEnd>(AnsLayerEnd.PACKET_ID);
            RegisterWith<AnsLayerStart>(AnsLayerStart.PACKET_ID);
            RegisterWith<AnsLayerUserList>(AnsLayerUserList.PACKET_ID);
            RegisterWith<AnsLoginInfo>(AnsLoginInfo.PACKET_ID);
            RegisterWith<AnsLoginInfoSucess>(AnsLoginInfoSucess.PACKET_ID);
            RegisterWith<AnsLoginSuccess>(AnsLoginSuccess.PACKET_ID);
            RegisterWith<AnsLoginSuccess2>(AnsLoginSuccess2.PACKET_ID);
            RegisterWith<AnsMaintenance>(AnsMaintenance.PACKET_ID);
            RegisterWith<AnsServerTime>(AnsServerTime.PACKET_ID);
            RegisterWith<AnsServerWrong>(AnsServerWrong.PACKET_ID);
            RegisterWith<AnsShut>(AnsShut.PACKET_ID);
            RegisterWith<AnsTicket>(AnsTicket.PACKET_ID);
            RegisterWith<AnsUserBinarySet>(AnsUserBinarySet.PACKET_ID);
            RegisterWith<AnsUserListData>(AnsUserListData.PACKET_ID);
            RegisterWith<AnsUserListDataSucess>(AnsUserListDataSucess.PACKET_ID);
            RegisterWith<AnsUserListFoot>(AnsUserListFoot.PACKET_ID);
            RegisterWith<AnsUserListHead>(AnsUserListHead.PACKET_ID);
            RegisterWith<AnsUserObject>(AnsUserObject.PACKET_ID);
            RegisterWith<AnsUserSearchInfoMine>(AnsUserSearchInfoMine.PACKET_ID);
            RegisterWith<AnsUserSearchSet>(AnsUserSearchSet.PACKET_ID);
            RegisterWith<AnsUserSelectedSlot>(AnsUserSelectedSlot.PACKET_ID);
            RegisterWith<AnsUserStatusSet>(AnsUserStatusSet.PACKET_ID);
            RegisterWith<LmpConnect>(LmpConnect.PACKET_ID);
            RegisterWith<NtcLayerUserNum>(NtcLayerUserNum.PACKET_ID);
            RegisterWith<NtcLogin>(NtcLogin.PACKET_ID);
            RegisterWith<ReqBinaryData>(ReqBinaryData.PACKET_ID);
            RegisterWith<ReqBinaryFoot>(ReqBinaryFoot.PACKET_ID);
            RegisterWith<ReqBinaryHead>(ReqBinaryHead.PACKET_ID);
            RegisterWith<ReqBinaryVersion>(ReqBinaryVersion.PACKET_ID);
            RegisterWith<ReqBlackList>(ReqBlackList.PACKET_ID);
            RegisterWith<ReqCircleInfoNoticeSet>(ReqCircleInfoNoticeSet.PACKET_ID);
            RegisterWith<ReqConnection>(ReqConnection.PACKET_ID);
            RegisterWith<ReqFmpInfo>(ReqFmpInfo.PACKET_ID, ReqFmpInfo.PACKET_ID_FMP);
            RegisterWith<ReqFmpListData>(ReqFmpListData.PACKET_ID, ReqFmpListData.PACKET_ID_FMP);
            RegisterWith<ReqFmpListFoot>(ReqFmpListFoot.PACKET_ID, ReqFmpListFoot.PACKET_ID_FMP);
            RegisterWith<ReqFmpListHead>(ReqFmpListHead.PACKET_ID, ReqFmpListHead.PACKET_ID_FMP);
            RegisterWith<ReqFmpListVersion>(ReqFmpListVersion.PACKET_ID, ReqFmpListVersion.PACKET_ID_FMP);
            RegisterWith<ReqFriendList>(ReqFriendList.PACKET_ID);
            RegisterWith<ReqLayerChildInfo>(ReqLayerChildInfo.PACKET_ID);
            RegisterWith<ReqLayerChildListData>(ReqLayerChildListData.PACKET_ID);
            RegisterWith<ReqLayerChildListFoot>(ReqLayerChildListFoot.PACKET_ID);
            RegisterWith<ReqLayerChildListHead>(ReqLayerChildListHead.PACKET_ID);
            RegisterWith<ReqLayerDown>(ReqLayerDown.PACKET_ID);
            RegisterWith<ReqLayerEnd>(ReqLayerEnd.PACKET_ID);
            RegisterWith<ReqLayerStart>(ReqLayerStart.PACKET_ID);
            RegisterWith<ReqLayerUserList>(ReqLayerUserList.PACKET_ID);
            RegisterWith<ReqLineCheck>(ReqLineCheck.PACKET_ID);
            RegisterWith<ReqShut>(ReqShut.PACKET_ID);
            RegisterWith<ReqUnknownCheck>(ReqUnknownCheck.PACKET_ID);
            RegisterWith<ReqUserBinarySet>(ReqUserBinarySet.PACKET_ID);
            RegisterWith<ReqUserListData>(ReqUserListData.PACKET_ID);
            RegisterWith<ReqUserObjects>(ReqUserObjects.PACKET_ID);
            RegisterWith<ReqUserSearchInfoMine>(ReqUserSearchInfoMine.PACKET_ID);
            RegisterWith<ReqUserSearchSet>(ReqUserSearchSet.PACKET_ID);
            RegisterWith<ReqUserStatusSet>(ReqUserStatusSet.PACKET_ID);
            RegisterWith<RfpConnect>(RfpConnect.PACKET_ID);
            RegisterWith<Timeout>(Timeout.PACKET_ID);

        }

        public static Packet CreateFrom(uint id, ushort size, ushort counter)
        {
            if (!m_PacketFactoryMap.TryGetValue(id, out var constructor))
            {
                return null;
            }

            var packet = constructor(id, size, counter);
            Debug.Assert(packet != null);
            return packet;
        }

        public static string Hexstring(byte[] slice, char? separator, int expectedSize = 0)
        {
            var lineBuilder = new StringBuilder();

            if (expectedSize == 0)
            {
                expectedSize = slice.Length;
            }

            for (var x = 0; x < expectedSize; ++x)
            {
                if (x < slice.Length)
                {
                    lineBuilder.Append($"{slice[x]:X2}");
                }
                else
                {
                    lineBuilder.Append("  ");
                }
                lineBuilder.Append(separator);
            }
            return lineBuilder.ToString();
        }

        public static void Hexdump(byte[] bytes)
        {
            static char Convert2ASCII(byte val)
            {
                if (0x20 <= val && val < 0x7F)
                {
                    return (char)val;
                }

                return '.';
            }

            var lineBuilder = new StringBuilder();
            for (var i = 0; i < bytes.Length; i += 16)
            {
                var endIndex = Math.Min(i + 16, bytes.Length);
                var slice = bytes[i..endIndex];

                lineBuilder.Append($"{i:X8} | ");

                lineBuilder.Append(Hexstring(slice, ' ', 16));

                lineBuilder.Append("| ");
                lineBuilder.Append(slice.Select(b => Convert2ASCII(b)).ToArray());

                Console.WriteLine(lineBuilder.ToString());

                lineBuilder.Clear();
            }
        }
    }
}
