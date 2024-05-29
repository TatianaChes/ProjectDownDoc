using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaApplication2.ViewModels
{
    public class FourthPageViewModel : PageViewModelBase
    {
        public FourthPageViewModel()
        {

        }

        private bool _CanNavigatePrevious;
        public override bool CanNavigatePrevious
        {
            get => true;
            protected set => throw new NotSupportedException();
        }

        private bool _CanNavigateNext;

        public override bool CanNavigateNext
        {
            get { return false; }
            set { this.RaiseAndSetIfChanged(ref _CanNavigateNext, value); }
        }
    }
}
