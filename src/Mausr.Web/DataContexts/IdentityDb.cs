using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Mausr.Web.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Mausr.Web.DataContexts {
	public class IdentityDb : IdentityDbContext<ApplicationUser> {
		public IdentityDb()
			: base("IdentityConnection", throwIfV1Schema: false) {
		}

		public static IdentityDb Create() {
			return new IdentityDb();
		}
	}
}