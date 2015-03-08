using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Mausr.Core;

namespace Mausr.Web.Entities {
	/// <summary>
	/// Drawing performed as a search query.
	/// </summary>
	public class Drawing {

		/// <summary>
		/// Primary key.
		/// </summary>
		[Required]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int DrawingId { get; set; }

		/// <summary>
		/// Random GUID that is used to identify updates drawings from new ones.
		/// </summary>
		[Required]
		[Index]
		public Guid ClientGuid { get; set; }

		/// <summary>
		/// Top symbol that was shown to user (if any).
		/// </summary>
		public virtual Symbol TopSymbol { get; set; }

		/// <summary>
		/// Top symbol's score (if any).
		/// </summary>
		public double? TopSymbolScore { get; set; }		
			
		/// <summary>
		/// Timestamp of the frawing.
		/// </summary>
		[Required]
		[Display(Name = "Drawn")]
		public DateTime DrawnDateTime { get; set; }

		/// <summary>
		/// Data of the drawing.
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
		/// Authority marked this symbol as low quality, thus, not suitable for promoting to symbol drawing.
		/// </summary>
		[Required]
		public bool LowQuality { get; set; }

		/// <summary>
		/// Symbol drawing that was created based on this drawing.
		/// </summary>
		public virtual SymbolDrawing PromotedSymbolDrawing { get; set; }

		
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