using System;

namespace DarkSky.Commerce.Exceptions {
    public class DuplicatePaymentException : InvalidOperationException {
        public DuplicatePaymentException(string message) : base(message) {
        }
    }
}