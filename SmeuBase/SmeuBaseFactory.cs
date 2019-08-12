using System;

namespace SmeuBase
{
    public class SmeuBaseFactory
    {
        private readonly IContextSettingsProvider contextSettings;

        public SmeuBaseFactory(IContextSettingsProvider contextSettings)
        {
            this.contextSettings = contextSettings;
        }

        public SmeuContext GetSmeuBase()
        {
            switch (contextSettings.DbType)
            {
                case DbType.Sqlite:
                    return new SmeuContextSqlite(contextSettings);
                case DbType.MySql:
                    return new SmeuContextMySQL(contextSettings);
                default:
                    throw new ArgumentException($"Given database type is not supported: {contextSettings.DbType}");
            }
        }
    }
}
