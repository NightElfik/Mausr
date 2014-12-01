using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Mausr.Core;
using Mausr.Web.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Newtonsoft.Json;

namespace Mausr.Web.DataContexts {
	public class MausrDb : IdentityDbContext<ApplicationUser> {
		public MausrDb()
			: base("MausrConnection", throwIfV1Schema: false) {
		}

		public static MausrDb Create() {
			return new MausrDb();
		}
			


		public DbSet<Symbol> Symbols { get; set; }

		public DbSet<SymbolDrawing> SymbolDrawings { get; set; }


	}
}