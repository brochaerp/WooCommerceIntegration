using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Extensions
{
    public class ServiceLog : IServiceLog
    {
        public Guid LogId { get; set; }
        public DateTime LogDate { get; set; }
        public byte Category { get; set; }
        public string LogGroup { get; set; }
        public string MessageText { get; set; }
        public Guid? ProfileSettingId { get; set; }

        public MessageSeverity CategorySeverity
        {
            get
            {
                return (MessageSeverity)Category;
            }
            set
            {
                Category = (byte)value;
            }
        }

        public ServiceLog()
        {
            LogId = Guid.NewGuid();
            LogDate = DateTime.Now;
        }

        public override string ToString()
        {
            return string.Format("{0}:\t {1}", LogDate, MessageText);
        }

    }
}
