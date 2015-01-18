using System.Data.Entity.Migrations;
using System.Linq;
using Mausr.Web.Entities;
using Mausr.Web.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Mausr.Web.Migrations {

	internal sealed class Configuration : DbMigrationsConfiguration<MausrDb> {
		public Configuration() {
			AutomaticMigrationsEnabled = true;
			AutomaticMigrationDataLossAllowed = false;
		}

		protected override void Seed(MausrDb db) {

			// Turn on case-sensitive (binary) comparison.
			// Default locale is case insensitive and treats symbols Σ, ς, and σ as identical.
			db.Database.ExecuteSqlCommand(
				@"ALTER TABLE Symbols ALTER COLUMN SymbolStr NVARCHAR(2) COLLATE SQL_Latin1_General_CP850_BIN NOT NULL");

#if DEBUG && false

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
			
			db.Symbols.AddOrUpdate(
				s => s.SymbolStr,
				//new Symbol { SymbolStr = "", Name = "", HtmlEntity = "Alpha" },
				//new Symbol { SymbolStr = "", Name = "", HtmlEntity = "Beta" },
				new Symbol { SymbolStr = "Γ", Name = "Greek Capital Letter Gamma", HtmlEntity = "Gamma" },
				new Symbol { SymbolStr = "Δ", Name = "Greek Capital Letter Delta", HtmlEntity = "Delta" },
				//new Symbol { SymbolStr = "", Name = "", HtmlEntity = "Epsilon" },
				//new Symbol { SymbolStr = "", Name = "", HtmlEntity = "Zeta" },
				//new Symbol { SymbolStr = "", Name = "", HtmlEntity = "Eta" },
				new Symbol { SymbolStr = "Θ", Name = "Greek Capital Letter Theta", HtmlEntity = "Theta" },
				//new Symbol { SymbolStr = "", Name = "", HtmlEntity = "Iota" },
				//new Symbol { SymbolStr = "", Name = "", HtmlEntity = "Kappa" },
				new Symbol { SymbolStr = "Λ", Name = "Greek Capital Letter Lamda", HtmlEntity = "Lambda" },
				//new Symbol { SymbolStr = "", Name = "", HtmlEntity = "Mu" },
				//new Symbol { SymbolStr = "", Name = "", HtmlEntity = "Nu" },
				new Symbol { SymbolStr = "Ξ", Name = "Greek Capital Letter Xi", HtmlEntity = "Xi" },
				//new Symbol { SymbolStr = "", Name = "", HtmlEntity = "Omicron" },
				new Symbol { SymbolStr = "Π", Name = "Greek Capital Letter Pi", HtmlEntity = "Pi" },
				//new Symbol { SymbolStr = "", Name = "", HtmlEntity = "Rho" },
				new Symbol { SymbolStr = "Σ", Name = "Greek Capital Letter Sigma", HtmlEntity = "Sigma" },
				//new Symbol { SymbolStr = "", Name = "", HtmlEntity = "Tau" },
				//new Symbol { SymbolStr = "", Name = "", HtmlEntity = "Upsilon" },
				new Symbol { SymbolStr = "Φ", Name = "Greek Capital Letter Phi", HtmlEntity = "Phi" },
				//new Symbol { SymbolStr = "", Name = "", HtmlEntity = "Chi" },
				new Symbol { SymbolStr = "Ψ", Name = "Greek Capital Letter Psi", HtmlEntity = "Psi" },
				new Symbol { SymbolStr = "Ω", Name = "Greek Capital Letter Omega", HtmlEntity = "Omega" },

				new Symbol { SymbolStr = "α", Name = "Greek Small Letter Alpha", HtmlEntity = "alpha" },
				new Symbol { SymbolStr = "β", Name = "Greek Small Letter Beta", HtmlEntity = "beta" },
				new Symbol { SymbolStr = "γ", Name = "Greek Small Letter Gamma", HtmlEntity = "gamma" },
				new Symbol { SymbolStr = "δ", Name = "Greek Small Letter Delta", HtmlEntity = "delta" },
				new Symbol { SymbolStr = "ε", Name = "Greek Small Letter Epsilon", HtmlEntity = "epsilon" },
				new Symbol { SymbolStr = "ζ", Name = "Greek Small Letter Zeta", HtmlEntity = "zeta" },
				new Symbol { SymbolStr = "η", Name = "Greek Small Letter Eta", HtmlEntity = "eta" },
				new Symbol { SymbolStr = "θ", Name = "Greek Small Letter Theta", HtmlEntity = "theta" },
				new Symbol { SymbolStr = "ι", Name = "Greek Small Letter Iota", HtmlEntity = "iota" },
				new Symbol { SymbolStr = "κ", Name = "Greek Small Letter Kappa", HtmlEntity = "kappa" },
				new Symbol { SymbolStr = "λ", Name = "Greek Small Letter Lamda", HtmlEntity = "lambda" },
				new Symbol { SymbolStr = "μ", Name = "Greek Small Letter Mu", HtmlEntity = "mu" },
				new Symbol { SymbolStr = "ν", Name = "Greek Small Letter Nu", HtmlEntity = "nu" },
				new Symbol { SymbolStr = "ξ", Name = "Greek Small Letter Xi", HtmlEntity = "xi" },
				new Symbol { SymbolStr = "ο", Name = "Greek Small Letter Omicron", HtmlEntity = "omicron" },
				new Symbol { SymbolStr = "π", Name = "Greek Small Letter Pi", HtmlEntity = "pi" },
				new Symbol { SymbolStr = "ρ", Name = "Greek Small Letter Rho", HtmlEntity = "rho" },
				new Symbol { SymbolStr = "ς", Name = "Greek Small Letter Final Sigma", HtmlEntity = "sigmaf" },
				new Symbol { SymbolStr = "σ", Name = "Greek Small Letter Sigma", HtmlEntity = "sigma" },
				new Symbol { SymbolStr = "τ", Name = "Greek Small Letter Tau", HtmlEntity = "tau" },
				new Symbol { SymbolStr = "υ", Name = "Greek Small Letter Upsilon", HtmlEntity = "upsilon" },
				new Symbol { SymbolStr = "φ", Name = "Greek Small Letter Phi", HtmlEntity = "phi" },
				new Symbol { SymbolStr = "χ", Name = "Greek Small Letter Chi", HtmlEntity = "chi" },
				new Symbol { SymbolStr = "ψ", Name = "Greek Small Letter Psi", HtmlEntity = "psi" },
				new Symbol { SymbolStr = "ω", Name = "Greek Small Letter Omega", HtmlEntity = "omega" },

				new Symbol { SymbolStr = "ϑ", Name = "Greek Theta Symbol", HtmlEntity = "thetasym" },
				new Symbol { SymbolStr = "ϕ", Name = "Greek Phi Symbol", HtmlEntity = "phiv" },


				new Symbol { SymbolStr = "–", Name = "En Dash", HtmlEntity = "ndash" },
				new Symbol { SymbolStr = "—", Name = "Em Dash", HtmlEntity = "mdash" },
				new Symbol { SymbolStr = "†", Name = "Dagger", HtmlEntity = "dagger" },
				new Symbol { SymbolStr = "‡", Name = "Double Dagger", HtmlEntity = "Dagger" },
				new Symbol { SymbolStr = "‰", Name = "Per Mille Sign", HtmlEntity = "permil" },
				new Symbol { SymbolStr = "‱", Name = "Per Ten Thousand Sign", HtmlEntity = "pertenk" },

				new Symbol { SymbolStr = "♥", Name = "Black Heart Suit", HtmlEntity = "hearts" },
				new Symbol { SymbolStr = "★", Name = "Black Star", HtmlEntity = "bigstar" },
				new Symbol { SymbolStr = "✰", Name = "Shadowed White Star" },
				new Symbol { SymbolStr = "✓", Name = "Check Mark" },
				new Symbol { SymbolStr = "✗", Name = "Ballot X" },

				new Symbol { SymbolStr = "←", Name = "Leftwards Arrow", HtmlEntity = "larr" },
				new Symbol { SymbolStr = "↑", Name = "Upwards Arrow", HtmlEntity = "uarr" },
				new Symbol { SymbolStr = "→", Name = "Rightwards Arrow", HtmlEntity = "rarr" },
				new Symbol { SymbolStr = "↓", Name = "Downwards Arrow", HtmlEntity = "darr" },
				new Symbol { SymbolStr = "↔", Name = "Left Right Arrow", HtmlEntity = "harr" },
				new Symbol { SymbolStr = "↕", Name = "Up Down Arrow", HtmlEntity = "updownarrow" },
				new Symbol { SymbolStr = "⇐", Name = "Leftwards Double Arrow", HtmlEntity = "lArr" },
				new Symbol { SymbolStr = "⇒", Name = "Rightwards Double Arrow", HtmlEntity = "rArr" },

				new Symbol { SymbolStr = "±", Name = "Plus-Minus Sign", HtmlEntity = "plusmn" },
				new Symbol { SymbolStr = "°", Name = "Degree Sign", HtmlEntity = "deg" },
				new Symbol { SymbolStr = "×", Name = "Multiplication Sign", HtmlEntity = "times" },
				new Symbol { SymbolStr = "÷", Name = "Division Sign", HtmlEntity = "divide" },

				new Symbol { SymbolStr = "©", Name = "Copyright Sign", HtmlEntity="copy" },
				new Symbol { SymbolStr = "®", Name = "Registered Sign", HtmlEntity ="reg" },
				new Symbol { SymbolStr = "™", Name = "Trade Mark Sign", HtmlEntity ="trade" },
				
				new Symbol { SymbolStr = "‹", Name = "Single Left-Pointing Angle Quotation Mark", HtmlEntity = "lsaquo" },
				new Symbol { SymbolStr = "›", Name = "Single Right-Pointing Angle Quotation Mark", HtmlEntity = "rsaquo" },
				new Symbol { SymbolStr = "«", Name = "Left-Pointing Double Angle Quotation Mark", HtmlEntity = "laquo" },
				new Symbol { SymbolStr = "»", Name = "Right-Pointing Double Angle Quotation Mark", HtmlEntity = "raquo" },

				new Symbol { SymbolStr = "€", Name = "Euro Sign", HtmlEntity = "euro" },
				new Symbol { SymbolStr = "£", Name = "Pound Sign", HtmlEntity = "pound" },
				new Symbol { SymbolStr = "¥", Name = "Yen Sign", HtmlEntity = "yen" },

				new Symbol { SymbolStr = "∀", Name = "For All", HtmlEntity = "forall" },
				new Symbol { SymbolStr = "∂", Name = "Partial Differential", HtmlEntity = "part" },
				new Symbol { SymbolStr = "∃", Name = "There Exists", HtmlEntity = "exist" },
				new Symbol { SymbolStr = "∄", Name = "There Does Not Exist", HtmlEntity = "" },
				new Symbol { SymbolStr = "∅", Name = "Empty Set", HtmlEntity = "empty" },
				new Symbol { SymbolStr = "∆", Name = "Increment" },
				new Symbol { SymbolStr = "∇", Name = "Nabla", HtmlEntity = "nabla" },
				new Symbol { SymbolStr = "∈", Name = "Element Of", HtmlEntity = "isin" },
				new Symbol { SymbolStr = "∉", Name = "Not an Element Of", HtmlEntity = "notin" },
				new Symbol { SymbolStr = "∎", Name = "End of Proof" },
				new Symbol { SymbolStr = "∘", Name = "Ring Operator", HtmlEntity = "compfn" },
				new Symbol { SymbolStr = "∙", Name = "Bullet Operator" },
				new Symbol { SymbolStr = "√", Name = "Square Root", HtmlEntity = "radic" },
				new Symbol { SymbolStr = "∞", Name = "Infinity", HtmlEntity = "infin" },
				new Symbol { SymbolStr = "∠", Name = "Angle", HtmlEntity = "ang" },
				new Symbol { SymbolStr = "∡", Name = "Measured Angle", HtmlEntity = "angmsd" },
				new Symbol { SymbolStr = "∤", Name = "Does Not Divide", HtmlEntity = "nmid" },
				new Symbol { SymbolStr = "∧", Name = "Logical And", HtmlEntity = "and" },
				new Symbol { SymbolStr = "∨", Name = "Logical Or", HtmlEntity = "or" },
				new Symbol { SymbolStr = "∩", Name = "Intersection", HtmlEntity = "cap" },
				new Symbol { SymbolStr = "∪", Name = "Union", HtmlEntity = "cup" },
				new Symbol { SymbolStr = "∫", Name = "Integral", HtmlEntity = "int" },
				//new Symbol { SymbolStr = "∬", Name = "Double Integral", HtmlEntity = "" },
				//new Symbol { SymbolStr = "∮", Name = "Contour Integral", HtmlEntity = "" },
				//new Symbol { SymbolStr = "∯", Name = "Surface Integral", HtmlEntity = "" },
				//new Symbol { SymbolStr = "≁", Name = "Not Tilde", HtmlEntity = "" },
				new Symbol { SymbolStr = "≃", Name = "Asymptotically Equal To", HtmlEntity = "cong" },
				new Symbol { SymbolStr = "≄", Name = "Not Asymptotically Equal To", HtmlEntity = "NotTildeEqual" },
				//new Symbol { SymbolStr = "≅", Name = "Approximately Equal To", HtmlEntity = "" },
				//new Symbol { SymbolStr = "≆", Name = "Approximately But Not Actually Equal To", HtmlEntity = "" },
				// new Symbol { SymbolStr = "≇", Name = "Neither Approximately nor Actually Equal To", HtmlEntity = "" },
				new Symbol { SymbolStr = "≈", Name = "Almost Equal To", HtmlEntity = "asymp" },
				//new Symbol { SymbolStr = "≉", Name = "Not Almost Equal To", HtmlEntity = "" },
				new Symbol { SymbolStr = "≠", Name = "Not Equal To", HtmlEntity = "ne" },
				new Symbol { SymbolStr = "≡", Name = "Identical To", HtmlEntity = "equiv" },
				new Symbol { SymbolStr = "≤", Name = "Less-Than or Equal To", HtmlEntity = "le" },
				new Symbol { SymbolStr = "≥", Name = "Greater-Than or Equal To", HtmlEntity = "ge" },

				new Symbol { SymbolStr = "⌘", Name = "Place of Interest Sign" },
				new Symbol { SymbolStr = "☹", Name = "White Frowning Face" },
				new Symbol { SymbolStr = "☺", Name = "White Smiling Face" },

				new Symbol { SymbolStr = "ℂ", Name = "Double-Struck Capital C", HtmlEntity = "complexes" },
				new Symbol { SymbolStr = "ℕ", Name = "Double-Struck Capital N", HtmlEntity = "naturals" },
				new Symbol { SymbolStr = "ℚ", Name = "Double-Struck Capital Q", HtmlEntity = "Qopf" },
				new Symbol { SymbolStr = "ℝ", Name = "Double-Struck Capital R", HtmlEntity = "reals" },
				new Symbol { SymbolStr = "ℤ", Name = "Double-Struck Capital Z", HtmlEntity = "integers" },

				new Symbol { SymbolStr = "❄", Name = "Snowflake" },
				new Symbol { SymbolStr = "❅", Name = "Tight Trifoliate Snowflake" },
				new Symbol { SymbolStr = "❆", Name = "Heavy Chevron Snowflake" }
			);
#endif
		}
	}
}
