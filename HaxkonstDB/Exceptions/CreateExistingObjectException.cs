using System;
using System.Collections.Generic;
using System.Text;

namespace HaxkonstDB.Exceptions
{
    public class CreateExistingObjectException : Exception {

        public CreateExistingObjectException() : base() {
        }

        public CreateExistingObjectException(String message) : base(message) {
        }


    }
}
