namespace IAndOthers.Application.Category.Attributes
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