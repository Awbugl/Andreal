using System;

namespace Andreal.Window.Common;

public static class Utils
{
    public class DataFriendlyNameAttribute : Attribute
    {
        public DataFriendlyNameAttribute(string dataFriendlyName)
        {
            DataFriendlyName = dataFriendlyName;
        }

        public string DataFriendlyName { get; }
    }
}
