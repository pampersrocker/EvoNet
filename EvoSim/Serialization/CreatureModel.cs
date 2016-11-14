using EvoNet.Objects;
using System;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using SQLite.CodeFirst;
using System.Data.SQLite;

namespace EvoSim.Serialization
{

    public class CreatureModel : DbContext
    {
        // Your context has been configured to use a 'CreatureModel' connection string from your application's 
        // configuration file (App.config or Web.config). By default, this connection string targets the 
        // 'EvoSim.Serialization.CreatureModel' database on your LocalDb instance. 
        // 
        // If you wish to target a different database and/or database provider, modify the 'CreatureModel' 
        // connection string in the application configuration file.
        public CreatureModel(DbConnection connection)
            : base(connection, false)
        {
        }

        public CreatureModel()
            : this(new SQLiteConnection("Data Source=./EvoDatabase.sqlite"))
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //base.OnModelCreating(modelBuilder);
            var creator = new SqliteCreateDatabaseIfNotExists<CreatureModel>(modelBuilder);
            Database.SetInitializer(creator);
        }

        // Add a DbSet for each entity type that you want to include in your model. For more information 
        // on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.

        public virtual DbSet<Creature> Creatures { get; set; }
    }

    //public class MyEntity
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //}
}