using Microsoft.EntityFrameworkCore;

namespace Foliofy.DataBase
{
    public class Database : DbContext
    {
        public Database(DbContextOptions<Database> options) : base(options) { }

    }
}
