using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using MathNet.Numerics.LinearAlgebra;
using Mausr.Core.NeuralNet;

namespace Mausr.Core {
	public class Rasterizer {

		public int ImageSize { get; set; }
		public float PenSizePerc { get; set; }
		public bool ExtraMargin { get; set; }
		public bool DrawPoints { get; set; }


		public Bitmap Rasterize(RawDrawing drawing) {
			return Rasterize(drawing, ImageSize, PenSizePerc, ExtraMargin, DrawPoints);
		}

		public void Rasterize(Bitmap img, RawDrawing drawing) {
			Rasterize(img, new Rectangle(0, 0, img.Width, img.Height), drawing, PenSizePerc, ExtraMargin, DrawPoints);
		}

		[Pure]
		public Bitmap Rasterize(RawDrawing drawing, int imageSize, float penSizePerc, bool extraMargin, bool drawPoints) {
			Bitmap img = new Bitmap(imageSize, imageSize, PixelFormat.Format24bppRgb);
			Rasterize(img, new Rectangle(0, 0, img.Width, img.Height), drawing, penSizePerc, extraMargin, drawPoints);
			return img;
		}
		
		[Pure]
		public void Rasterize(Bitmap img, Rectangle dest, RawDrawing drawing, float penSizePerc, bool extraMargin, bool drawPoints) {
			using (var g = Graphics.FromImage(img)) {
				g.Clear(Color.White);
				g.SetClip(new Rectangle(0, 0, img.Width, img.Height));
				rasterize(g, drawing, penSizePerc, extraMargin, drawPoints);
			}
		}

		public Bitmap RasterizeToGrid(IList<Tuple<RawDrawing, Brush, string>> drawings, Color bg, int cols) {
			Contract.Requires(drawings.Count > 0);

			int count = drawings.Count;
			int rows = (count + cols - 1) / cols;

			Size size = new Size(1 + cols * (ImageSize + 1), 1 + rows * (ImageSize + 1));
			Bitmap img = new Bitmap(size.Width, size.Height, PixelFormat.Format24bppRgb);

			var font = new Font(FontFamily.GenericSansSerif, 5);

			using (var g = Graphics.FromImage(img)) {
				g.Clear(bg);

				int row = 0;
				int col = 0;
				foreach (var drawing in drawings) {
					Contract.Assert(row < rows && col < cols);

					var bb = new Rectangle(1 + col * (ImageSize + 1), 1 + row * (ImageSize + 1), ImageSize, ImageSize);
					g.SetClip(bb);
					g.FillRectangle(drawing.Item2, bb);
					rasterize(g, drawing.Item1, PenSizePerc, ExtraMargin, DrawPoints);

					if (!string.IsNullOrWhiteSpace(drawing.Item3)) {
						g.DrawString(drawing.Item3, font, Brushes.White, bb.X + 1, bb.Y + 1);
						g.DrawString(drawing.Item3, font, Brushes.Black, bb.Location);
					}

					col += 1;
					if (col >= cols) {
						col = 0;
						row += 1;
					}
				}
			}

			return img;
		}

		[Pure]
		public Bitmap DrawDataToGrid(IList<Tuple<Vector<double>, Color, string>> data, int imageSize, Color bg, int cols) {
			Contract.Requires(data.Count > 0);

			int count = data.Count;
			int rows = (count + cols - 1) / cols;

			Size size = new Size(1 + cols * (imageSize + 1), 1 + rows * (imageSize + 1));
			Bitmap img = new Bitmap(size.Width, size.Height, PixelFormat.Format24bppRgb);

			var font = new Font(FontFamily.GenericSansSerif, 5);

			var drawingImg = new Bitmap(imageSize, imageSize, PixelFormat.Format24bppRgb);
			var ic = new NetInputConvertor();

			using (Graphics g = Graphics.FromImage(img), imgG = Graphics.FromImage(drawingImg)) {
				g.Clear(bg);

				int row = 0;
				int col = 0;
				foreach (var item in data) {
					Contract.Assert(row < rows && col < cols);

					var bb = new Rectangle(1 + col * (imageSize + 1), 1 + row * (imageSize + 1), imageSize, imageSize);
					imgG.Clear(item.Item2);
					ic.VectorToImage(item.Item1, drawingImg);
					g.DrawImageUnscaled(drawingImg, bb);

					if (!string.IsNullOrWhiteSpace(item.Item3)) {
						g.DrawString(item.Item3, font, Brushes.White, bb.X + 1, bb.Y + 1);
						g.DrawString(item.Item3, font, Brushes.Black, bb.Location);
					}

					col += 1;
					if (col >= cols) {
						col = 0;
						row += 1;
					}
				}
			}

			return img;
		}


		/// <summary>
		/// Draws given drawing using given graphics object. Clip bounds are used as area for drawing.
		/// </summary>
		[Pure]
		private void rasterize(Graphics g, RawDrawing drawing, float penSizePerc, bool extraMargin, bool drawPoints) {
			float penSize = ((g.ClipBounds.Width + g.ClipBounds.Height) / 2) * penSizePerc;
			// Pen has to be local otherwise it cannot be user in multiple threads.
			var pen = new Pen(Color.Black, penSize) {
				StartCap = LineCap.Round,
				EndCap = LineCap.Round,
				LineJoin = LineJoin.Round,
			};


			// Given points are from 0 to 1 inclusive, max is thus (size - 1).
			SizeF drawingMult = new SizeF(g.ClipBounds.Width - 1, g.ClipBounds.Height - 1);
			PointF offset = g.ClipBounds.Location;

			if (extraMargin) {
				// Make sure to fill all available space (pixel precise).
				// All strokes should be exactly to the edge of image.
				drawingMult.Width += 2;
				drawingMult.Height += 2;

				float off = (float)Math.Ceiling(penSize / 2);
				drawingMult.Width -= 2 * off;
				drawingMult.Height -= 2 * off;
				off -= 1;

				offset.X += off;
				offset.Y += off;
			}

			var pointsColors = new Pen[] { Pens.Red, Pens.Lime, Pens.Yellow };

			g.SmoothingMode = SmoothingMode.HighQuality;

			foreach (var line in drawing.Lines) {
				if (line.Length == 0) {
					continue;
				}

				var pts = new PointF[line.Length];

				for (int i = 0; i < line.Length; i++) {
					pts[i].X = offset.X + line[i].X * drawingMult.Width;
					pts[i].Y = offset.Y + line[i].Y * drawingMult.Height;
				}
				g.DrawLines(pen, pts);
			}

			if (drawPoints) {
				int lineIndex = 0;
				foreach (var line in drawing.Lines) {
					if (line.Length == 0) {
						continue;
					}

					var ptPen = pointsColors[lineIndex % pointsColors.Length];
					for (int i = 0; i < line.Length; i++) {
						float x = (float)Math.Round(offset.X + line[i].X * drawingMult.Width);
						float y = (float)Math.Round(offset.Y + line[i].Y * drawingMult.Height);
						g.DrawLine(ptPen, x, y, x + 0.2f, y);
						g.DrawLine(ptPen, x, y, x - 0.2f, y);
					}

					lineIndex += 1;
				}
			}
		}

	}
}
