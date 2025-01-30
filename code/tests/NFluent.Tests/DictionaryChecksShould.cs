﻿ // --------------------------------------------------------------------------------------------------------------------
 // <copyright file="DictionaryChecksShould.cs" company="">
 //   Copyright 2013-2018 Cyrille DUPUYDAUBY
 //   Licensed under the Apache License, Version 2.0 (the "License");
 //   you may not use this file except in compliance with the License.
 //   You may obtain a copy of the License at
 //       http://www.apache.org/licenses/LICENSE-2.0
 //   Unless required by applicable law or agreed to in writing, software
 //   distributed under the License is distributed on an "AS IS" BASIS,
 //   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 //   See the License for the specific language governing permissions and
 //   limitations under the License.
 // </copyright>
 // --------------------------------------------------------------------------------------------------------------------

namespace NFluent.Tests
{
    using System.Collections;
    using System.Collections.Generic;
    using Helpers;
#if !NET35
    using SutClasses;
#endif
    using NFluent.Helpers;
    using NUnit.Framework;

    [TestFixture]
    public class DictionaryChecksShould
    {
        private static readonly Dictionary<string, string> SimpleDico;

        static DictionaryChecksShould()
        {
            SimpleDico = new Dictionary<string, string> {["demo"] = "value", ["other"] = "test"};
        }

        [Test]
        public void ContainsKeyWorks()
        {
            Check.That(SimpleDico).ContainsKey("demo");
        }

        [Test]
        public void InheritedChecks()
        {
            Check.That(SimpleDico).Equals(SimpleDico);

            Check.That(SimpleDico).HasSize(2);

            Check.That(SimpleDico).IsInstanceOf<Dictionary<string, string>>();

            Check.That(SimpleDico).IsNotEqualTo(4);

            Check.That(SimpleDico).IsSameReferenceAs(SimpleDico);
        }

        [Test]
        public void ContainsKeyFailsProperly()
        {
            Check.ThatCode(() =>
            {
                Check.That(SimpleDico).ContainsKey("value");
            })
                .IsAFailingCheckWithMessage("",
                    "The checked dictionary does not contain the expected key.",
                    "The checked dictionary:",
                    "\t{[demo, value],[other, test]} (2 items)",
                    "Expected key:",
                    "\t[\"value\"]");
        }

        [Test]
        public void NotContainsKeyWorksProperly()
        {
            Check.That(SimpleDico).Not.ContainsKey("value");
        }

        [Test]
        public void NotContainsKeyFailsProperly()
        {
            Check.ThatCode(() =>
            {
                Check.That(SimpleDico).Not.ContainsKey("demo");
            })
            .IsAFailingCheckWithMessage("",
                    "The checked dictionary does contain the given key, whereas it must not.",
                    "The checked dictionary:",
                    "\t{[demo, value],[other, test]} (2 items)",
                    "Forbidden key:",
                    "\t[\"demo\"]");
        }

        [Test]
        public void ContainsValueWorks()
        {
            Check.That(SimpleDico).ContainsValue("value");
        }

        [Test]
        public void ContainsValueFailsProperly()
        {
            Check.ThatCode(() =>
            {
                Check.That(SimpleDico).ContainsValue("demo");
            })
                .IsAFailingCheckWithMessage("",
                    "The checked dictionary does not contain the expected value.",
                    "The checked dictionary:",
                    "\t{[demo, value],[other, test]} (2 items)",
                    "Expected value:",
                    "\t[\"demo\"]");
        }

        [Test]
        public void NotContainsValueWorksProperly()
        {
            Check.That(SimpleDico).Not.ContainsValue("demo");
        }

        [Test]
        public void NotContainsValueFailsProperly()
        {
            Check.ThatCode(() =>
            {
                Check.That(SimpleDico).Not.ContainsValue("value");
            })
                .IsAFailingCheckWithMessage("",
                    "The checked dictionary does contain the given value, whereas it must not.",
                    "The checked dictionary:",
                    "\t{[demo, value],[other, test]} (2 items)",
                    "Forbidden value:",
                    "\t[\"value\"]");
        }

        [Test]
        public void ContainsPairWorksProperly()
        {
            Check.That(SimpleDico).ContainsPair("demo", "value");
        }

        [Test]
        public void NotContainsPairFailsOnForbidenPair()
        {
            Check.ThatCode(() =>
                    Check.That(SimpleDico).Not.ContainsPair("demo", "value"))
                .IsAFailingCheckWithMessage("",
                    "The checked dictionary does contain the given key-value pair, whereas it must not.",
                    "The checked dictionary:",
                    "\t{[demo, value],[other, test]} (2 items)",
                    "Forbidden pair:", 
                    "\t[[demo, value]]");
        }

        [Test]
        public void
            WorkWithEnumerationOfKeyValuePair()
        {
            var customDic = new List<KeyValuePair<string, int>> {
                new KeyValuePair<string, int>("otherKey", 15) ,
                new KeyValuePair<string, int>("key", 12)
                };
            Check.That(customDic).ContainsKey("key");
            Check.That(customDic).ContainsValue(12);
            Check.That(customDic).ContainsPair("key", 12);
            Check.ThatCode(() => Check.That(customDic).ContainsKey("missing")).IsAFailingCheckWithMessage("",
                "The checked enumerable does not contain the expected key.",
                "The checked enumerable:",
                "\t{[otherKey, 15],[key, 12]} (2 items)",
                "Expected key:",
                "\t[\"missing\"]");
            // test with empty array
            Check.ThatCode(()=>
            Check.That(new List<KeyValuePair<string, int>>()).ContainsPair("key", 12)).IsAFailingCheckWithMessage("",
                "The checked enumerable does not contain the expected key-value pair. The given key was not found.",
                "The checked enumerable:",
                "\t{} (0 item)",
                "Expected pair:",
                "\t[[key, 12]]");
        }

        [Test]
        public void
            WorkWithEmptyEnumerationOfKeyValuePair()
        {
            var customDic = new List<KeyValuePair<string, int>>();
            Check.ThatCode(() => Check.That(customDic).ContainsKey("missing")).IsAFailingCheckWithMessage("",
                "The checked enumerable does not contain the expected key.",
                "The checked enumerable:",
                "\t{} (0 item)",
                "Expected key:",
                "\t[\"missing\"]");
        }

#if !NET35
        // GH #222
        [Test]
        public void
            WorkWithReadonlyDictionary()
        {
            IReadOnlyDictionary<string, string> roDico = new RoDico(SimpleDico);
            Check.That(roDico).ContainsKey("demo");
            Check.That(roDico).ContainsPair("demo", "value");
            Check.ThatCode(() => Check.That(roDico).ContainsKey("missing")).IsAFailingCheckWithMessage("",
            "The checked enumerable does not contain the expected key.",
                "The checked enumerable:",
                "\t{[demo, value],[other, test]} (2 items)",
           "Expected key:",
                "\t[\"missing\"]");
        }
#endif

        [Test]
        public void CompatibleWithHashtable()
        {
            var basic = new Hashtable {["foo"] = "bar"};

            Check.That(basic).ContainsKey("foo");
            Check.That(basic).ContainsValue("bar");
            Check.That(basic).ContainsPair("foo", "bar");

            Check.ThatCode(() => { Check.That(basic).ContainsKey("bar"); }).IsAFailingCheck();
            Check.ThatCode(() => { Check.That(basic).ContainsValue("foo"); }).IsAFailingCheck();
            Check.ThatCode(() => { Check.That(basic).ContainsPair("bar", "foo"); }).IsAFailingCheck();
            Check.ThatCode(() => { Check.That(basic).ContainsPair("foo", "foo"); }).IsAFailingCheck();
        }

        [Test]
        public void FailsOnHashtable()
        {
            var basic = new Hashtable {["foo"] = "bar"};
            Check.ThatCode(()=>
            Check.That(basic).ContainsKey("bar")).IsAFailingCheckWithMessage("",
                "The checked dictionary does not contain the expected key.",
                "The checked dictionary:",
                "\t{[\"foo\"]= \"bar\"} (1 item)", 
                "Expected key:",
                "\t[\"bar\"]");
            Check.ThatCode(()=>
            Check.That(basic).ContainsValue("foo")).IsAFailingCheckWithMessage("",
                "The checked dictionary does not contain the expected value.",
                "The checked dictionary:", 
                "\t{[\"foo\"]= \"bar\"} (1 item)", 
                "Expected value:", 
                "\t[\"foo\"]");
            Check.ThatCode(()=>
            Check.That(basic).ContainsPair("bar", "foo")).IsAFailingCheckWithMessage("",
                "The checked dictionary does not contain the expected key-value pair. The given key was not found.",
                "The checked dictionary:", 
                "\t{[\"foo\"]= \"bar\"} (1 item)", 
                "Expected pair:", 
                "\t[[bar, foo]]");
            Check.ThatCode(()=>
                Check.That(basic).ContainsPair("foo", "foo")).IsAFailingCheckWithMessage("",
                "The checked dictionary does not contain the expected value for the given key.",
                "The checked dictionary:", 
                "\t{[\"foo\"]= \"bar\"} (1 item)", 
                "Expected pair:", 
                "\t[[foo, foo]]");
        }

        [Test]
        public void ContainsKeyFailsWhenNegated()
        {
            var basic = new Hashtable {["foo"] = "bar"};
            Check.ThatCode(()=>
            Check.That(basic).Not.ContainsKey("foo")).IsAFailingCheckWithMessage("",
                "The checked dictionary does contain the given key, whereas it must not.",
                "The checked dictionary:",
                "\t{[\"foo\"]= \"bar\"} (1 item)", 
                "Forbidden key:",
                "\t[\"foo\"]");
            Check.ThatCode(()=>
            Check.That(basic).Not.ContainsValue("bar")).IsAFailingCheckWithMessage("",
                "The checked dictionary does contain the given value, whereas it must not.",
                "The checked dictionary:", 
                "\t{[\"foo\"]= \"bar\"} (1 item)", 
                "Forbidden value:", 
                "\t[\"bar\"]");
            Check.ThatCode(()=>
                Check.That(basic).Not.ContainsPair("foo", "bar")).IsAFailingCheckWithMessage("",
                "The checked dictionary does contain the given key-value pair, whereas it must not.",
                "The checked dictionary:", 
                "\t{[\"foo\"]= \"bar\"} (1 item)", 
                "Forbidden pair:", 
                "\t[[foo, bar]]");
        }

        [Test]
        public void ContainsPairFailsAppropriately()
        {
            Check.ThatCode(() =>
                    Check.That(SimpleDico).ContainsPair("demo", "1")
                )
                .IsAFailingCheckWithMessage(
                "",
                "The checked dictionary does not contain the expected value for the given key.",
                "The checked dictionary:",
                "\t{[demo, value],[other, test]} (2 items)",
                "Expected pair:",
                "\t[[demo, 1]]");

            Check.ThatCode(() =>
                    Check.That(SimpleDico).ContainsPair("demo2", "1")
                )
                .IsAFailingCheckWithMessage(
                "",
                "The checked dictionary does not contain the expected key-value pair. The given key was not found.",
                "The checked dictionary:",
                "\t{[demo, value],[other, test]} (2 items)",
                "Expected pair:",
                "\t[[demo2, 1]]");
        }

        [Test]
        public void IsEqualToWorks()
        {
            var dict = new Dictionary<string, object> { ["foo"] = new[] { "bar", "baz" } };
            var expected = new Dictionary<string, object> { ["foo"] = new[] { "bar", "baz" } };
            Check.That(dict).IsEqualTo(expected);
        }

        [Test]
        public void IsEqualWorksWhenOnlyOrderChanges()
        {
            var dict = new Dictionary<string, object> { ["bar"] = 1,  ["foo"] = 0 };
            var expected = new Dictionary<string, object> { ["foo"] = 0,  ["bar"] = 1 };
            Check.That(dict).IsEqualTo(expected);
        }

        [Test]
        public void IsEqualToFailsOnDifference()
        {
            // just change the order
            var expected = new Dictionary<string, int> { ["bar"] = 1,  ["foo"] = 0, ["fizz"] = 3};
            Check.ThatCode( () =>
            Check.That(new Dictionary<string, int> { ["foo"] = 1, ["bar"] = 1, ["fizz"] = 3 }).IsEqualTo(expected)).IsAFailingCheckWithMessage("", 
                "The checked dictionary is different from the expected one.", 
                "actual[\"foo\"] = 1 instead of 0.", 
                "The checked dictionary:", 
                "\t{*[foo, 1]*,[bar, 1],[fizz, 3]} (3 items)", 
                "The expected dictionary:", 
                "\t{[bar, 1],*[foo, 0]*,[fizz, 3]} (3 items)");
            Check.ThatCode( () =>
            Check.That(new Dictionary<string, int> { ["bar"] = 1, ["foo"] = 1, ["fizz"] = 3}).IsEqualTo(expected)).IsAFailingCheckWithMessage("", 
                "The checked dictionary is different from the expected one.", 
                "actual[\"foo\"] = 1 instead of 0.",
                "The checked dictionary:", 
                "\t{[bar, 1],*[foo, 1]*,[fizz, 3]} (3 items)", 
                "The expected dictionary:", 
                "\t{[bar, 1],*[foo, 0]*,[fizz, 3]} (3 items)");
            Check.ThatCode( () =>
            Check.That(new Dictionary<string, int> { ["bar"] = 1, ["foo!"] = 0, ["fizz"] = 3}).IsEqualTo(expected)).IsAFailingCheckWithMessage("", 
                "The checked dictionary is different from the expected one.", 
                "[\"foo!\"]= 0 should not exist (found in actual); [\"foo\"]= 0 should be found instead.",
                "The checked dictionary:",
                "\t{[bar, 1],*[foo!, 0]*,[fizz, 3]} (3 items)",
                "The expected dictionary:", 
                "\t{[bar, 1],[foo, 0],[fizz, 3]} (3 items)");
            Check.ThatCode( () =>
            Check.That(new Dictionary<string, int> { ["bar"] = 1, ["fizz"] = 3}).IsEqualTo(expected)).IsAFailingCheckWithMessage("", 
                "The checked dictionary is different from the expected one.", 
                "actual[\"foo\"] does not exist. Expected [\"foo\"]= 0.", 
                "The checked dictionary:", 
                "\t{[bar, 1],[fizz, 3]} (2 items)", 
                "The expected dictionary:", 
                "\t{[bar, 1],*[foo, 0]*,[fizz, 3]} (3 items)");
            Check.ThatCode( () =>
                Check.That(new Dictionary<string, int> { ["bar"] = 1, ["foo"] = 0, ["fizz"] = 3, ["extra"] = 2 }).IsEqualTo(expected)).IsAFailingCheckWithMessage("", 
                "The checked dictionary is different from the expected one.", 
                "actual[\"extra\"] should not exist (value 2).",
                "The checked dictionary:", 
                "\t{[bar, 1],[foo, 0],[fizz, 3],*[extra, 2]*} (4 items)", 
                "The expected dictionary:", 
                "\t{[bar, 1],[foo, 0],[fizz, 3]} (3 items)");
        }

        [Test]
        public void IsEqualToDealWithRecursion()
        {
            var dico = new Dictionary<string, object>();
            dico["first"] = dico;
            var expectedDico = new Dictionary<string, object>();
            expectedDico["first"] = expectedDico;
            // this used to fail (as a precaution against infinite recursion, but it has been properly implemented in V3.
            Check.That(dico).IsEqualTo(expectedDico);
        }

        [Test]
        public void SupportEquivalence()
        {
            var dictionary = new Dictionary<int, int[]> {[0] = new[] {1, 3, 2}};

            Check.ThatCode(() =>
                Check.That(dictionary).IsEqualTo(new Dictionary<int, int[]> {[0] = new[] {1, 2, 3}})).
                IsAFailingCheckWithMessage("", 
                "The checked dictionary is different from the expected one. 2 differences found! But they are equivalent.", 
                "actual[0][1] value ('3') was found at index 1 instead of 2.", 
                "actual[0][2] value ('2') was found at index 2 instead of 1.", 
                "The checked dictionary:", 
                "\t{[0, System.Int32[]]} (1 item)", 
                "The expected dictionary:", 
                "\t{[0, System.Int32[]]} (1 item)");
        }

        [Test]
        public void SupportIsEqualWithEnumerable()
        {
            var expectedDico = new Dictionary<string, object>();
            expectedDico["first"] = expectedDico;
            var sut = new List<string>{"first"};

            Check.ThatCode(() => Check.That(sut).IsEqualTo(expectedDico)).IsAFailingCheckWithMessage("",
                "The checked enumerable is different from the expected dictionary.",
                "\"first\" should not exist (found in actual[0]); [first, System.Collections.Generic.Dictionary`2[System.String,System.Object]] should be found instead.", 
                "The checked enumerable:", 
                "\t{\"first\"} (1 item) of type: [System.Collections.Generic.List<string>]", 
            "The expected dictionary:", 
            "\t{[first, System.Collections.Generic.Dictionary`2[System.String,System.Object]]} (1 item) of type: [System.Collections.Generic.Dictionary<string, object>]");
        }

        [Test]
        public void IsEquivalentToWorks()
        {
            var dict = new Dictionary<string, object> { ["foo"] = new[] { "bar", "baz" } };
            var expected = new Dictionary<string, object> { ["foo"] = new[] { "bar", "baz" } };
            Check.That(dict).IsEquivalentTo(expected);
            Check.That((IDictionary<string, string>) null).IsEquivalentTo((IDictionary<string, string>)null);
#if !NET35
            IReadOnlyDictionary<string, string> value = new RoDico(SimpleDico);
            Check.That(value).IsEqualTo(SimpleDico);
            Check.That(value).IsEquivalentTo(SimpleDico);
#endif
        }

        [Test]
        public void IsEquivalentToWorksEnumerationOfEntries()
        {
            var customDic = new List<KeyValuePair<string, int>> {
                new KeyValuePair<string, int>("otherKey", 15) ,
                new KeyValuePair<string, int>("key", 12)
            };
            var dic = new Dictionary<string, int>{["otherKey"]= 15, ["key"] = 12};
            Check.That(customDic).IsEquivalentTo(dic);
            Check.That(dic).IsEquivalentTo(customDic);
            dic["extra"] = 20;
            Check.ThatCode(() => Check.That(customDic).IsEquivalentTo(dic)).IsAFailingCheckWithMessage("", 
                "The checked enumerable is not equivalent to the expected dictionary.",
                "[\"extra\"]= 20 should be present but was not found.",
                "The checked enumerable:", 
                "\t{[otherKey, 15],[key, 12]} (2 items) of type: [System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<string, int>>]", 
                "The expected dictionary: equivalent to", 
                "\t{[otherKey, 15],[key, 12],*[extra, 20]*} (3 items) of type: [System.Collections.Generic.Dictionary<string, int>]");
        }
       [Test]
        public void IsEquivalentToWorksForCustomEnumerationOfEntries()
        {
            var customDic = new CustomEnumerable<KeyValuePair<string, int>> (new List<KeyValuePair<string, int>> {
                new KeyValuePair<string, int>("otherKey", 15) ,
                new KeyValuePair<string, int>("key", 12)
            });
            var dic = new Dictionary<string, int>{["otherKey"]= 15, ["key"] = 12};
            Check.That(customDic).IsEquivalentTo(dic);
            Check.That(dic).IsEquivalentTo(customDic);
            dic["extra"] = 20;
            Check.ThatCode(() => Check.That(customDic).IsEquivalentTo(dic)).IsAFailingCheckWithMessage("", 
                "The checked enumerable is not equivalent to the expected dictionary.",
                "[\"extra\"]= 20 should be present but was not found.",
                "The checked enumerable:", 
                "\t{[otherKey, 15],[key, 12]} (2 items) of type: [NFluent.Tests.Helpers.CustomEnumerable<System.Collections.Generic.KeyValuePair<string, int>>]", 
                "The expected dictionary: equivalent to", 
                "\t{[otherKey, 15],[key, 12],*[extra, 20]*} (3 items) of type: [System.Collections.Generic.Dictionary<string, int>]");
        }

        [Test]
        public void IsEquivalentToWorksWithCustomDics()
        {
            var customDic = new CustomDico<string, int>(new Dictionary<string, int>{["otherKey"]= 15, ["key"] = 12});
            var dic = new Dictionary<string, int>{["otherKey"]= 15, ["key"] = 12};
            Check.That(customDic).IsEquivalentTo(dic);
            Check.That(dic).IsEquivalentTo(customDic);
            Check.That(customDic).ContainsKey("key");
            Check.That(customDic).ContainsValue(12);
            Check.That(customDic).ContainsPair("key", 12);
            dic["extra"] = 20;
            Check.ThatCode(() => Check.That(customDic).IsEquivalentTo(dic)).IsAFailingCheckWithMessage("", 
                "The checked enumerable is not equivalent to the expected dictionary.",
                "[\"extra\"]= 20 should be present but was not found.",
                "The checked enumerable:", 
                "\t{[otherKey, 15],[key, 12]} (2 items) of type: [NFluent.Tests.CustomDico<string, int>]", 
                "The expected dictionary: equivalent to", 
                "\t{[otherKey, 15],[key, 12],*[extra, 20]*} (3 items) of type: [System.Collections.Generic.Dictionary<string, int>]");
        }

        [Test]
        public void IsIsEquivalentFailsWithProperErrorMessageWhenWrongKey()
        {
            var expected = new Dictionary<string, object> { ["foo"] = new[] { "bar", "baz" } };
            Check.ThatCode(() =>
                Check.That(new Dictionary<string, object> { ["bar"] = new[] { "bar", "baz" } }).IsEquivalentTo(expected)).IsAFailingCheckWithMessage(
                "", 
                "The checked dictionary is not equivalent to the expected one.",
                "[\"bar\"]= {\"bar\",\"baz\"} should not exist (found in actual); [\"foo\"]= {\"bar\",\"baz\"} should be found instead.",
                "The checked dictionary:", 
                "\t{[bar, System.String[]]} (1 item)", 
                "The expected dictionary: equivalent to", 
                "\t{[foo, System.String[]]} (1 item)");
        }

        [Test]
        public void IsIsEquivalentFailsWithProperErrorMessageWithWrongVal()
        {
            var expected = new Dictionary<string, object> { ["foo"] = new[] { "bar", "baz" } };
            Check.ThatCode(() =>
                Check.That(new Dictionary<string, object> { ["foo"] = new[] { "bar", "bar" } }).IsEquivalentTo(expected)).IsAFailingCheckWithMessage("", 
                "The checked dictionary is not equivalent to the expected one.", 
                "\"bar\" should not exist (found in actual[\"foo\"][1]); \"baz\" should be found instead.",
                "The checked dictionary:", 
                "\t{[foo, System.String[]]} (1 item)", 
                "The expected dictionary: equivalent to", 
                "\t{[foo, System.String[]]} (1 item)");
        }

        [Test]
        public void IsIsEquivalentFailsWithProperErrorMessageWhenEmptySut()
        {
            var expected = new Dictionary<string, object> { ["foo"] = new[] { "bar", "baz" } };

            // added due to GH #307
            Check.ThatCode(() =>
                Check.That(expected).IsEquivalentTo(new Dictionary<string, object>())).IsAFailingCheckWithMessage("", 
                "The checked dictionary is not equivalent to the expected one.",
                "actual[\"foo\"] value should not exist (value {\"bar\",\"baz\"})",
            "The checked dictionary:", 
                "\t{[foo, System.String[]]} (1 item)", 
                "The expected dictionary: equivalent to", 
                "\t{} (0 item)");
        }

        [Test]
        public void IsIsEquivalentFailsWithProperErrorMessageWhenSutIsNull()
        {
            var expected = new Dictionary<string, object> { ["foo"] = new[] { "bar", "baz" } };

            Check.ThatCode(() =>
                Check.That((IDictionary<string, object>) null).IsEquivalentTo(expected)).IsAFailingCheckWithMessage("", 
                "The checked enumerable is null whereas it should not.", 
                "The checked enumerable:", 
                "\t[null] of type: [System.Collections.Generic.IDictionary<string, object>]", 
                "The expected dictionary: equivalent to",
                "\t{[foo, System.String[]]} (1 item) of type: [System.Collections.Generic.Dictionary<string, object>]");

        }

        [Test]
        public void IsIsEquivalentFailsWithProperErrorMessageWhenExpectedIsNull()
        {
            var expected = new Dictionary<string, object> { ["foo"] = new[] { "bar", "baz" } };
            Check.ThatCode(() =>
                Check.That(new Dictionary<string, object> { ["foo"] = new[] { "bar", "bar" } }).IsEquivalentTo((IDictionary<string, object>)null)).IsAFailingCheckWithMessage(
                "", 
                "The checked dictionary should be null.", 
                "The checked dictionary:", 
                "\t{[foo, System.String[]]} (1 item)", 
                "The expected enumerable: equivalent to", 
                "\t[null]");
        }

        [Test]
        public void IsIsEquivalentFailsWithProperErrorMessageWhenNegated()
        {
            var dict = new Dictionary<string, object> { ["foo"] = new[] { "bar", "baz" } };
            var expected = new Dictionary<string, object> { ["foo"] = new[] { "bar", "baz" } };
            Check.ThatCode(() =>
                Check.That(dict).Not.IsEquivalentTo(expected)).IsAFailingCheckWithMessage("", 
                "The checked dictionary is equivalent to the given one whereas it must not.", 
                "The checked dictionary:", 
                "\t{[foo, System.String[]]} (1 item) of type: [System.Collections.Generic.Dictionary<string, object>]", 
                "The expected dictionary: different from (content)", 
                "\t{[foo, System.String[]]} (1 item) of type: [System.Collections.Generic.Dictionary<string, object>]");
        }
    }
}
