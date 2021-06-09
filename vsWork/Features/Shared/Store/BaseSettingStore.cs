using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace vsWork.Features.Shared.Store
{
    public record SettingState<T>
    {
        public bool Initialized { get; init; }
        public bool Loading { get; init; }
        public T[] ListData { get; init; }
        public T SelectedData { get; set; }
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
