namespace MHTriServer.Server
{
    public class Constants
    {
        public const uint TERMS_AND_CONDITIONS_VERSION = 1;
        public const string TERMS_AND_CONDITIONS = "Don't cheat\nBe kind\nRespect other humans\nHAVE FUN!";

        public const string MAINTENANCE_MESSAGE = "<BODY><CENTER><SIZE=62>MHTri Server<BR><BODY>Custom Server Implementation<BR><BR><BODY><LEFT>Special Thanks<BR><BODY>" +
            "<COLOR=2>Sepalani<BR><BODY>Dolphin Emulator Team<BR><BR><BODY><CENTER><SIZE=40><COLOR=7>Without their work this project would not be possible<END>";

        public const string ANNOUNCEMENT_MESSAGE = "<BODY><CENTER><SIZE=62>MHTri Server<BR><BODY>Custom Server Implementation<BR><BR><BODY><LEFT>Special Thanks<BR><BODY>" +
            "<COLOR=2>Sepalani<BR><BODY>Dolphin Emulator Team<BR><BR><BODY><CENTER><SIZE=40><COLOR=7>Without their work this project would not be possible<END>";

        public const string SERVER_CLOSED_MESSAGE = "<LF=5><BODY><CENTER><COLOR=2>Server Closed<END>";

        public const string OUT_OF_ORDER_ERROR_MESSAGE = "<LF=5><BODY><CENTER><COLOR=1>YOU DON'T BELONG HERE!<END>";
        public const string INACTIVITY_MESSAGE = "<LF=5><BODY><CENTER>Kicked due to network inactivity<END>";
        public const string MULTIPLE_ACCOUNT_ERROR_MESSAGE = "<LF=5><BODY><CENTER><COLOR=1>There is already a player online with this account<END>";
        public const string MAX_ACCOUNT_REACHED_ERROR_MESSAGE = "<LF=5><BODY><CENTER><COLOR=3>Server maximum account have bean reach<END>";
        public const string PLAYER_NOT_LOADED_ERROR_MESSAGE = "<LF=5><BODY><CENTER><COLOR=3>You tried to skip steps...<END>";
    }
}
