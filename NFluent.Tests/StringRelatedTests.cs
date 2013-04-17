﻿namespace NFluent.Tests
{
    using NUnit.Framework;

    [TestFixture]
    public class StringRelatedTests
    {
        #region Public Methods and Operators

        [Test]
        [ExpectedException(typeof(FluentAssertionException), ExpectedMessage = "\nThe actual string:\n\t[\"abcdefghijklmnopqrstuvwxyz\"]\ndoes not contain the expected value(s):\n\t[\"C\", \"A\"].")]
        public void ContainsIsCaseSensitive()
        {
            string alphabet = "abcdefghijklmnopqrstuvwxyz";
            Check.That(alphabet).Contains("C", "a", "A", "z");
        }

        [Test]
        [ExpectedException(typeof(FluentAssertionException), ExpectedMessage = "\nThe actual string:\n\t[\"abcdefghijklmnopqrstuvwxyz\"]\ndoes not contain the expected value(s):\n\t[\"0\", \"4\"].")]
        public void ContainsThrowsExceptionWhenFails()
        {
            string alphabet = "abcdefghijklmnopqrstuvwxyz";
            Check.That(alphabet).Contains("c", "0", "4");
        }

        [Test]
        public void ContainsWork()
        {
            string alphabet = "abcdefghijklmnopqrstuvwxyz";
            Check.That(alphabet).Contains("c", "z", "u");
        }

        [Test]
        [ExpectedException(typeof(FluentAssertionException), ExpectedMessage = "\nThe actual string:\n\t[\"abcdefghijklmnopqrstuvwxyz\"]\ndoes not start with:\n\t[\"ABCDEF\"].")]
        public void StartWithIsCaseSensitive()
        {
            string alphabet = "abcdefghijklmnopqrstuvwxyz";
            Check.That(alphabet).StartsWith("ABCDEF");
        }

        [Test]
        public void StartWithWorks()
        {
            string alphabet = "abcdefghijklmnopqrstuvwxyz";
            Check.That(alphabet).StartsWith("abcdef");
        }

        #endregion

        [Test]
        public void AndOperatorCanChainMultipleAssertionsOnString()
        {
            string alphabet = "abcdefghijklmnopqrstuvwxyz";
            Check.That(alphabet).Contains("i").And.StartsWith("abcd").And.IsInstanceOf<string>();
        }
    }
}