using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jolt.NET.Data;
using Jolt.NET.Data.DataStorage;
using Jolt.NET.Network;

namespace Jolt.NET.Interfaces
{
    /// <summary>
    /// A base class of the data store entry without generic parameter.
    /// </summary>
    public interface IDataStoreEntry : INotifyPropertyChanged
    {
        #region Properties
        
        object Data { get; }
        string Key { get; set; }
        User User { get; }

        #endregion

        #region Events
        
        event EventHandler<ResponseEventArgs> RemoveDataStorageCompleted;
        event EventHandler<ResponseEventArgs> FetchDataStorageCompleted;

        #endregion

        #region Methods
        
        Task<SuccessResponse> RemoveDataStorageEntry(User user = null);
        Task<object> FetchDataStorageEntry(User user = null);

        #endregion
    }
    
    public interface IGenericDataStoreEntry<T> : IDataStoreEntry
    {
        new T Data { get; }

        #region Events
        
        event EventHandler<ResponseEventArgs> SetDataStorageCompleted;
        event EventHandler<ResponseEventArgs> UpdateDataStorageCompleted;

        #endregion

        #region Methods
        
        Task<SuccessResponse> SetDataStorageEntry(T data, User user = null);
        Task<UpdateDataStorageResponse> UpdateDataStorageEntry(
            T value, DataStorageUpdateOperation updateOperation, User user = null);
        new Task<T> FetchDataStorageEntry(User user = null);

        #endregion
    }
}
