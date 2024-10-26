namespace IAndOthers.Core.Mvc.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class IODropDownAttribute : Attribute
    {
        public string ViewDataKey { get; }

        public IODropDownAttribute(string viewDataKey)
        {
            ViewDataKey = viewDataKey;
        }
    }
}