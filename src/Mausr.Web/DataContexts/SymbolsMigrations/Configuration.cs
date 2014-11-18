namespace Mausr.Web.DataContexts.SymbolsMigrations {
	using System;
	using System.Data.Entity;
	using System.Data.Entity.Migrations;
	using System.Linq;

	internal sealed class Configuration : DbMigrationsConfiguration<SymbolsDb> {
		public Configuration() {
			AutomaticMigrationsEnabled = true;
			MigrationsDirectory = @"DataContexts\SymbolsMigrations";
		}

		protected override void Seed(SymbolsDb context) {
			//  This method will be called after migrating to the latest version.

			//  You can use the DbSet<T>.AddOrUpdate() helper extension method 
			//  to avoid creating duplicate seed data. E.g.
			//
			//    context.People.AddOrUpdate(
			//      p => p.FullName,
			//      new Person { FullName = "Andrew Peters" },
			//      new Person { FullName = "Brice Lambson" },
			//      new Person { FullName = "Rowan Miller" }
			//    );
			//
		}
	}
}
