using System;
using System.Collections.Generic;
using System.Text;

namespace HaxkonstDB.Exceptions
{
    public class UpdateUnexistingObjectException : Exception {

        public UpdateUnexistingObjectException() : base() {
        }

        public UpdateUnexistingObjectException(String message) : base(message) {
        }


    }
}
