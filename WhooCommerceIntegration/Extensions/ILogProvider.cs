using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Extensions
{
    public interface ILogProvider
    {
        Guid? ProfileSettingId { get; }

        void InsertServiceLog(IServiceLog serviceLog);
    }

    public interface IServiceLog
    {
        Guid LogId { get; set; }
        DateTime LogDate { get; set; }
        byte Category { get; set; }
        string LogGroup { get; set; }
        string MessageText { get; set; }
        Guid? ProfileSettingId { get; set; }
    }

    public enum MessageSeverity
    {
        Informational = 1,
        Failure = 2,
        Warning = 3,
        Error = 4,
        Success = 5,
        Debug = 6
    }
}
