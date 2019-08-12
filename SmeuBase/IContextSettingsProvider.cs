using System;
using System.Collections.Generic;
using System.Text;

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
