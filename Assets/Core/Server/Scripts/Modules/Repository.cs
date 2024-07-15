using System;

namespace MMC.Server
{
    // access to data and database
    public class Repository : Module
    {
        [UseModule] public Database database;
    }

    public sealed class RepositoryAttribute : ModuleAttribute
    {
    }
}