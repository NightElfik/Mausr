using System.Data.Entity;
using Mausr.Web.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Mausr.Web.Entities {
	public class MausrDb : IdentityDbContext<ApplicationUser> {
		public MausrDb()
			: base("MausrConnection", throwIfV1Schema: false) {
		}

		public static MausrDb Create() {
			return new MausrDb();
		}
			


		public DbSet<Symbol> Symbols { get; set; }

		public DbSet<SymbolDrawing> SymbolDrawings { get; set; }

		public DbSet<Drawing> Drawings { get; set; }

	}
}