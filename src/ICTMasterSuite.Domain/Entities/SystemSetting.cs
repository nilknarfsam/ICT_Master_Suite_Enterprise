using ICTMasterSuite.Domain.Common;

namespace ICTMasterSuite.Domain.Entities;

public sealed class SystemSetting : EntityBase
{
    public string Category { get; private set; }
    public string Key { get; private set; }
    public string Value { get; private set; }

    public SystemSetting(string category, string key, string value)
    {
        Category = category;
        Key = key;
        Value = value;
    }

    public void UpdateValue(string value)
    {
        Value = value;
        Touch();
    }
}
