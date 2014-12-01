using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Mausr.Core;

namespace Mausr.Web.Models {
	public class SymbolDrawing {
		
		[Required]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int SymbolDrawingId { get; set; }
			
		[Required]
		[Display(Name = "Created")]
		public DateTime CreatedDateTime { get; set; }

		[Required]
		public byte[] RawData { get; set; }
				
		[Required]
		public virtual Symbol Symbol { get; set; }
		
		[Required]
		[Display(Name = "Drawn using touch")]
		public virtual bool DrawnUsingTouch { get; set; }
		
		
		[NotMapped]
		public RawDrawing RawDrawing {
			get { return RawDrawing.Deserialize(RawData); }
			set { RawData = value.Serialize(); }
		}

	}
}