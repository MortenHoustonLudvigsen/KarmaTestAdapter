using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsTestAdapter.Helpers
{
    public class Validator
    {
        public Validator()
        {
            IsValid = true;
        }

        public bool IsValid { get; private set; }
        public string InvalidReason { get; private set; }

        public void Validate(bool isValid, string reasonFmt, params object[] args)
        {
            Validate(isValid, string.Format(reasonFmt, args));
        }

        public void Validate(bool isValid, string reason = null)
        {
            if (!isValid && IsValid)
            {
                IsValid = false;
                InvalidReason = reason;
            }
        }
    }
}
