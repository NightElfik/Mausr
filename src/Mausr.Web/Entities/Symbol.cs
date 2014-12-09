using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mausr.Web.Entities {
	public class Symbol {

		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int SymbolId { get; set; }

		[Required]
		[MaxLength(2)]
		[Display(Name = "Symbol")]
		public string SymbolStr { get; set; }

		[Required]
		public string Name { get; set; }


		public virtual ICollection<SymbolDrawing> SymbolDrawings { get; set; }

		public virtual ICollection<Drawing> Drawings { get; set; }

	}
}