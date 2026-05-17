namespace Unity.Collections.LowLevel.Unsafe
{
    using System;

    [AttributeUsage(AttributeTargets.Field)]
    public sealed class NativeDisableUnsafePtrRestrictionAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Struct)]
    public sealed class NativeContainerAttribute : Attribute { }
}

namespace Unity.Collections
{
    using System;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter)]
    public sealed class ReadOnlyAttribute : Attribute { }
}
