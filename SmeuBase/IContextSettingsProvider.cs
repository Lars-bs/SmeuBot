namespace SmeuBase
{
    public interface IContextSettingsProvider
    {
        DbType DbType { get; }
        string ConnectionString { get; }
    }

    public enum DbType
    {
        Sqlite, MySql
    }
}
