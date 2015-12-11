using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using Jolt.NET.UI.Services;
using Microsoft.Practices.ServiceLocation;

namespace Jolt.NET.UI.ViewModels
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            if (ViewModelBase.IsInDesignModeStatic)
            { }
            else
            {
                SimpleIoc.Default.Register<IDialogService, DialogService>();
                SimpleIoc.Default.Register(() => new GameClient());
                SimpleIoc.Default.Register<SessionManager>();
                SimpleIoc.Default.Register<TrophyClient>();
                SimpleIoc.Default.Register<DataStorageClient>();
                SimpleIoc.Default.Register<ScoreClient>();
            }

            // ViewModels
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<GeneralViewModel>();
            SimpleIoc.Default.Register<UserViewModel>();
            SimpleIoc.Default.Register<TrophyViewModel>();
            SimpleIoc.Default.Register<DataStorageViewModel>();
            SimpleIoc.Default.Register<ScoreViewModel>();
        }

        public MainViewModel Main
        {
            get { return ServiceLocator.Current.GetInstance<MainViewModel>(); }
        }

        public GeneralViewModel General
        {
            get { return ServiceLocator.Current.GetInstance<GeneralViewModel>(); }
        }

        public UserViewModel User
        {
            get { return ServiceLocator.Current.GetInstance<UserViewModel>(); }
        }

        public TrophyViewModel Trophy
        {
            get { return ServiceLocator.Current.GetInstance<TrophyViewModel>(); }
        }

        public DataStorageViewModel DataStorage
        {
            get { return ServiceLocator.Current.GetInstance<DataStorageViewModel>(); }
        }

        public ScoreViewModel Score
        {
            get { return ServiceLocator.Current.GetInstance<ScoreViewModel>(); }
        }

        public static void Cleanup()
        {
            SimpleIoc.Default.GetInstance<ScoreViewModel>()?.Cleanup();
            SimpleIoc.Default.GetInstance<DataStorageViewModel>()?.Cleanup();
            SimpleIoc.Default.GetInstance<TrophyViewModel>()?.Cleanup();
            SimpleIoc.Default.GetInstance<UserViewModel>()?.Cleanup();
            SimpleIoc.Default.GetInstance<GeneralViewModel>()?.Cleanup();           
            SimpleIoc.Default.GetInstance<MainViewModel>()?.Cleanup();
        }
    }
}