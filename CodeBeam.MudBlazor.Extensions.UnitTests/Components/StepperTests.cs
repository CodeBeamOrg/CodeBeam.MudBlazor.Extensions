using Bunit;
using AwesomeAssertions;
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
            var comp = Context.Render<MudStepperExtended>();
            comp.Instance.Steps.Count.Should().Be(0);
        }

        [Test]
        public async Task StepperPreventStepChangeDirectionIsForwardWhenCompletingStepOneOfOneTest()
        {
            // Arrange
            var lastStepChangeDirection = StepChangeDirection.None;
            var targetIndex = -1;

            var stepper = Context.Render<MudStepperExtended>(parameters => parameters
                .Add(p => p.PreventStepChangeAsync,
                    new Func<StepChangeDirection, int, Task<bool>>((direction, index) =>
                    {
                        lastStepChangeDirection = direction;
                        targetIndex = index;
                        return Task.FromResult(false);
                    }))
            );

            Context.Render<MudStepExtended>(parameters => parameters
                .AddCascadingValue(stepper.Instance)
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
            var targetIndex = -1;

            var stepper = Context.Render<MudStepperExtended>(parameters => parameters
                .Add(p => p.PreventStepChangeAsync,
                    new Func<StepChangeDirection, int, Task<bool>>((direction, index) =>
                    {
                        lastStepChangeDirection = direction;
                        targetIndex = index;
                        return Task.FromResult(false);
                    }))
            );

            Context.Render<MudStepExtended>(parameters => parameters
                .AddCascadingValue(stepper.Instance)
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
            var capturedTargetIndex = -1;

            var stepper = Context.Render<MudStepperExtended>(parameters => parameters
                .Add(p => p.PreventStepChangeAsync,
                    new Func<StepChangeDirection, int, Task<bool>>((direction, targetIndex) =>
                    {
                        lastStepChangeDirection = direction;
                        capturedTargetIndex = targetIndex;
                        return Task.FromResult(false);
                    }))
            );

            var step0 = Context.Render<MudStepExtended>(p => p
                .AddCascadingValue(stepper.Instance)
            );

            var step1 = Context.Render<MudStepExtended>(p => p
                .AddCascadingValue(stepper.Instance)
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
            var capturedTargetIndex = -1;

            var stepper = Context.Render<MudStepperExtended>(parameters => parameters
                .Add(p => p.PreventStepChangeAsync,
                    new Func<StepChangeDirection, int, Task<bool>>((direction, targetIndex) =>
                    {
                        lastStepChangeDirection = direction;
                        capturedTargetIndex = targetIndex;
                        return Task.FromResult(false);
                    }))
            );

            var step0 = Context.Render<MudStepExtended>(p => p
                .AddCascadingValue(stepper.Instance)
            );

            var step1 = Context.Render<MudStepExtended>(p => p
                .AddCascadingValue(stepper.Instance)
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

            var stepper = Context.Render<MudStepperExtended>(parameters => parameters
                .Add(p => p.PreventStepChangeAsync,
                    new Func<StepChangeDirection, int, Task<bool>>((direction, targetIndex) =>
                    {
                        lastStepChangeDirection = direction;
                        return Task.FromResult(false);
                    }))
            );

            var step0 = Context.Render<MudStepExtended>(p => p
                .AddCascadingValue(stepper.Instance)
            );

            await stepper.Instance.GoToStepByReferenceAsync(step0.Instance);
            await stepper.Instance.GoToStepAsync(0);

            lastStepChangeDirection.Should().Be(StepChangeDirection.None);
        }

        [Test]
        public async Task StepperPreventStepChangeIsInvokedWhenCompletingActiveStepTest()
        {
            var preventStepChangeWasInvoked = false;

            var stepper = Context.Render<MudStepperExtended>(parameters => parameters
                .Add(p => p.PreventStepChangeAsync,
                    new Func<StepChangeDirection, int, Task<bool>>((direction, targetIndex) =>
                    {
                        preventStepChangeWasInvoked = true;
                        return Task.FromResult(false);
                    }))
            );

            var step0 = Context.Render<MudStepExtended>(p => p
                .AddCascadingValue(stepper.Instance)
            );

            var step1 = Context.Render<MudStepExtended>(p => p
                .AddCascadingValue(stepper.Instance)
            );

            await stepper.Instance.GoToStepByReferenceAsync(step0.Instance);
            await stepper.Instance.CompleteStep(stepper.Instance.Steps.IndexOf(step0.Instance));

            preventStepChangeWasInvoked.Should().BeTrue();
        }

        [Test]
        public async Task StepperPreventStepChangeIsInvokedWhenSkippingActiveStepTest()
        {
            var preventStepChangeWasInvoked = false;

            var stepper = Context.Render<MudStepperExtended>(parameters => parameters
                .Add(p => p.PreventStepChangeAsync,
                    new Func<StepChangeDirection, int, Task<bool>>((direction, targetIndex) =>
                    {
                        preventStepChangeWasInvoked = true;
                        return Task.FromResult(false);
                    }))
            );

            var step0 = Context.Render<MudStepExtended>(p => p
                .AddCascadingValue(stepper.Instance)
            );

            var step1 = Context.Render<MudStepExtended>(p => p
                .AddCascadingValue(stepper.Instance)
            );

            await stepper.Instance.GoToStepByReferenceAsync(step0.Instance);
            await stepper.Instance.SkipStep(stepper.Instance.Steps.IndexOf(step0.Instance));

            preventStepChangeWasInvoked.Should().BeTrue();
        }

        [Test]
        public async Task StepperPreventStepChangeIsNotInvokedWhenCompletingNonActiveStepTest()
        {
            var preventStepChangeWasInvoked = false;

            var stepper = Context.Render<MudStepperExtended>(parameters => parameters
                .Add(p => p.PreventStepChangeAsync,
                    new Func<StepChangeDirection, int, Task<bool>>((direction, targetIndex) =>
                    {
                        preventStepChangeWasInvoked = true;
                        return Task.FromResult(false);
                    }))
            );

            var step0 = Context.Render<MudStepExtended>(p => p
                .AddCascadingValue(stepper.Instance)
                .Add(s => s.Order, 0)
            );

            var step1 = Context.Render<MudStepExtended>(p => p
                .AddCascadingValue(stepper.Instance)
                .Add(s => s.Order, 1)
            );

            // step 0'ı aktif yap (prevent'i bilinçli bypass ediyoruz)
            await stepper.Instance.GoToStepAsync(0, skipPrevent: true);

            // aktif olmayan step (step1) complete ediliyor
            await stepper.Instance.CompleteStep(1);

            preventStepChangeWasInvoked.Should().BeFalse();
        }

        [Test]
        public async Task StepperPreventStepChangeIsNotInvokedWhenSkippingNonActiveStepTest()
        {
            var preventStepChangeWasInvoked = false;

            var stepper = Context.Render<MudStepperExtended>(parameters => parameters
                .Add(p => p.PreventStepChangeAsync,
                    new Func<StepChangeDirection, int, Task<bool>>((direction, targetIndex) =>
                    {
                        preventStepChangeWasInvoked = true;
                        return Task.FromResult(false);
                    }))
            );

            var step0 = Context.Render<MudStepExtended>(p => p
                .AddCascadingValue(stepper.Instance)
                .Add(s => s.Order, 0)
            );

            var step1 = Context.Render<MudStepExtended>(p => p
                .AddCascadingValue(stepper.Instance)
                .Add(s => s.Order, 1)
            );

            // Aktif step'i ayarla (prevent'i bypass ederek)
            await stepper.Instance.GoToStepAsync(0, skipPrevent: true);

            var indexOfStep1 = stepper.Instance.Steps.IndexOf(step1.Instance);
            await stepper.Instance.SkipStep(indexOfStep1);

            preventStepChangeWasInvoked.Should().BeFalse();
        }

        [Test]
        public async Task StepperActiveIndexIsNotChangedWhenCompletingNonActiveStepTest()
        {
            var stepper = Context.Render<MudStepperExtended>();

            var step0 = Context.Render<MudStepExtended>(p => p
                .AddCascadingValue(stepper.Instance)
            );

            var step1 = Context.Render<MudStepExtended>(p => p
                .AddCascadingValue(stepper.Instance)
            );

            await stepper.Instance.GoToStepByReferenceAsync(step0.Instance, skipPrevent: true);

            var activeBefore = stepper.Instance.ActiveIndex;

            var step1Index = stepper.Instance.Steps.IndexOf(step1.Instance);
            await stepper.Instance.CompleteStep(step1Index, moveToNextStep: true);

            stepper.Instance.ActiveIndex.Should().Be(activeBefore);
        }

        [Test]
        public async Task StepperActiveIndexIsNotChangedWhenSkippingNonActiveStepTest()
        {
            var stepper = Context.Render<MudStepperExtended>();

            var step0 = Context.Render<MudStepExtended>(p => p
                .AddCascadingValue(stepper.Instance)
            );

            var step1 = Context.Render<MudStepExtended>(p => p
                .AddCascadingValue(stepper.Instance)
            );

            await stepper.Instance.GoToStepByReferenceAsync(step0.Instance, skipPrevent: true);

            var activeBefore = stepper.Instance.ActiveIndex;
            var step1Index = stepper.Instance.Steps.IndexOf(step1.Instance);

            await stepper.Instance.SkipStep(step1Index, moveToNextStep: true);

            stepper.Instance.ActiveIndex.Should().Be(activeBefore);
        }

        [Test]
        public async Task StepperCheckChangeCountTest()
        {
            var comp = Context.Render<StepperTest1>();
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
            var stepper = Context.Render<MudStepperExtended>(p => p
                .Add(s => s.PreventStepChangeAsync,
                    new Func<StepChangeDirection, int, Task<bool>>((_, __) => Task.FromResult(true)))
            );

            var step0 = Context.Render<MudStepExtended>(p => p
                .AddCascadingValue(stepper.Instance)
            );

            var step1 = Context.Render<MudStepExtended>(p => p
                .AddCascadingValue(stepper.Instance)
            );

            await stepper.Instance.GoToStepAsync(0);
            await stepper.Instance.GoToStepAsync(1);

            stepper.Instance.ActiveIndex
                .Should()
                .Be(stepper.Instance.Steps.IndexOf(step0.Instance));
        }

        [Test]
        public async Task StepperGoNextIsBlockedWhenPreventReturnsTrueTest()
        {
            var stepper = Context.Render<MudStepperExtended>(p => p
                .Add(s => s.PreventStepChangeAsync,
                    new Func<StepChangeDirection, int, Task<bool>>((_, __) => Task.FromResult(true)))
            );

            var step0 = Context.Render<MudStepExtended>(p => p
                .AddCascadingValue(stepper.Instance)
            );

            var step1 = Context.Render<MudStepExtended>(p => p
                .AddCascadingValue(stepper.Instance)
            );

            await stepper.Instance.GoToStepAsync(0);
            await stepper.Instance.GoNextStepAsync();

            stepper.Instance.ActiveIndex.Should().Be(0);
        }

        [Test]
        public async Task StepperNavigatesToResultStepWhenAllStepsCompletedTest()
        {
            var stepper = Context.Render<MudStepperExtended>();

            var step0 = Context.Render<MudStepExtended>(p => p
                .AddCascadingValue(stepper.Instance)
            );

            var step1 = Context.Render<MudStepExtended>(p => p
                .AddCascadingValue(stepper.Instance)
            );

            var resultStep = Context.Render<MudStepExtended>(p => p
                .AddCascadingValue(stepper.Instance)
                .Add(s => s.IsResultStep, true)
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

            var stepper = Context.Render<MudStepperExtended>(p => p
                .Add(s => s.BeforeFinishedAsync,
                    new Func<Task<bool>>(() =>
                    {
                        beforeCalled = true;
                        return Task.FromResult(true);
                    }))
            );

            var step0 = Context.Render<MudStepExtended>(p => p
                .AddCascadingValue(stepper.Instance)
            );

            var step1 = Context.Render<MudStepExtended>(p => p
                .AddCascadingValue(stepper.Instance)
            );

            await stepper.Instance.GoToStepAsync(0);
            await stepper.Instance.CompleteStep(0);
            await stepper.Instance.CompleteStep(1);

            beforeCalled.Should().BeTrue();
        }

        [Test]
        public async Task StepperCallsOnFinishedWhenCompletingLastRemainingStepTest()
        {
            bool finishedCalled = false;

            var stepper = Context.Render<MudStepperExtended>(p => p
                .Add(s => s.OnFinished,
                    EventCallback.Factory.Create(this, () => finishedCalled = true))
            );

            var step0 = Context.Render<MudStepExtended>(p => p
                .AddCascadingValue(stepper.Instance)
            );

            var step1 = Context.Render<MudStepExtended>(p => p
                .AddCascadingValue(stepper.Instance)
            );

            await stepper.Instance.GoToStepAsync(0);
            await stepper.Instance.CompleteStep(0);
            await stepper.Instance.CompleteStep(1);

            finishedCalled.Should().BeTrue();
        }

        [Test]
        public async Task StepperGoByIndexComputesDirectionCorrectlyTest()
        {
            StepChangeDirection? captured = null;

            var stepper = Context.Render<MudStepperExtended>(p => p
                .Add(s => s.PreventStepChangeAsync,
                    new Func<StepChangeDirection, int, Task<bool>>((dir, _) =>
                    {
                        captured = dir;
                        return Task.FromResult(false);
                    }))
            );

            var step0 = Context.Render<MudStepExtended>(p => p
                .AddCascadingValue(stepper.Instance)
            );

            var step1 = Context.Render<MudStepExtended>(p => p
                .AddCascadingValue(stepper.Instance)
            );

            await stepper.Instance.GoToStepAsync(0);
            await stepper.Instance.GoByIndexAsync(1);

            captured.Should().Be(StepChangeDirection.Forward);
        }

        [Test]
        public async Task StepperSetsStatusContinuedWhenNavigatingToUnstartedStepTest()
        {
            var stepper = Context.Render<MudStepperExtended>();

            var step0 = Context.Render<MudStepExtended>(p => p
                .AddCascadingValue(stepper.Instance)
            );

            var step1 = Context.Render<MudStepExtended>(p => p
                .AddCascadingValue(stepper.Instance)
            );

            await stepper.Instance.GoToStepAsync(0);
            await stepper.Instance.GoToStepAsync(1);

            step1.Instance.Status.Should().Be(StepStatus.Continued);
        }

        [Test]
        public async Task StepperGoesToIntroStepWhenIntroExists()
        {
            var stepper = Context.Render<MudStepperExtended>();

            var intro = Context.Render<MudStepExtended>(p => p
                .AddCascadingValue(stepper.Instance)
                .Add(s => s.IsIntroStep, true)
            );

            var step0 = Context.Render<MudStepExtended>(p => p
                .AddCascadingValue(stepper.Instance)
            );

            await stepper.Instance.GoToStepAsync(-1);

            stepper.Instance.ActiveIndex.Should().Be(-1);
        }

        [Test]
        public async Task StepperNegativeIndexGoesToZeroWhenNoIntroStepExists()
        {
            var stepper = Context.Render<MudStepperExtended>();

            var step0 = Context.Render<MudStepExtended>(p => p
                .AddCascadingValue(stepper.Instance)
            );

            await stepper.Instance.GoToStepAsync(-1);

            stepper.Instance.ActiveIndex.Should().Be(0);
        }

        [Test]
        public async Task StepperResetReturnsToIntroStep()
        {
            var stepper = Context.Render<MudStepperExtended>();

            var intro = Context.Render<MudStepExtended>(p => p
                .AddCascadingValue(stepper.Instance)
                .Add(s => s.IsIntroStep, true)
            );

            var step0 = Context.Render<MudStepExtended>(p => p
                .AddCascadingValue(stepper.Instance)
            );

            await stepper.Instance.GoToStepAsync(0);
            stepper.Instance.Reset();

            stepper.Instance.ActiveIndex.Should().Be(-1);
        }

        [Test]
        public async Task StepperNextFromIntroGoesToFirstStep()
        {
            var stepper = Context.Render<MudStepperExtended>();

            var intro = Context.Render<MudStepExtended>(p => p
                .AddCascadingValue(stepper.Instance)
                .Add(s => s.IsIntroStep, true)
            );

            var step0 = Context.Render<MudStepExtended>(p => p
                .AddCascadingValue(stepper.Instance)
            );

            await stepper.Instance.GoToStepAsync(-1);
            await stepper.Instance.GoNextStepAsync(true);

            stepper.Instance.ActiveIndex.Should().Be(0);
        }

        [Test]
        public async Task StepperPreviousFromFirstStepGoesToIntroWhenExists()
        {
            var stepper = Context.Render<MudStepperExtended>();

            var intro = Context.Render<MudStepExtended>(p => p
                .AddCascadingValue(stepper.Instance)
                .Add(s => s.IsIntroStep, true)
            );

            var step0 = Context.Render<MudStepExtended>(p => p
                .AddCascadingValue(stepper.Instance)
            );

            await stepper.Instance.GoToStepAsync(0);
            await stepper.Instance.GoPreviousStepAsync(true);

            stepper.Instance.ActiveIndex.Should().Be(-1);
        }

        [Test]
        public async Task StepperCannotSkipIntroStep()
        {
            var stepper = Context.Render<MudStepperExtended>();

            var intro = Context.Render<MudStepExtended>(p => p
                .AddCascadingValue(stepper.Instance)
                .Add(s => s.IsIntroStep, true)
            );

            var step0 = Context.Render<MudStepExtended>(p => p
                .AddCascadingValue(stepper.Instance)
            );

            await stepper.Instance.GoToStepAsync(-1);

            await stepper.Instance.SkipStep(stepper.Instance.Steps.IndexOf(intro.Instance));

            stepper.Instance.ActiveIndex.Should().Be(-1);
        }

        [Test]
        public async Task StepperIntroThenResultStepFlowIsCorrect()
        {
            var stepper = Context.Render<MudStepperExtended>();

            var intro = Context.Render<MudStepExtended>(p => p
                .AddCascadingValue(stepper.Instance)
                .Add(s => s.IsIntroStep, true)
            );

            var step0 = Context.Render<MudStepExtended>(p => p
                .AddCascadingValue(stepper.Instance)
            );

            var result = Context.Render<MudStepExtended>(p => p
                .AddCascadingValue(stepper.Instance)
                .Add(s => s.IsResultStep, true)
            );

            await stepper.Instance.GoToStepAsync(-1);
            await stepper.Instance.GoNextStepAsync(true);
            await stepper.Instance.CompleteStep(
                stepper.Instance.Steps.IndexOf(step0.Instance),
                moveToNextStep: true
            );

            stepper.Instance.ActiveIndex.Should().Be(1);
        }

    }
}
