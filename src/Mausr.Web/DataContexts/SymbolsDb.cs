using System.Data.Entity;
using System.Linq;
using Mausr.Web.Models;

namespace Mausr.Web.DataContexts {
	
	public class SymbolsDb : DbContext {

		public SymbolsDb() : base("SymbolsConnection") {

		}


		public DbSet<Symbol> Symbols { get; set; }

		public DbSet<SymbolDrawing> SymbolDrawings { get; set; }



	}
}