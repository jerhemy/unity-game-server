using System.Collections.Concurrent;
using System.Collections.Generic;

namespace NetcodeIO.NET
{
    public class InstanceDefinition
    {
        private string name;
        private string longname;

        private ConcurrentDictionary<ulong, dynamic> _mobs;

        private List<dynamic> _instanceObjects;
        
        private InstanceDefinition()
        {
            
        }

    }
}