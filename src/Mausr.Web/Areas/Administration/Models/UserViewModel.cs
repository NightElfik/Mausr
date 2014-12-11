using System.Collections.Generic;
using Mausr.Web.Entities;

namespace Mausr.Web.Areas.Administration.Models {
	public class UserViewModel {

		public ApplicationUser User { get; set; }
		
		public IDictionary<string, string> RoleNamesLookup { get; set; }

	}
}