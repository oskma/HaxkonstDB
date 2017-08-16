using System;
using System.Collections.Generic;
using System.Text;

namespace HaxkonstDB.Exceptions
{
    public class DeleteUnexistingObjectException : Exception {

        public DeleteUnexistingObjectException() : base() {
        }

        public DeleteUnexistingObjectException(String message) : base(message) {
        }


    }
}
