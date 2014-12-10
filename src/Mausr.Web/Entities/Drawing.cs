using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Mausr.Core;

namespace Mausr.Web.Entities {
	public class Drawing {

		[Required]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int DrawingId { get; set; }

		[Required]
		public Guid ClientGuid { get; set; }

		public virtual Symbol TopSymbol { get; set; }

		public double? TopSymbolScore { get; set; }		
			
		[Required]
		[Display(Name = "Drawn")]
		public DateTime DrawnDateTime { get; set; }

		[Required]
		public byte[] RawData { get; set; }
						
		[Required]
		[Display(Name = "Drawn using touch")]
		public bool DrawnUsingTouch { get; set; }

		[Required]
		public bool Learned { get; set; }

		
		
		public RawDrawing GetRawDrawing() {
			return RawDrawing.Deserialize(RawData); 
		}

		public void SetRawDrawing(RawDrawing value) {
			RawData = value.Serialize();
		}

	}
}