using Autofac;
using SimpleAudio.ViewModels;

namespace SimpleAudio
{
    public class ViewModelLocator
    {
        #region IsDesignMode property hack

        private System.Windows.DependencyObject dummy = new System.Windows.DependencyObject();
        private bool IsDesignMode => System.ComponentModel.DesignerProperties.GetIsInDesignMode(dummy);

        #endregion

        private static IContainer _container;
        private static IContainer Container => _container ?? (_container = CreateContainer());
        private static IContainer CreateContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<StatusViewModel>().SingleInstance();
            builder.RegisterType<SearchViewModel>().SingleInstance();

            return builder.Build();
        }

        public SearchViewModel SearchViewModel
        {
            get { return IsDesignMode ? CreateDesignSearchViewModel() : Container.Resolve<SearchViewModel>(); }
        }
        public StatusViewModel StatusViewModel
        {
            get { return IsDesignMode ? CreateDesignStatusViewModel() : Container.Resolve<StatusViewModel>(); }
        }

        private SearchViewModel CreateDesignSearchViewModel()
        {
            var vm = new SearchViewModel();

            return vm;
        }
        private StatusViewModel CreateDesignStatusViewModel()
        {
            var vm = new StatusViewModel();

            return vm;
        }
    }
}
