using System.Collections.Generic;
using System.Linq;
using Mausr.Web.Entities;
using Mausr.Web.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Mausr.Web.Areas.Administration.Models {
	public class UsersViewModel {

		public IDictionary<string, string> RoleNamesLookup { get; set; }

		public IPagination<ApplicationUser> Users { get; set; }

	}
}