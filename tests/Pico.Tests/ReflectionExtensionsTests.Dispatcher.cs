using System.Collections;

namespace Pico.Tests;

public partial class ReflectionExtensionsTests
{
    public interface ISampleVoidDispatcher
    {
        void SomeMethod(
            string message,
            IComparer comparer,
            CancellationToken token);

        void SomeMethod(
            int message,
            IServiceProvider services);

        void OtherMethod(bool value1);
    }

    [Fact]
    public void CreateVoidDispatcher()
    {
        // GIVEN
        var dispatcher = typeof(ISampleVoidDispatcher)
            .CreateVoidDispatcher("SomeMethod");

        // THEN
        dispatcher.CanDispatch(typeof(string)).Should().BeTrue();

        // THEN
        dispatcher.CanDispatch(typeof(int)).Should().BeTrue();

        // THEN
        dispatcher.CanDispatch(typeof(bool)).Should().BeFalse();

        // GIVEN
        var target = new Mock<ISampleVoidDispatcher>(MockBehavior.Loose);

        // THEN
        target.Verify(
            e => e.SomeMethod(
                It.IsAny<string>(),
                It.IsAny<IComparer>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        // THEN
        target.Verify(
            e => e.SomeMethod(
                It.IsAny<int>(),
                It.IsAny<IServiceProvider>()),
            Times.Never);

        // THEN
        target.Verify(
            e => e.OtherMethod(
                It.IsAny<bool>()),
            Times.Never);

        // GIVEN
        var ctoken = new CancellationTokenSource().Token;

        // GIVEN
        var comparer = new Mock<IComparer>(MockBehavior.Loose);

        // GIVEN
        var resolve = ParameterResolver.Builder()
            .ForInstances("message value", ctoken)
            .ForInstance(comparer.Object)
            .Build();

        // WHEN
        dispatcher.Dispatch(target.Object, typeof(string), resolve);

        // THEN
        target.Verify(
            e => e.SomeMethod("message value", comparer.Object, ctoken),
            Times.Once);

        // WHEN
        var action = () => dispatcher.Dispatch(target.Object, typeof(char), resolve);

        // THEN
        action.Should().Throw<KeyNotFoundException>();
    }

    public interface ISampleTaskDispatcher
    {
        Task SomeMethodAsync(
            string message,
            IComparer comparer,
            CancellationToken token);

        Task SomeMethodAsync(
            int message,
            IServiceProvider services);

        Task OtherMethod(bool value1);
    }

    [Fact]
    public async Task CreateTaskDispatcher()
    {
        // GIVEN
        var dispatcher = typeof(ISampleTaskDispatcher)
            .CreateTaskDispatcher("SomeMethodAsync");

        // THEN
        dispatcher.CanDispatch(typeof(string)).Should().BeTrue();

        // THEN
        dispatcher.CanDispatch(typeof(int)).Should().BeTrue();

        // THEN
        dispatcher.CanDispatch(typeof(bool)).Should().BeFalse();

        // GIVEN
        var target = new Mock<ISampleTaskDispatcher>(MockBehavior.Loose);

        // THEN
        target.Verify(
            e => e.SomeMethodAsync(
                It.IsAny<string>(),
                It.IsAny<IComparer>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        // THEN
        target.Verify(
            e => e.SomeMethodAsync(
                It.IsAny<int>(),
                It.IsAny<IServiceProvider>()),
            Times.Never);

        // THEN
        target.Verify(
            e => e.OtherMethod(
                It.IsAny<bool>()),
            Times.Never);

        // GIVEN
        var ctoken = new CancellationTokenSource().Token;

        // GIVEN
        var comparer = new Mock<IComparer>(MockBehavior.Loose);

        // GIVEN
        var resolve = ParameterResolver.Builder()
            .ForInstances("message value", ctoken)
            .ForFactory(() => comparer.Object)
            .Build();

        // WHEN
        await dispatcher.Dispatch(target.Object, typeof(string), resolve);

        // THEN
        target.Verify(
            e => e.SomeMethodAsync("message value", comparer.Object, ctoken),
            Times.Once);

        // WHEN
        var action = () => dispatcher.Dispatch(target.Object, typeof(char), resolve);

        // THEN
        await action.Should().ThrowAsync<KeyNotFoundException>();
    }
}
