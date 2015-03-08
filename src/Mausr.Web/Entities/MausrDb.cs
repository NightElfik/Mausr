using System.Data.Entity;
using System.Data.Entity.Validation;
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

		/// <summary>
		/// This saves the changes without validating the contrains from data annotations.
		/// This is necessary for updates that do not include all dependant properties that are required.
		/// </summary>
		public int SaveChangesNotValidated() {
			bool oldValue = Configuration.ValidateOnSaveEnabled;
			Configuration.ValidateOnSaveEnabled = false;
			int result = SaveChanges();
			Configuration.ValidateOnSaveEnabled = oldValue;
			return result;
		}

		public override int SaveChanges() {
			try {
				return base.SaveChanges();
			}
			catch (DbEntityValidationException e) {
				throw new FormattedDbEntityValidationException(e);
			}
		}

	}
}