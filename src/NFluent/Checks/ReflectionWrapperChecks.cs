﻿// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="ReflectionWrapperChecks.cs" company="NFluent">
//   Copyright 2018 Thomas PIERRAIN & Cyrille DUPUYDAUBY
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

namespace NFluent
{
    using Extensibility;
    using Extensions;
    using Helpers;

    /// <summary>
    ///     Hosts reflection-based checks <see cref="ObjectFieldsCheckExtensions.Considering{T}" />
    /// </summary>
    public static class ReflectionWrapperChecks
    {
        /// <summary>
        /// </summary>
        /// <param name="check"></param>
        /// <param name="expectedValue"></param>
        /// <returns></returns>
        public static ICheckLink<ICheck<ReflectionWrapper>> IsEqualTo<TU>(this ICheck<ReflectionWrapper> check,
            TU expectedValue)
        {
            FieldEqualTest(check, expectedValue, true);
            var checker = ExtensibilityHelper.ExtractChecker(check);
            return checker.BuildChainingObject();
        }

        internal static void FieldEqualTest<TU>(ICheck<ReflectionWrapper> check, TU expectedValue, bool mustMatch)
        {
            ExtensibilityHelper.BeginCheck(check)
                .Analyze((sut, test) =>
                {
                    foreach (var match in sut.MemberMatches(expectedValue))
                    {
                        if (match.DoValuesMatches != mustMatch)
                        {
                            match.Check(test);
                        }
                    }
                }).Negates("The {0} is equal to one of {1} whereas it should not.")
                .EndCheck();
        }

        /// <summary>
        ///     Checks if the extracted members match one of the provided expected values.
        /// </summary>
        /// <param name="check">Checker logiv.</param>
        /// <param name="values">List of possible values</param>
        /// <returns>A link object</returns>
        public static ICheckLink<ICheck<ReflectionWrapper>> IsOneOf(this ICheck<ReflectionWrapper> check,
            params object[] values)
        {
            ExtensibilityHelper.BeginCheck(check)
                .ExpectingValues(values, values.Length, "one of", "none of")
                .Analyze((sut, test) =>
                {
                    bool match = false;
                    foreach (var value in values)
                    {
                        match = true;
                        foreach (var memberMatch in sut.MemberMatches(value))
                        {
                            if (!memberMatch.DoValuesMatches)
                            {
                                match = false;
                                break;
                            }

                        }

                        if (match)
                        {
                            break;
                        }
                    }

                    test.FailsIf(_ => !match, "The {0} is equal to none of the {1} whereas it should.");
                }).Negates("The {0} is equal to one of {1} whereas it should not.")
                .EndCheck();
            return ExtensibilityHelper.BuildCheckLink(check);
        }

        /// <inheritdoc cref="ObjectCheckExtensions.IsNull{T}(NFluent.ICheck{T})" />
        public static ICheckLink<ICheck<ReflectionWrapper>> IsNull(this ICheck<ReflectionWrapper> check)
        {
            ExtensibilityHelper.BeginCheck(check)
                .Analyze((sut, test) =>
                {
                    sut.ScanFields((scan, depth) =>
                    {
                        test.GetSutProperty(_ => scan.Value, scan.MemberLabel)
                            .FailsIf(val => depth <= 0 && val != null,
                                "The {0} is non null, whereas it should be.");
                        return !test.Failed && scan.Value != null;
                    });
                })
                .Negates("The {0} has only null member, whereas it should not.", MessageOption.NoCheckedBlock)
                .EndCheck();
            return ExtensibilityHelper.BuildCheckLink(check);
        }

        /// <inheritdoc cref="ObjectCheckExtensions.IsNotNull{T}(NFluent.ICheck{T})" />
        public static ICheckLink<ICheck<ReflectionWrapper>> IsNotNull(this ICheck<ReflectionWrapper> check)
        {
            ExtensibilityHelper.BeginCheck(check)
                .Analyze((sut, test) =>
                {
                    sut.ScanFields((scan, depth) =>
                    {
                        test.GetSutProperty(_ => scan.Value, scan.MemberLabel)
                            .FailsIf(val => depth <= 0 && val == null,
                                "The {0} is null, whereas it should not.", MessageOption.NoCheckedBlock);
                        return !test.Failed && scan.Value != null;
                    });
                })
                .Negates("The {0} has a non null member, whereas it should not.")
                .EndCheck();
            return ExtensibilityHelper.BuildCheckLink(check);
        }

        /// <inheritdoc cref="ObjectCheckExtensions.IsSameReferenceAs{T,TU}" />
        public static ICheckLink<ICheck<ReflectionWrapper>> IsSameReferenceAs<TU>(this ICheck<ReflectionWrapper> check,
            TU expected)
        {
            ExtensibilityHelper.BeginCheck(check)
                .Analyze((sut, test) =>
                {
                    sut.MemberMatches(expected);
                    foreach (var match in sut.MemberMatches(expected))
                    {
                        if (!match.ExpectedFieldFound)
                        {
                            test.GetSutProperty(_ => match.Expected.Value, match.Expected.MemberLabel)
                                .Fails("The {1} is absent from the {0}.", MessageOption.NoCheckedBlock)
                                .Expecting(match.Expected.Value);
                            break;
                        }

                        if (!match.ActualFieldFound)
                        {
                            test.GetSutProperty(_ => match.Actual.Value, match.Actual.MemberLabel)
                                .Fails("The {0} is absent from the {1}.");
                            break;
                        }
                        if (!ReferenceEquals(match.Actual.Value, match.Expected.Value))
                        {
                            test.GetSutProperty(_ => match.Actual.Value, match.Actual.MemberLabel)
                                .Fails("The {0} does not reference the {1}.")
                                .Expecting(match.Expected.Value);
                            break;
                        }
                    }
                })
                .Negates("The {0} contains the same reference than the {1}, whereas it should not.")
                .EndCheck();
            return ExtensibilityHelper.BuildCheckLink(check);
        }

        /// <inheritdoc cref="ObjectCheckExtensions.IsDistinctFrom{T,TU}" />
        public static ICheckLink<ICheck<ReflectionWrapper>> IsDistinctFrom<TU>(this ICheck<ReflectionWrapper> check,
            TU expected)
        {
            ExtensibilityHelper.BeginCheck(check)
                .Analyze((sut, test) =>
                {
                    sut.MemberMatches(expected);
                    foreach (var match in sut.MemberMatches(expected))
                    {
                        if (!match.ExpectedFieldFound)
                        {
                            test.GetSutProperty(_ => match.Expected.Value, match.Expected.MemberLabel)
                                .Fails("The {1} is absent from the {0}.", MessageOption.NoCheckedBlock)
                                .Expecting(match.Expected.Value);
                            break;
                        }

                        if (!match.ActualFieldFound)
                        {
                            test.GetSutProperty(_ => match.Actual.Value, match.Actual.MemberLabel)
                                .Fails("The {0} is absent from the {1}.");
                            break;
                        }
                        if (ReferenceEquals(match.Actual.Value, match.Expected.Value))
                        {
                            test.GetSutProperty(_ => match.Actual.Value, match.Actual.MemberLabel)
                                .Fails("The {0} does reference the {1}, wheras it should not.", MessageOption.NoCheckedBlock)
                                .ComparingTo(match.Expected.Value, "different instance than", "same instance than");
                            break;
                        }
                    }
                })
                .Negates("The {0} contains the same reference than the {1}, whereas it should not.")
                .EndCheck();
            return ExtensibilityHelper.BuildCheckLink(check);
        }
    }
}