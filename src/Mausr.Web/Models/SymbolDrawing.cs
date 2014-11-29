using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Mausr.Core;

namespace Mausr.Web.Models {
	public class SymbolDrawing {

		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int SymbolDrawingId { get; set; }
			
		[Display(Name = "Created")]
		public DateTime CreatedDateTime { get; set; }

		[Required]
		public byte[] RawData { get; set; }
				
		[Required]
		public virtual Symbol Symbol { get; set; }

		[Required]
		[Display(Name = "Drawing tool")]
		public virtual DrawingTool DrawingTool { get; set; }

		[Required]
		[Display(Name = "Drawing device")]
		public virtual DrawingDevice DrawingDevice { get; set; }
		
		
		[NotMapped]
		public RawDrawing RawDrawing {
			get { return RawDrawing.Deserialize(RawData); }
			set { RawData = value.Serialize(); }
		}

	}


	public enum DrawingTool {
		Unknown,
		Mouse,
		Stylus,
		Finger,
	}

	public enum DrawingDevice {
		Unknown,
		Desktop,
		Laptop,
		Tablet,
		Phone,
	}
}