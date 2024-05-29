using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaApplication2.ViewModels
{
    public abstract class PageViewModelBase : ViewModelBase
    {
      public abstract bool CanNavigateNext { get; set; }
      public abstract bool CanNavigatePrevious { get; protected set; }
    }
}
