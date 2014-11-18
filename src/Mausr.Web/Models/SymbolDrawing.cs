using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Mausr.Core;

namespace Mausr.Web.Models {
	public class SymbolDrawing {

		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int SymbolDrawingId { get; set; }
		
		public DateTime CreatedDateTime { get; set; }

		[Required]
		public byte[] RawData { get; set; }
				
		[Required]
		public virtual Symbol Symbol { get; set; }

		
		[NotMapped]
		public RawDrawing RawDrawing {
			get { return RawDrawing.Deserialize(RawData); }
			set { RawData = value.Serialize(); }
		}

	}
}