using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KarmaTestAdapterTests.Expectations
{
    public class BetweenConstraint : ComparisonConstraint
    {
        private object expected1;
        private object expected2;

        public BetweenConstraint(object expectedLower, object expectedUpper)
            : base(expectedLower, expectedUpper)
        {
            this.expected1 = expectedLower;
            this.expected2 = expectedUpper;
        }

        private object Lower
        {
            get
            {
                if (expected1 == null || expected2 == null)
                {
                    return null;
                }
                return comparer.Compare(expected1, expected2) < 0 ? expected1 : expected2;
            }
        }

        private object Upper
        {
            get
            {
                if (expected1 == null || expected2 == null)
                {
                    return null;
                }
                return comparer.Compare(expected1, expected2) < 0 ? expected2 : expected1;
            }
        }

        private bool Same
        {
            get
            {
                return expected1 != null && expected2 != null && comparer.Compare(expected1, expected2) == 0;
            }
        }

        public override bool Matches(object actual)
        {
            this.actual = actual;
            if (Lower == null || Upper == null || actual == null)
            {
                throw new ArgumentException("Cannot compare using a null reference");
            }
            return comparer.Compare(actual, Lower) >= 0 && comparer.Compare(actual, Upper) <= 0;
        }

        public override void WriteDescriptionTo(MessageWriter writer)
        {
            if (Same)
            {
                writer.WriteExpectedValue(expected1);
            }
            else
            {
                writer.WritePredicate("To be between");
                writer.WriteExpectedValue(expected1);
                writer.Write(" and ");
                writer.WriteExpectedValue(expected2);
            }
        }
    }
}
