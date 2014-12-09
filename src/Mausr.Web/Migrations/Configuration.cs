using System.Data.Entity.Migrations;
using System.Linq;
using Mausr.Web.DataContexts;
using Mausr.Web.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Mausr.Web.Migrations {

	internal sealed class Configuration : DbMigrationsConfiguration<MausrDb> {
		public Configuration() {
			AutomaticMigrationsEnabled = true;
		}

		protected override void Seed(MausrDb db) {

			if (!db.Roles.Any(r => r.Name == RolesHelper.Admin)) {
				var store = new RoleStore<IdentityRole>(db);
				var manager = new RoleManager<IdentityRole>(store);
				var role = new IdentityRole { Name = RolesHelper.Admin };
				manager.Create(role);
			}

			if (!db.Roles.Any(r => r.Name == RolesHelper.Trainer)) {
				var store = new RoleStore<IdentityRole>(db);
				var manager = new RoleManager<IdentityRole>(store);
				var role = new IdentityRole { Name = RolesHelper.Trainer };
				manager.Create(role);
			}

			if (!db.Roles.Any(r => r.Name == RolesHelper.Teacher)) {
				var store = new RoleStore<IdentityRole>(db);
				var manager = new RoleManager<IdentityRole>(store);
				var role = new IdentityRole { Name = RolesHelper.Teacher };
				manager.Create(role);
			}

			if (!db.Users.Any(u => u.UserName == "NightElfik")) {
				var store = new UserStore<ApplicationUser>(db);
				var manager = new UserManager<ApplicationUser>(store);
				var user = new ApplicationUser {
					UserName = "NightElfik",
					Email = "NightElfik@mausr.com",
				};

				manager.Create(user, "ChangeItAsap!");
				manager.AddToRole(user.Id, RolesHelper.Admin);
				manager.AddToRole(user.Id, RolesHelper.Trainer);
				manager.AddToRole(user.Id, RolesHelper.Teacher);
			}

			if (!db.Users.Any(u => u.UserName == "Trainer")) {
				var store = new UserStore<ApplicationUser>(db);
				var manager = new UserManager<ApplicationUser>(store);
				var user = new ApplicationUser {
					UserName = "Trainer",
					Email = "Trainer@mausr.com",
				};

				manager.Create(user, "PleaseDoN0tShare");
				manager.AddToRole(user.Id, RolesHelper.Trainer);
				manager.AddToRole(user.Id, RolesHelper.Teacher);
			}

			if (!db.Users.Any(u => u.UserName == "Teacher")) {
				var store = new UserStore<ApplicationUser>(db);
				var manager = new UserManager<ApplicationUser>(store);
				var user = new ApplicationUser {
					UserName = "Teacher",
					Email = "Teacher@mausr.com",
				};

				manager.Create(user, "DontShareP1ease");
				manager.AddToRole(user.Id, RolesHelper.Teacher);
			}

			// Turn on case-sensitive (binary) comparison.
			// Default locale is case insensitive and treats symbols Σ, ς, and σ as identical.
			db.Database.ExecuteSqlCommand(
				@"ALTER TABLE Symbols ALTER COLUMN SymbolStr NVARCHAR(2) COLLATE SQL_Latin1_General_CP850_BIN NOT NULL");

			db.Symbols.AddOrUpdate(
				s => s.SymbolStr,
				//new Symbol { SymbolStr = "Γ", Name = "Greek Capital Letter Gamma" },
				//new Symbol { SymbolStr = "Δ", Name = "Greek Capital Letter Delta" },
				//new Symbol { SymbolStr = "Θ", Name = "Greek Capital Letter Theta" },
				//new Symbol { SymbolStr = "Λ", Name = "Greek Capital Letter Lamda" },
				//new Symbol { SymbolStr = "Ξ", Name = "Greek Capital Letter Xi" },
				//new Symbol { SymbolStr = "Π", Name = "Greek Capital Letter Pi" },
				//new Symbol { SymbolStr = "Σ", Name = "Greek Capital Letter Sigma" },
				//new Symbol { SymbolStr = "Φ", Name = "Greek Capital Letter Phi" },
				//new Symbol { SymbolStr = "Ψ", Name = "Greek Capital Letter Psi" },
				//new Symbol { SymbolStr = "Ω", Name = "Greek Capital Letter Omega" },

				new Symbol { SymbolStr = "α", Name = "Greek Small Letter Alpha" },
				new Symbol { SymbolStr = "β", Name = "Greek Small Letter Beta" },
				new Symbol { SymbolStr = "γ", Name = "Greek Small Letter Gamma" },
				new Symbol { SymbolStr = "δ", Name = "Greek Small Letter Delta" },
				new Symbol { SymbolStr = "ε", Name = "Greek Small Letter Epsilon" },
				new Symbol { SymbolStr = "ζ", Name = "Greek Small Letter Zeta" },
				new Symbol { SymbolStr = "η", Name = "Greek Small Letter Eta" },
				new Symbol { SymbolStr = "θ", Name = "Greek Small Letter Theta" },
				new Symbol { SymbolStr = "ι", Name = "Greek Small Letter Iota" },
				new Symbol { SymbolStr = "κ", Name = "Greek Small Letter Kappa" },
				new Symbol { SymbolStr = "λ", Name = "Greek Small Letter Lamda" },
				new Symbol { SymbolStr = "μ", Name = "Greek Small Letter Mu" },
				new Symbol { SymbolStr = "ν", Name = "Greek Small Letter Nu" },
				new Symbol { SymbolStr = "ξ", Name = "Greek Small Letter Xi" },
				new Symbol { SymbolStr = "ο", Name = "Greek Small Letter Omicron" },
				new Symbol { SymbolStr = "π", Name = "Greek Small Letter Pi" },
				new Symbol { SymbolStr = "ρ", Name = "Greek Small Letter Rho" },
				//new Symbol { SymbolStr = "ς", Name = "Greek Small Letter Final Sigma" },
				new Symbol { SymbolStr = "σ", Name = "Greek Small Letter Sigma" },
				new Symbol { SymbolStr = "τ", Name = "Greek Small Letter Tau" },
				new Symbol { SymbolStr = "υ", Name = "Greek Small Letter Upsilon" },
				new Symbol { SymbolStr = "φ", Name = "Greek Small Letter Phi" },
				new Symbol { SymbolStr = "χ", Name = "Greek Small Letter Chi" },
				new Symbol { SymbolStr = "ψ", Name = "Greek Small Letter Psi" },
				new Symbol { SymbolStr = "ω", Name = "Greek Small Letter Omega" },

				new Symbol { SymbolStr = "ϑ", Name = "Greek Theta Symbol" },
				new Symbol { SymbolStr = "ϕ", Name = "Greek Phi Symbol" }

				//new Symbol { SymbolStr = "♥", Name = "Black Heart Suit" },
				//new Symbol { SymbolStr = "★", Name = "Black Star" },

				//new Symbol { SymbolStr = "←", Name = "Leftwards Arrow" },
				//new Symbol { SymbolStr = "↑", Name = "Upwards Arrow" },
				//new Symbol { SymbolStr = "→", Name = "Rightwards Arrow" },
				//new Symbol { SymbolStr = "↓", Name = "Downwards Arrow" },
				//new Symbol { SymbolStr = "↔", Name = "Left Right Arrow" },
				//new Symbol { SymbolStr = "↕", Name = "Up Down Arrow" },
				////new Symbol { SymbolStr = "↖", Name = "North West Arrow" },
				////new Symbol { SymbolStr = "↗", Name = "North East Arrow" },
				////new Symbol { SymbolStr = "↘", Name = "South East Arrow" },
				////new Symbol { SymbolStr = "↙", Name = "South West Arrow" },
				//new Symbol { SymbolStr = "⇐", Name = "Leftwards Double Arrow" },
				//new Symbol { SymbolStr = "⇒", Name = "Rightwards Double Arrow" },

				//new Symbol { SymbolStr = "±", Name = "Plus-Minus Sign" },
				//new Symbol { SymbolStr = "°", Name = "Degree Sign" },

				//new Symbol { SymbolStr = "©", Name = "Copyright Sign" },
				//new Symbol { SymbolStr = "®", Name = "Registered Sign" },

				//new Symbol { SymbolStr = "«", Name = "Left-Pointing Double Angle Quotation Mark" },
				//new Symbol { SymbolStr = "»", Name = "Right-Pointing Double Angle Quotation Mark" },

				//new Symbol { SymbolStr = "€", Name = "Euro Sign" },
				//new Symbol { SymbolStr = "£", Name = "Pound Sign" },
				//new Symbol { SymbolStr = "¥", Name = "Yen Sign" },

				//new Symbol { SymbolStr = "∀", Name = "For All" },
				//new Symbol { SymbolStr = "∂", Name = "Partial Differential" },
				//new Symbol { SymbolStr = "∃", Name = "There Exists" },
				//new Symbol { SymbolStr = "∄", Name = "There Does Not Exist" },
				//new Symbol { SymbolStr = "∅", Name = "Empty Set" },
				//new Symbol { SymbolStr = "∆", Name = "Increment" },
				//new Symbol { SymbolStr = "∇", Name = "Nabla" },
				//new Symbol { SymbolStr = "∈", Name = "Element Of" },
				//new Symbol { SymbolStr = "∉", Name = "Not an Element Of" },
				//new Symbol { SymbolStr = "∎", Name = "End of Proof" },
				//new Symbol { SymbolStr = "∘", Name = "Ring Operator" },
				//new Symbol { SymbolStr = "∙", Name = "Bullet Operator" },
				//new Symbol { SymbolStr = "√", Name = "Square Root" },
				//new Symbol { SymbolStr = "∞", Name = "Infinity" },
				//new Symbol { SymbolStr = "∠", Name = "Angle" },
				//new Symbol { SymbolStr = "∡", Name = "Measured Angle" },
				//new Symbol { SymbolStr = "∤", Name = "Does Not Divide" },
				//new Symbol { SymbolStr = "∧", Name = "Logical And" },
				//new Symbol { SymbolStr = "∨", Name = "Logical Or" },
				//new Symbol { SymbolStr = "∩", Name = "Intersection" },
				//new Symbol { SymbolStr = "∪", Name = "Union" },
				//new Symbol { SymbolStr = "∫", Name = "Integral" },
				////new Symbol { SymbolStr = "∬", Name = "Double Integral" },
				////new Symbol { SymbolStr = "∮", Name = "Contour Integral" },
				////new Symbol { SymbolStr = "∯", Name = "Surface Integral" },
				////new Symbol { SymbolStr = "≁", Name = "Not Tilde" },
				//new Symbol { SymbolStr = "≃", Name = "Asymptotically Equal To" },
				//new Symbol { SymbolStr = "≄", Name = "Not Asymptotically Equal To" },
				////new Symbol { SymbolStr = "≅", Name = "Approximately Equal To" },
				////new Symbol { SymbolStr = "≆", Name = "Approximately But Not Actually Equal To" },
				//// new Symbol { SymbolStr = "≇", Name = "Neither Approximately nor Actually Equal To" },
				//new Symbol { SymbolStr = "≈", Name = "Almost Equal To" },
				////new Symbol { SymbolStr = "≉", Name = "Not Almost Equal To" },
				//new Symbol { SymbolStr = "≤", Name = "Less-Than or Equal To" },
				//new Symbol { SymbolStr = "≥", Name = "Greater-Than or Equal To" },

				//new Symbol { SymbolStr = "⌘", Name = "Place of Interest Sign" },
				//new Symbol { SymbolStr = "☹", Name = "White Frowning Face" },
				//new Symbol { SymbolStr = "☺", Name = "White Smiling Face" },

				//new Symbol { SymbolStr = "ℂ", Name = "Double-Struck Capital C" },
				//new Symbol { SymbolStr = "ℕ", Name = "Double-Struck Capital N" },
				//new Symbol { SymbolStr = "ℚ", Name = "Double-Struck Capital Q" },
				//new Symbol { SymbolStr = "ℝ", Name = "Double-Struck Capital R" },
				//new Symbol { SymbolStr = "ℤ", Name = "Double-Struck Capital Z" }
			);
		}
	}
}
