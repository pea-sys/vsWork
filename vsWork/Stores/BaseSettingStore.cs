using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace vsWork.Stores
{
    public record BaseSettingState
    {
        public bool Initialized { get; init; }
        public bool Loading { get; init; }
        public SettingMode Mode { get; set; }
    }

    public enum SettingMode
    {
        None,
        Add,
        Update,
        Delete
    }
}
