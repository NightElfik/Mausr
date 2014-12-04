using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Drawing.Imaging;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace Mausr.Core.NeuralNet {
	public class NetInputConvertor {

		public void Shuffle<T>(IList<T> stuff) {
			var rand = new Random();
			int count = stuff.Count;

			for (int i = 0; i < count; ++i) {
				int otherI = rand.Next(0, count - 1);
				T tempI = stuff[i];
				stuff[i] = stuff[otherI];
				stuff[otherI] = tempI;
			}
		}

		public Matrix<double> CreateInputsMatrix(IEnumerable<RawDrawing> rawDrawings, Rasterizer rasterizer) {
			var inputVectors = new List<Vector<double>>();

			using (var img = new Bitmap(rasterizer.ImageSize, rasterizer.ImageSize, PixelFormat.Format24bppRgb)) {
				int cols = img.Width * img.Height;
				foreach (var drawing in rawDrawings) {
					rasterizer.Rasterize(img, drawing);
					inputVectors.Add(ImageToVector(img));
				}
			}

			return DenseMatrix.OfRowVectors(inputVectors);
		}

		public int[] CreateOutIndicesFromIds(IEnumerable<int> inputIds, Net network) {
			return inputIds.Select(id => network.MapOutputToOutNeuron(id)).ToArray();
		}

		unsafe public Vector<double> ImageToVector(Bitmap img) {
			Contract.Requires(img.PixelFormat == PixelFormat.Format24bppRgb);

			int wid = img.Width;
			int hei = img.Height;

			var imgData = img.LockBits(new Rectangle(0, 0, img.Width, img.Height),
				ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

			var resultVector = new DenseVector(wid * hei);

			byte* imgPtr = (byte*)imgData.Scan0;
			int imgStride = imgData.Stride;

			for (int y = 0; y < hei; ++y) {
				int baseVecI = y * wid;
				int baseStride = y * imgStride;

				for (int x = 0; x < wid; ++x) {
					resultVector[baseVecI + x] = 1.0 - imgPtr[baseStride + x * 3] / 255.0;
				}
			}

			img.UnlockBits(imgData);

			return resultVector;
		}

	}
}
