using System;
using System.Data.Entity.Validation;
using System.Text;

namespace Mausr.Web.Entities {
	public class FormattedDbEntityValidationException : Exception {
		public FormattedDbEntityValidationException(DbEntityValidationException innerException) :
			base(null, innerException) {
		}

		public override string Message {
			get {
				var innerException = InnerException as DbEntityValidationException;
				if (innerException != null) {
					StringBuilder sb = new StringBuilder();

					sb.AppendLine();
					sb.AppendLine();
					foreach (var eve in innerException.EntityValidationErrors) {
						sb.AppendLine(string.Format("- Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
							eve.Entry.Entity.GetType().FullName, eve.Entry.State));
						foreach (var ve in eve.ValidationErrors) {
							object value = null;
							try {
								value = eve.Entry.CurrentValues.GetValue<object>(ve.PropertyName);
							}
							catch { }
							sb.AppendLine(string.Format("-- Property: \"{0}\", Value: \"{1}\", Error: \"{2}\"",
								ve.PropertyName,
								value,
								ve.ErrorMessage));
						}
					}
					sb.AppendLine();

					return sb.ToString();
				}

				return base.Message;
			}
		}
	}
}