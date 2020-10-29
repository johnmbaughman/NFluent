﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = NFluent.Analyzer.Test.CSharpCodeFixVerifier<
    NFluent.Analyzer.NFluentAnalyzer,
    NFluent.Analyzer.NFluentAnalyzerCodeFixProvider>;
using VerifyCSAnalyzer = NFluent.Analyzer.Test.CSharpAnalyzerVerifier<NFluent.Analyzer.NFluentAnalyzer>;

namespace NFluent.Analyzer.Test
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis.Testing;

    [TestClass]
    public class NFluentAnalyzerShould
    {
        private const string SampleCodeTemplate = @"using NFluent;
    using System.Collections.Generic;
    namespace ConsoleApplication1
    {{
        class TestClass
        {{
            private int x;

            public void SampleCheckMethod()
            {{
                {0}
            }}
        }}
    }}";

        private static string SampleCodeFromCheck(string checkCode)
        {
            return string.Format(SampleCodeTemplate, checkCode);
        }

        //No diagnostics expected to show up
        [TestMethod]
        public async Task StaySilentWhenNoCode()
        {
            await VerifyCS.VerifyAnalyzerAsync(@"");
        }

        // Diagnostic test
        [TestMethod]
        public async Task ReportCheckThatExpression()
        {
            var testCode = SampleCodeFromCheck("Check.That(10);");

            var referenceAssemblies = ReferenceAssemblies.Default.AddPackages(ImmutableArray.Create(new PackageIdentity("NFluent", "2.7.0")));
            var test = new VerifyCSAnalyzer.Test
            {
                TestCode =  testCode,
                ExpectedDiagnostics = {VerifyCS.Diagnostic(NFluentAnalyzer.MissingCheckId).WithArguments("10").WithLocation(11,17)},
                ReferenceAssemblies = referenceAssemblies
            };

            await test.RunAsync();
        }

        [TestMethod]
        public async Task DontReportProperCheckThatExpression()
        {
            var testCode = SampleCodeFromCheck("Check.That(10).IsZero();");

            var referenceAssemblies = ReferenceAssemblies.Default.AddPackages(ImmutableArray.Create(new PackageIdentity("NFluent", "2.7.0")));
            var test = new VerifyCSAnalyzer.Test
            {
                TestCode =  testCode,
                ReferenceAssemblies = referenceAssemblies
            };

            await test.RunAsync();
        }

        // Diagnostic test
        [DataTestMethod]
        [DataRow("this", "IsNotNull()")]
        [DataRow("true", "IsTrue()")]
        [DataRow("10", "IsNotZero()")]
        [DataRow("10.0", "IsNotZero()")]
        [DataRow("10m", "IsNotZero()")]
        [DataRow("10.0f", "IsNotZero()")]
        [DataRow("new []{1,2}", "Not.IsEmpty()")]
        [DataRow("new List<int>{1}", "Not.IsEmpty()")]
        [DataRow("(int?)2", "IsNotNull()")]
        [DataRow("\"Test\"", "IsNotEmpty()")]
        public async Task ReportAndFixCheckThatExpression(string sut, string check)
        {
            var testCode = SampleCodeFromCheck($"Check.That({sut});");
            var fixTest = SampleCodeFromCheck($"Check.That({sut}).{check};");

            var referenceAssemblies = ReferenceAssemblies.Default.AddPackages(ImmutableArray.Create(new PackageIdentity("NFluent", "2.7.0")));
            var test = new VerifyCS.Test
            {
                TestCode =  testCode,
                FixedCode = fixTest,
                ExpectedDiagnostics = {VerifyCS.Diagnostic(NFluentAnalyzer.MissingCheckId).WithArguments(sut).WithLocation(11,17)},
                ReferenceAssemblies = referenceAssemblies
            };

            await test.RunAsync();
        }

        [TestMethod]
        public async Task ReportCheckWithCustomMessage()
        {
            var testCode = SampleCodeFromCheck(@"Check.WithCustomMessage(""dont care"").That(10);");
 
            var referenceAssemblies = ReferenceAssemblies.Default.AddPackages(ImmutableArray.Create(new PackageIdentity("NFluent", "2.7.0")));
            var test = new VerifyCSAnalyzer.Test
            {
                TestCode =  testCode,
                ExpectedDiagnostics = {VerifyCS.Diagnostic(NFluentAnalyzer.MissingCheckId).WithArguments(10).WithLocation(11,17)},
                ReferenceAssemblies = referenceAssemblies
            };

            await test.RunAsync();
        }

        [TestMethod]
        public async Task ReportCheckWithAs()
        {
            var testCode = SampleCodeFromCheck(@"Check.That(10).As(""number"");");

            var referenceAssemblies = ReferenceAssemblies.Default.AddPackages(ImmutableArray.Create(new PackageIdentity("NFluent", "2.7.0")));
            var test = new VerifyCSAnalyzer.Test
            {
                TestCode =  testCode,
                ExpectedDiagnostics = {VerifyCS.Diagnostic(NFluentAnalyzer.MissingCheckId).WithArguments(10).WithLocation(11,17)},
                ReferenceAssemblies = referenceAssemblies
            };

            await test.RunAsync();
        }

        [TestMethod]
        [DataTestMethod]
        [DataRow("(10 == 10).IsTrue()", "10", "IsEqualTo", "10")]
        [DataRow("(10 != 10).IsTrue()", "10", "IsNotEqualTo", "10")]
        [DataRow("(10 < 15).IsTrue()", "10", "IsStrictlyLessThan", "15")]
        [DataRow("(10 < x).IsTrue()", "x", "IsStrictlyGreaterThan", "10")]
        [DataRow("(x > 10).IsTrue()", "x", "IsStrictlyGreaterThan", "10")]
        [DataRow("(10 > x).IsTrue()", "x", "IsStrictlyLessThan", "10")]
        [DataRow("(x >= 10).IsTrue()", "x", "IsAfter", "10")]
        [DataRow("(10 >= x).IsTrue()", "x", "IsBefore", "10")]
        [DataRow("(x <= 10).IsTrue()", "x", "IsBefore", "10")]
        [DataRow("(10 <= x).IsTrue()", "x", "IsAfter", "10")]
        public async Task FixBinaryExpression(string testBit, string sut, string checkName, string refValue)
        {
            var testCode = SampleCodeFromCheck($"Check.That{testBit};");
            var fixedCode = SampleCodeFromCheck($"Check.That({sut}).{checkName}({refValue});");

            var referenceAssemblies = ReferenceAssemblies.Default.AddPackages(ImmutableArray.Create(new PackageIdentity("NFluent", "2.7.0")));
            var test = new VerifyCS.Test
            {
                TestCode =  testCode, 
                FixedCode =  fixedCode,
                ExpectedDiagnostics = {VerifyCS.Diagnostic(NFluentAnalyzer.SutIsTheCheckId).WithArguments(sut, checkName).WithLocation(11,17)},
                ReferenceAssemblies = referenceAssemblies
            };

            await test.RunAsync();
        }

        [TestMethod]
        [DataTestMethod]
        [DataRow("(10 == 10).IsFalse()", "10", "IsEqualTo", "10")]
        public async Task FixFalseBinaryExpression(string testBit, string sut, string checkName, string refValue)
        {
            var testCode = SampleCodeFromCheck($"Check.That{testBit};");
            var fixedCode = SampleCodeFromCheck($"Check.That({sut}).Not.{checkName}({refValue});");

            var referenceAssemblies = ReferenceAssemblies.Default.AddPackages(ImmutableArray.Create(new PackageIdentity("NFluent", "2.7.0")));
            var test = new VerifyCS.Test
            {
                TestCode =  testCode, 
                FixedCode =  fixedCode,
                ExpectedDiagnostics = {VerifyCS.Diagnostic(NFluentAnalyzer.SutIsTheCheckId).WithArguments(sut, checkName).WithLocation(11,17)},
                ReferenceAssemblies = referenceAssemblies
            };

            await test.RunAsync();
        }

    }
}