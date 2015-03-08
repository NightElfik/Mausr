using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Mausr.Core;

namespace Mausr.Web.Entities {
	/// <summary>
	/// Symbol-drawing association.
	/// </summary>
	/// <remarks>
	/// This call is separate rather than just a link between symbols and drawings
	/// because some symbol drawings are not necessiraly originated from search queries
	/// and also for legacy reasons.
	/// </remarks>
	public class SymbolDrawing {
		
		/// <summary>
		/// Primary key.
		/// </summary>
		[Required]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int SymbolDrawingId { get; set; }

		/// <summary>
		/// Symbol that is represented by this drawing.
		/// </summary>
		[Required]
		public virtual Symbol Symbol { get; set; }
			
		/// <summary>
		/// Timestamp of creation.
		/// </summary>
		[Required]
		[Display(Name = "Created")]
		public DateTime CreatedDateTime { get; set; }

		/// <summary>
		/// Raw data of the drawing.
		/// </summary>
		[Required]
		public byte[] RawData { get; set; }
		
		/// <summary>
		/// True if client reported that this was drawn using touch-display (rather than mouse).
		/// This may not be accurate, depends on the client-side JS.
		/// </summary>
		[Required]
		[Display(Name = "Drawn using touch")]
		public bool DrawnUsingTouch { get; set; }
				
		/// <summary>
		/// Approved or rejected by authority.
		/// Unregistered or not approced users still can create symbol drawings but they won't be scored by default.
		/// </summary>
		public bool? Approved { get; set; }

		/// <summary>
		/// Creator of this drawing (if any).
		/// </summary>
		public virtual ApplicationUser Creator { get; set; }
					
		
		/// <summary>
		/// Deserializes the drawing from raw data.
		/// </summary>
		public RawDrawing GetRawDrawing() {
			return RawDrawing.Deserialize(RawData); 
		}
		
		/// <summary>
		/// Serializes given drawing to the raw data.
		/// </summary>
		public void SetRawDrawing(RawDrawing value) {
			RawData = value.Serialize();
		}

	}
}