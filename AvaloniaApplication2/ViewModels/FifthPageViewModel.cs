using ReactiveUI;
using System;

namespace AvaloniaApplication2.ViewModels
{
    public class FifthPageViewModel : PageViewModelBase        
    {
        public void OnButtonClicked()
        {
            CanNavigateNext = true;
        }

        private bool _CanNavigateNext;
        public override bool CanNavigateNext
        {
            get { return true; }
            set { this.RaiseAndSetIfChanged(ref _CanNavigateNext, value); }
        }

        private bool _CanNavigatePrevious;
        public override bool CanNavigatePrevious
        {
            get => true;
            protected set => throw new NotSupportedException();
        }
    }
}
