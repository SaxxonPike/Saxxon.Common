using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Dsl;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;

// ReSharper disable CheckNamespace
// ReSharper disable UnusedMember.Global

namespace Saxxon.TestInfrastructure
{
    [TestFixture]
    [Parallelizable]
    public abstract class TestBase
    {
        private static readonly ConcurrentDictionary<string, Fixture> Fixtures =
            new ConcurrentDictionary<string, Fixture>();

        private static Randomizer Random
        {
            [DebuggerStepThrough] get => TestContext.CurrentContext.Random;
        }

        private static Fixture Fixture
        {
            [DebuggerStepThrough] get => Fixtures[TestContext.CurrentContext.Test.ID];
        }

        [DebuggerStepThrough]
        protected static void Fail(string message = "") => Assert.Fail(message);

        [DebuggerStepThrough]
        protected static void Pass(string message = "") => Assert.Pass(message);

        [DebuggerStepThrough]
        protected static void Ignore(string message = "") => Assert.Ignore(message);

        [DebuggerStepThrough]
        protected static Mock<T> Mock<T>() where T : class => Fixture.Freeze<Mock<T>>();

        [DebuggerStepThrough]
        protected static T Create<T>() => Fixture.Create<T>();

        [DebuggerStepThrough]
        protected static IEnumerable<T> CreateMany<T>() => Fixture.CreateMany<T>();

        [DebuggerStepThrough]
        protected static IEnumerable<T> CreateMany<T>(int count) => Fixture.CreateMany<T>(count);

        [DebuggerStepThrough]
        protected static ICustomizationComposer<T> Build<T>() => Fixture.Build<T>();

        [DebuggerStepThrough]
        protected static int RandomInt() => Random.Next();

        [SetUp]
        [Obsolete("Do not call this directly.")]
        [DebuggerStepThrough]
        public void TestBaseSetUp()
        {
            var fixture = new Fixture();
            fixture.Customize(new AutoMoqCustomization());
            fixture.Customize(new SupportMutableValueTypesCustomization());
            Assert.That(Fixtures.TryAdd(TestContext.CurrentContext.Test.ID, fixture),
                "A unique fixture must be concurrently available for this test.");
        }

        [TearDown]
        [Obsolete("Do not call this directly.")]
        [DebuggerStepThrough]
        public void TestBaseTearDown()
        {
            Fixtures.TryRemove(TestContext.CurrentContext.Test.ID, out _);
        }
    }
}