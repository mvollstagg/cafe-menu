namespace IAndOthers.Core.Mvc.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class IOMultiCheckBoxAttribute : Attribute
    {
        public string ViewDataKey { get; }

        public IOMultiCheckBoxAttribute(string viewDataKey)
        {
            ViewDataKey = viewDataKey;
        }
    }
}