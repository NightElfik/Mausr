using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Mausr.Web.Models {
	public interface IPagination<T> : IReadOnlyList<T> {

		int Advance { get; }

		int TotalCount { get; }
		int PageNumber { get; }
		int TotalPages { get; }
		int ItemsParPage { get; }

		bool IsFirstPage { get; }
		bool IsLastPage { get; }


		HtmlString Html();

	}

	public static class IPaginationExts {

		public static Pagination<T> ToPagination<T>(this IQueryable<T> seq, int? pageNumber, int itemsPerPage,
				Func<int, string> urlFunc, int advance = 2, Func<int, string> labelSelector = null) {
			return new Pagination<T>(seq, pageNumber ?? 1, itemsPerPage, urlFunc, advance, labelSelector);
		}

	}


	public class Pagination<T> : IPagination<T> {

		public int Advance { get; private set; }

		private List<T> items;

		public T this[int index] { get { return items[index]; } }
		public int Count { get { return items.Count; } }
		public int TotalCount { get; private set; }

		public int PageNumber { get; private set; }
		public int TotalPages { get; private set; }
		public int ItemsParPage { get; private set; }

		public bool IsFirstPage { get { return PageNumber == 1; } }
		public bool IsLastPage { get { return PageNumber == TotalPages; } }

		private Func<int, string> urlFunc;
		private Func<int, string> labelSelector;


		public Pagination(IQueryable<T> sequence, int pageNumber, int itemsPerPage, Func<int, string> urlFunc,
				int advance, Func<int, string> labelSelector) {
			PageNumber = Math.Max(1, pageNumber);
			ItemsParPage = itemsPerPage;
			TotalCount = sequence.Count();
			TotalPages = (TotalCount + itemsPerPage - 1) / itemsPerPage;
			items = sequence.Skip((pageNumber - 1) * itemsPerPage).Take(itemsPerPage).ToList();

			this.urlFunc = urlFunc;
			this.labelSelector = labelSelector;

			Advance = advance;
		}


		public HtmlString Html() {
			var sb = new StringBuilder();
			sb.Append("<nav><ul class='pagination'>");
			sb.AppendFormat("<li{0}><a href='{1}'><span aria-hidden='true'>&laquo;</span><span class='sr-only'>Previous</span></a></li>",
				IsFirstPage ? " class='disabled'" : "",
				IsFirstPage ? urlFunc(PageNumber) : urlFunc(PageNumber - 1));


			if (PageNumber - Advance > 1) {
				sb.Append(pageLi(1));
			}

			renderElipsis(sb, 2, PageNumber - Advance - 1);

			for (int i = Math.Max(1, PageNumber - Advance); i <= PageNumber + Advance && i <= TotalPages; ++i) {
				sb.Append(pageLi(i));
			}
			
			renderElipsis(sb, PageNumber + Advance + 1, TotalPages - 1);

			if (PageNumber + Advance < TotalPages) {
				sb.Append(pageLi(TotalPages));
			}

			sb.AppendFormat("<li{0}><a href='{1}'><span aria-hidden='true'>&raquo;</span><span class='sr-only'>Next</span></a></li>",
				IsLastPage ? " class='disabled'" : "",
				IsLastPage ? urlFunc(PageNumber) : urlFunc(PageNumber + 1));
			sb.Append("</ul></nav>");

			return new HtmlString(sb.ToString());
		}

		private void renderElipsis(StringBuilder sb, int leftPage, int rightPage) {
			if (rightPage < leftPage) {
				return;
			}

			if (leftPage == rightPage) {
				sb.Append(pageLi(leftPage));
				return;
			}

			int midPage = (leftPage + rightPage) / 2;
			sb.Append(string.Format("<li><a href='{3}' title='Go to page {2}'>{0} ... {1}</a></li>",
				getLablel(leftPage),
				getLablel(rightPage),
				midPage,
				urlFunc(midPage)));
		}

		private string getLablel(int page) {
			return labelSelector == null ? page.ToString() : labelSelector(page);
		}

		private string pageLi(int i) {
			return string.Format("<li {2}><a href='{3}' title='Page {0}'>{1}</a></li>",
				i,
				getLablel(i),
				i == PageNumber ? " class='active'" : "",
				urlFunc(i));
		}


		public IEnumerator<T> GetEnumerator() {
			return items.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return items.GetEnumerator();
		}

	}
}