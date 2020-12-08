using System;

namespace Xels.SmartContracts
{
    /// <summary>
    /// Specifies that the value of a field within a log data structure can be used to search for receipts.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class IndexAttribute : Attribute
    {
    }
}
