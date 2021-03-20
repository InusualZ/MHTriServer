namespace MHTriServer.Server
{
    public enum ServerLoginType
    {
        // OPN
        OPN_SERVER_CLOSED = 1,
        OPN_SERVER_MAINTENANCE = 2,
        OPN_SERVER_NORMAL = 3,

        // LMP
        LMP_NORMAL_FIRST = 1, // First log in to the LMP Server
        LMP_NORMAL_SECOND = 2, // Second log in to the LMP Server

        // FMP
        FMP_NORMAL = 3,
        FMP_UNK = 5,

    }
}
