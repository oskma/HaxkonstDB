using System;
using System.Collections.Generic;
using System.Text;

namespace HaxkonstDB.Exceptions
{
	public class NonReferenceTypeException : Exception {

		public NonReferenceTypeException() : base() {
		}

		public NonReferenceTypeException(String message) : base(message) {
		}


	}
}
