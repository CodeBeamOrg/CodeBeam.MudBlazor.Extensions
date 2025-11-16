using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudExtensions;
using MudExtensions.UnitTests.TestComponents;

namespace MudExtensions.UnitTests.Components
{
    [TestFixture]
    public class StepperTests : BunitTest
    {
        [Test]
        public void StepperRenderTest()
        {
            var comp = Context.RenderComponent<MudStepperExtended>();
            comp.Instance.Steps.Count.Should().Be(0);
        }

        [Test]
        public async Task StepperPreventStepChangeDirectionIsForwardWhenCompletingStepOneOfOneTest()
        {
            // Arrange
            var lastStepChangeDirection = StepChangeDirection.None;
            int _targetIndex = 0;
            var stepper = Context.RenderComponent<MudStepperExtended>(
                ComponentParameterFactory.Parameter(
                    nameof(MudStepperExtended.PreventStepChangeAsync),
                    new Func<StepChangeDirection, int, Task<bool>>((direction, targetIndex) =>
                    {
                        lastStepChangeDirection = direction;
                        _targetIndex = targetIndex;
                        return Task.FromResult(false);
                    })
                )
            );
            var step0 = Context.RenderComponent<MudStepExtended>(
                ComponentParameterFactory.CascadingValue(stepper.Instance)
            );

            // Act
            await stepper.Instance.CompleteStep(0, moveToNextStep: true);

            // Assert
            lastStepChangeDirection.Should().Be(StepChangeDirection.Forward);
        }

        [Test]
        public async Task StepperPreventStepChangeDirectionIsForwardWhenSkippingStepOneOfOneTest()
        {
            // Arrange
            var lastStepChangeDirection = StepChangeDirection.None;
            int _targetIndex = 0;
            var stepper = Context.RenderComponent<MudStepperExtended>(
                ComponentParameterFactory.Parameter(
                    nameof(MudStepperExtended.PreventStepChangeAsync),
                    new Func<StepChangeDirection, int, Task<bool>>((direction, targetIndex) =>
                    {
                        lastStepChangeDirection = direction;
                        _targetIndex = targetIndex;
                        return Task.FromResult(false);
                    })
                )
            );
            var step0 = Context.RenderComponent<MudStepExtended>(
                ComponentParameterFactory.CascadingValue(stepper.Instance)
            );

            // Act
            await stepper.Instance.SkipStep(0, moveToNextStep: true);

            // Assert
            lastStepChangeDirection.Should().Be(StepChangeDirection.Forward);
        }

        [Test]
        public async Task StepperPreventStepChangeDirectionIsForwardWhenChangingFromStepOneToStepTwoTest()
        {
            var lastStepChangeDirection = StepChangeDirection.None;
            int capturedTargetIndex = -1;

            var stepper = Context.RenderComponent<MudStepperExtended>(
                ComponentParameterFactory.Parameter(
                    nameof(MudStepperExtended.PreventStepChangeAsync),
                    new Func<StepChangeDirection, int, Task<bool>>(
                        (direction, targetIndex) =>
                        {
                            lastStepChangeDirection = direction;
                            capturedTargetIndex = targetIndex;
                            return Task.FromResult(false);
                        })
                )
            );

            var step0 = Context.RenderComponent<MudStepExtended>(
                ComponentParameterFactory.CascadingValue(stepper.Instance)
            );
            var step1 = Context.RenderComponent<MudStepExtended>(
                ComponentParameterFactory.CascadingValue(stepper.Instance)
            );

            await stepper.Instance.GoToStepByReferenceAsync(step0.Instance);
            await stepper.Instance.GoToStepAsync(1);

            lastStepChangeDirection.Should().Be(StepChangeDirection.Forward);
            capturedTargetIndex.Should().Be(1);
        }

        [Test]
        public async Task StepperPreventStepChangeDirectionIsBackwardWhenChangingFromStepTwoToStepOneTest()
        {
            var lastStepChangeDirection = StepChangeDirection.None;
            int capturedTargetIndex = -1;

            var stepper = Context.RenderComponent<MudStepperExtended>(
                ComponentParameterFactory.Parameter(
                    nameof(MudStepperExtended.PreventStepChangeAsync),
                    new Func<StepChangeDirection, int, Task<bool>>(
                        (direction, targetIndex) =>
                        {
                            lastStepChangeDirection = direction;
                            capturedTargetIndex = targetIndex;
                            return Task.FromResult(false);
                        })
                )
            );

            var step0 = Context.RenderComponent<MudStepExtended>(
                ComponentParameterFactory.CascadingValue(stepper.Instance)
            );

            var step1 = Context.RenderComponent<MudStepExtended>(
                ComponentParameterFactory.CascadingValue(stepper.Instance)
            );

            await stepper.Instance.GoToStepByReferenceAsync(step1.Instance);
            await stepper.Instance.GoPreviousStepAsync();

            lastStepChangeDirection.Should().Be(StepChangeDirection.Backward);
            capturedTargetIndex.Should().Be(0);
        }

        [Test]
        public async Task StepperPreventStepChangeDirectionIsNoneWhenChangingToTheSameStepTest()
        {
            var lastStepChangeDirection = StepChangeDirection.None;

            var stepper = Context.RenderComponent<MudStepperExtended>(
                ComponentParameterFactory.Parameter(
                    nameof(MudStepperExtended.PreventStepChangeAsync),
                    new Func<StepChangeDirection, int, Task<bool>>(
                        (direction, targetIndex) =>
                        {
                            lastStepChangeDirection = direction;
                            return Task.FromResult(false);
                        })
                )
            );

            var step0 = Context.RenderComponent<MudStepExtended>(
                ComponentParameterFactory.CascadingValue(stepper.Instance)
            );

            await stepper.Instance.GoToStepByReferenceAsync(step0.Instance);
            await stepper.Instance.GoToStepAsync(0);

            lastStepChangeDirection.Should().Be(StepChangeDirection.None);
        }

        [Test]
        public async Task StepperPreventStepChangeIsInvokedWhenCompletingActiveStepTest()
        {
            var preventStepChangeWasInvoked = false;

            var stepper = Context.RenderComponent<MudStepperExtended>(
                ComponentParameterFactory.Parameter(
                    nameof(MudStepperExtended.PreventStepChangeAsync),
                    new Func<StepChangeDirection, int, Task<bool>>(
                        (direction, targetIndex) =>
                        {
                            preventStepChangeWasInvoked = true;
                            return Task.FromResult(false);
                        })
                )
            );

            var step0 = Context.RenderComponent<MudStepExtended>(
                ComponentParameterFactory.CascadingValue(stepper.Instance)
            );

            var step1 = Context.RenderComponent<MudStepExtended>(
                ComponentParameterFactory.CascadingValue(stepper.Instance)
            );

            await stepper.Instance.GoToStepByReferenceAsync(step0.Instance);
            await stepper.Instance.CompleteStep(stepper.Instance.Steps.IndexOf(step0.Instance));

            preventStepChangeWasInvoked.Should().BeTrue();
        }

        [Test]
        public async Task StepperPreventStepChangeIsInvokedWhenSkippingActiveStepTest()
        {
            var preventStepChangeWasInvoked = false;

            var stepper = Context.RenderComponent<MudStepperExtended>(
                ComponentParameterFactory.Parameter(
                    nameof(MudStepperExtended.PreventStepChangeAsync),
                    new Func<StepChangeDirection, int, Task<bool>>(
                        (direction, targetIndex) =>
                        {
                            preventStepChangeWasInvoked = true;
                            return Task.FromResult(false);
                        })
                )
            );

            var step0 = Context.RenderComponent<MudStepExtended>(
                ComponentParameterFactory.CascadingValue(stepper.Instance)
            );
            var step1 = Context.RenderComponent<MudStepExtended>(
                ComponentParameterFactory.CascadingValue(stepper.Instance)
            );

            await stepper.Instance.GoToStepByReferenceAsync(step0.Instance);
            await stepper.Instance.SkipStep(stepper.Instance.Steps.IndexOf(step0.Instance));

            preventStepChangeWasInvoked.Should().BeTrue();
        }

        [Test]
        public async Task StepperPreventStepChangeIsNotInvokedWhenCompletingNonActiveStepTest()
        {
            var preventStepChangeWasInvoked = false;

            var stepper = Context.RenderComponent<MudStepperExtended>(
                ComponentParameterFactory.Parameter(
                    nameof(MudStepperExtended.PreventStepChangeAsync),
                    new Func<StepChangeDirection, int, Task<bool>>((direction, targetIndex) =>
                    {
                        preventStepChangeWasInvoked = true;
                        return Task.FromResult(false);
                    })
                )
            );

            var step0 = Context.RenderComponent<MudStepExtended>(
                ComponentParameterFactory.CascadingValue(stepper.Instance),
                ComponentParameterFactory.Parameter(nameof(MudStepExtended.Order), 0)
            );

            var step1 = Context.RenderComponent<MudStepExtended>(
                ComponentParameterFactory.CascadingValue(stepper.Instance),
                ComponentParameterFactory.Parameter(nameof(MudStepExtended.Order), 1)
            );

            await stepper.Instance.GoToStepAsync(0, skipPrevent: true);
            await stepper.Instance.CompleteStep(1);

            preventStepChangeWasInvoked.Should().BeFalse();
        }

        [Test]
        public async Task StepperPreventStepChangeIsNotInvokedWhenSkippingNonActiveStepTest()
        {
            var preventStepChangeWasInvoked = false;

            var stepper = Context.RenderComponent<MudStepperExtended>(
                ComponentParameterFactory.Parameter(
                    nameof(MudStepperExtended.PreventStepChangeAsync),
                    new Func<StepChangeDirection, int, Task<bool>>((direction, targetIndex) =>
                    {
                        preventStepChangeWasInvoked = true;
                        return Task.FromResult(false);
                    })
                )
            );

            var step0 = Context.RenderComponent<MudStepExtended>(
                ComponentParameterFactory.CascadingValue(stepper.Instance),
                ComponentParameterFactory.Parameter(nameof(MudStepExtended.Order), 0)
            );

            var step1 = Context.RenderComponent<MudStepExtended>(
                ComponentParameterFactory.CascadingValue(stepper.Instance),
                ComponentParameterFactory.Parameter(nameof(MudStepExtended.Order), 1)
            );

            await stepper.Instance.GoToStepAsync(0, skipPrevent: true);

            int indexOfStep1 = stepper.Instance.Steps.IndexOf(step1.Instance);
            await stepper.Instance.SkipStep(indexOfStep1);

            preventStepChangeWasInvoked.Should().BeFalse();
        }

        [Test]
        public async Task StepperActiveIndexIsNotChangedWhenCompletingNonActiveStepTest()
        {
            var stepper = Context.RenderComponent<MudStepperExtended>();

            var step0 = Context.RenderComponent<MudStepExtended>(
                ComponentParameterFactory.CascadingValue(stepper.Instance)
            );

            var step1 = Context.RenderComponent<MudStepExtended>(
                ComponentParameterFactory.CascadingValue(stepper.Instance)
            );

            await stepper.Instance.GoToStepByReferenceAsync(step0.Instance, skipPrevent: true);

            int activeBefore = stepper.Instance.ActiveIndex;

            int step1Index = stepper.Instance.Steps.IndexOf(step1.Instance);
            await stepper.Instance.CompleteStep(step1Index, moveToNextStep: true);

            stepper.Instance.ActiveIndex.Should().Be(activeBefore);
        }

        [Test]
        public async Task StepperActiveIndexIsNotChangedWhenSkippingNonActiveStepTest()
        {
            var stepper = Context.RenderComponent<MudStepperExtended>();

            var step0 = Context.RenderComponent<MudStepExtended>(
                ComponentParameterFactory.CascadingValue(stepper.Instance)
            );

            var step1 = Context.RenderComponent<MudStepExtended>(
                ComponentParameterFactory.CascadingValue(stepper.Instance)
            );

            await stepper.Instance.GoToStepByReferenceAsync(step0.Instance, skipPrevent: true);

            int activeBefore = stepper.Instance.ActiveIndex;
            int step1Index = stepper.Instance.Steps.IndexOf(step1.Instance);
            await stepper.Instance.SkipStep(step1Index, moveToNextStep: true);

            stepper.Instance.ActiveIndex.Should().Be(activeBefore);
        }

        [Test]
        public async Task StepperCheckChangeCountTest()
        {
            var comp = Context.RenderComponent<StepperTest1>();
            var stepper = comp.FindComponent<MudStepperExtended>();

            comp.Instance.CheckChangeCount.Should().Be(0);

            await comp.InvokeAsync(() => stepper.Instance.GoToStepAsync(1));
            comp.WaitForAssertion(() => comp.Instance.CheckChangeCount.Should().Be(1));

            await comp.InvokeAsync(() => stepper.Instance.GoToStepAsync(0));
            comp.WaitForAssertion(() => comp.Instance.CheckChangeCount.Should().Be(2));
        }

        [Test]
        public async Task StepperNavigationIsBlockedWhenPreventReturnsTrueTest()
        {
            // Arrange
            var stepper = Context.RenderComponent<MudStepperExtended>(
                ComponentParameterFactory.Parameter(
                    nameof(MudStepperExtended.PreventStepChangeAsync),
                    new Func<StepChangeDirection, int, Task<bool>>((_, __) => Task.FromResult(true))
                )
            );

            var step0 = Context.RenderComponent<MudStepExtended>(ComponentParameterFactory.CascadingValue(stepper.Instance));
            var step1 = Context.RenderComponent<MudStepExtended>(ComponentParameterFactory.CascadingValue(stepper.Instance));

            await stepper.Instance.GoToStepAsync(0);
            await stepper.Instance.GoToStepAsync(1);

            stepper.Instance.ActiveIndex.Should().Be(stepper.Instance.Steps.IndexOf(step0.Instance));
        }

        [Test]
        public async Task StepperGoNextIsBlockedWhenPreventReturnsTrueTest()
        {
            var stepper = Context.RenderComponent<MudStepperExtended>(
                ComponentParameterFactory.Parameter(
                    nameof(MudStepperExtended.PreventStepChangeAsync),
                    new Func<StepChangeDirection, int, Task<bool>>((_, __) => Task.FromResult(true))
                )
            );

            var step0 = Context.RenderComponent<MudStepExtended>(ComponentParameterFactory.CascadingValue(stepper.Instance));
            var step1 = Context.RenderComponent<MudStepExtended>(ComponentParameterFactory.CascadingValue(stepper.Instance));

            await stepper.Instance.GoToStepAsync(0);
            await stepper.Instance.GoNextStepAsync();

            stepper.Instance.ActiveIndex.Should().Be(0);
        }

        [Test]
        public async Task StepperNavigatesToResultStepWhenAllStepsCompletedTest()
        {
            var stepper = Context.RenderComponent<MudStepperExtended>();

            var step0 = Context.RenderComponent<MudStepExtended>(ComponentParameterFactory.CascadingValue(stepper.Instance));
            var step1 = Context.RenderComponent<MudStepExtended>(ComponentParameterFactory.CascadingValue(stepper.Instance));
            var resultStep = Context.RenderComponent<MudStepExtended>(
                ComponentParameterFactory.CascadingValue(stepper.Instance),
                ComponentParameterFactory.Parameter("IsResultStep", true)
            );

            await stepper.Instance.GoToStepAsync(0);
            await stepper.Instance.CompleteStep(0);
            await stepper.Instance.CompleteStep(1);
            await stepper.Instance.GoNextStepAsync();

            stepper.Instance.ActiveIndex.Should().Be(2);
        }

        [Test]
        public async Task StepperCallsBeforeFinishedWhenCompletingLastRemainingStepTest()
        {
            bool beforeCalled = false;

            var stepper = Context.RenderComponent<MudStepperExtended>(
                ComponentParameterFactory.Parameter(
                    nameof(MudStepperExtended.BeforeFinishedAsync),
                    new Func<Task<bool>>(() =>
                    {
                        beforeCalled = true;
                        return Task.FromResult(true);
                    })
                )
            );

            var step0 = Context.RenderComponent<MudStepExtended>(ComponentParameterFactory.CascadingValue(stepper.Instance));
            var step1 = Context.RenderComponent<MudStepExtended>(ComponentParameterFactory.CascadingValue(stepper.Instance));

            await stepper.Instance.GoToStepAsync(0);
            await stepper.Instance.CompleteStep(0);
            await stepper.Instance.CompleteStep(1);

            beforeCalled.Should().BeTrue();
        }

        [Test]
        public async Task StepperCallsOnFinishedWhenCompletingLastRemainingStepTest()
        {
            // Arrange
            bool finishedCalled = false;

            var stepper = Context.RenderComponent<MudStepperExtended>(
                ComponentParameterFactory.Parameter(
                    nameof(MudStepperExtended.OnFinished),
                    EventCallback.Factory.Create(this, () => finishedCalled = true)
                )
            );

            var step0 = Context.RenderComponent<MudStepExtended>(ComponentParameterFactory.CascadingValue(stepper.Instance));
            var step1 = Context.RenderComponent<MudStepExtended>(ComponentParameterFactory.CascadingValue(stepper.Instance));

            await stepper.Instance.GoToStepAsync(0);
            await stepper.Instance.CompleteStep(0);
            await stepper.Instance.CompleteStep(1);

            finishedCalled.Should().BeTrue();
        }

        [Test]
        public async Task StepperGoByIndexComputesDirectionCorrectlyTest()
        {
            StepChangeDirection? captured = null;

            var stepper = Context.RenderComponent<MudStepperExtended>(
                ComponentParameterFactory.Parameter(
                    nameof(MudStepperExtended.PreventStepChangeAsync),
                    new Func<StepChangeDirection, int, Task<bool>>((dir, _) =>
                    {
                        captured = dir;
                        return Task.FromResult(false);
                    })
                )
            );

            var step0 = Context.RenderComponent<MudStepExtended>(ComponentParameterFactory.CascadingValue(stepper.Instance));
            var step1 = Context.RenderComponent<MudStepExtended>(ComponentParameterFactory.CascadingValue(stepper.Instance));

            await stepper.Instance.GoToStepAsync(0);
            await stepper.Instance.GoByIndexAsync(1);

            captured.Should().Be(StepChangeDirection.Forward);
        }

        [Test]
        public async Task StepperSetsStatusContinuedWhenNavigatingToUnstartedStepTest()
        {
            var stepper = Context.RenderComponent<MudStepperExtended>();
            var step0 = Context.RenderComponent<MudStepExtended>(ComponentParameterFactory.CascadingValue(stepper.Instance));
            var step1 = Context.RenderComponent<MudStepExtended>(ComponentParameterFactory.CascadingValue(stepper.Instance));

            await stepper.Instance.GoToStepAsync(0);
            await stepper.Instance.GoToStepAsync(1);

            step1.Instance.Status.Should().Be(StepStatus.Continued);
        }

    }
}
