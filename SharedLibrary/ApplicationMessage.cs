using System;
using System.Collections.Generic;

namespace SharedLibrary
{
    public class ApplicationMessage
    {
        public DateTime SyncTime { get; set; }
        public List<int> SyncIDs { get; set; }
    }
}
