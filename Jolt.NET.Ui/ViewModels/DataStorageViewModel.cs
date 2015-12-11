using System.Collections.Generic;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using Jolt.NET.Data;
using Jolt.NET.Data.DataStorage;
using Jolt.NET.Network;
using Jolt.NET.UI.Common;

namespace Jolt.NET.UI.ViewModels
{
    public class DataStorageViewModel : ViewModelBase
    {
        #region Properties

        private IDialogService DialogService { get; set; }
        private GameClient Client { get; set; }
        private DataStorageClient DataStorageClient { get; set; }

        public User User
        {
            get { return Core.Settings.Instance.CurrentUser; }
        }

        private ObservableCollection<KeyValuePair<DataStorageKey, string>> _dataStorage;
        public ObservableCollection<KeyValuePair<DataStorageKey, string>> DataStorage
        {
            get { return _dataStorage; }
            set { Set(() => DataStorage, ref _dataStorage, value); }
        }

        private string _newDataStorageKey;
        public string NewDataStorageKey
        {
            get { return _newDataStorageKey; }
            set { Set(() => NewDataStorageKey, ref _newDataStorageKey, value, true); }
        }

        private string _newDataStorageData;
        public string NewDataStorageData
        {
            get { return _newDataStorageData; }
            set { Set(() => NewDataStorageData, ref _newDataStorageData, value, true); }
        }

        #endregion

        public DataStorageViewModel(IDialogService dialogService, GameClient client, DataStorageClient dataStorageClient)
        {
            Client = client;
            Client.AuthenticationCompleted += OnAuthenticationCompleted;

            DataStorageClient = dataStorageClient;
            DialogService = dialogService;

            FetchDataStorageCommand = new AutoRelayCommand(FetchDataStorage, CanFetchDataStorage);
            FetchDataStorageCommand.DependsOn(() => User);
            FetchDataCommand = new RelayCommand<DataStorageKey>(FetchData);
            AddNewDataStorageCommand = new AutoRelayCommand(AddNewDataStorage, CanAddNewDataStorage);
            AddNewDataStorageCommand.DependsOn(() => NewDataStorageKey);
            AddNewDataStorageCommand.DependsOn(() => NewDataStorageData);
        }

        #region Methods
        private async void ShowResultOnError(SuccessResponse response)
        {
            if (!response.Success)
            {
                await DialogService.ShowError(response.Message, "Error in web api request", "", null);
            }
        }

        private void OnAuthenticationCompleted(object sender, ResponseEventArgs e)
        {
            RaisePropertyChanged(() => User, default(User), default(User), true);
        }

        #endregion
        
        #region Commands

        public AutoRelayCommand FetchDataStorageCommand { get; set; }
        private async void FetchDataStorage()
        {
            var data = await DataStorageClient.GetDataStorageKeys();
            DataStorage = new ObservableCollection<KeyValuePair<DataStorageKey, string>>();
            foreach (var item in data)
            {
                DataStorage.Add(new KeyValuePair<DataStorageKey, string>(item, string.Empty));
            }
        }
        private bool CanFetchDataStorage()
        {
            return User != null
                && User.IsAuthenticated;
        }

        public RelayCommand<DataStorageKey> FetchDataCommand { get; set; }
        private async void FetchData(DataStorageKey key)
        {
            var data = new StringDataStoreEntry();
            data.Key = key.Key;
            await data.FetchDataStorageEntry();

            for (int i = 0; i < DataStorage.Count; i++)
            {
                if (DataStorage[i].Key == key)
                    DataStorage[i] = new KeyValuePair<DataStorageKey, string>(key, data.Data);
            }
        }

        public AutoRelayCommand AddNewDataStorageCommand { get; set; }
        private async void AddNewDataStorage()
        {
            var dataStorageEntry = new StringDataStoreEntry();
            dataStorageEntry.Key = NewDataStorageKey;
            ShowResultOnError(await dataStorageEntry.SetDataStorageEntry(NewDataStorageData));

            bool dataSet = false;
            var entry = new KeyValuePair<DataStorageKey, string>(
                new DataStorageKey() { Key = NewDataStorageKey },
                NewDataStorageData);

            for (int i = 0; i < DataStorage.Count; i++)
            {
                if (DataStorage[i].Key.Key == entry.Key.Key)
                {
                    DataStorage[i] = entry;
                    dataSet = true;
                }
            }

            if (!dataSet)
                DataStorage.Add(entry);

            NewDataStorageKey = "";
            NewDataStorageData = "";
        }
        private bool CanAddNewDataStorage()
        {
            return !string.IsNullOrEmpty(NewDataStorageKey)
                && !string.IsNullOrEmpty(NewDataStorageData);
        }

        #endregion
    }
}
