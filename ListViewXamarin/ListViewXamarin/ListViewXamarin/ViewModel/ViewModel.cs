using Newtonsoft.Json;
using Syncfusion.ListView.XForms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ListViewXamarin
{
    public class ListViewModel : INotifyPropertyChanged
    {
        #region Fields

        private IList<dynamic> items;
        private bool isDownloaded;
        private int totalItems = 22;
        #endregion

        #region Properties

        public IList<dynamic> Items
        {
            get { return items; }
            set
            {
                items = value;
                OnPropertyChanged("Items");
            }
        }

        public IList<dynamic> JSONCollection { get; set; }

        public DataServices DataService { get; set; }

        public Command<object> LoadMoreItemsCommand { get; set; }

        private bool isBusy;
        public bool IsBusy
        {
            get { return isBusy; }
            set
            {
                isBusy = value;
                OnPropertyChanged("IsBusy");
            }
        }

        #endregion

        #region Constructor

        public ListViewModel()
        {
            JSONCollection = new ObservableCollection<dynamic>();
            Items = new ObservableCollection<dynamic>();
            DataService = new DataServices();            
            GetDataAsync();
            LoadMoreItemsCommand = new Command<object>(LoadMoreItems, CanLoadMoreItems);

        }
        #endregion

        #region Methods
        private async void GetDataAsync()
        {
            isDownloaded = await DataService.DownloadJsonAsync();
            if (isDownloaded)
            {
                var localFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                var fileText = File.ReadAllText(Path.Combine(localFolder, "Data.json"));

                //Read data from the local path and set it to the collection bound to the ListView.
                JSONCollection = JsonConvert.DeserializeObject<IList<dynamic>>(fileText);
                totalItems = JSONCollection.Count;
            }
        }

        private bool CanLoadMoreItems(object obj)
        {
            if (Items.Count >= totalItems)
                return false;
            return true;
        }

        private async void LoadMoreItems(object obj)
        {
            var listview = obj as SfListView;
            try
            {
                IsBusy = true;
                await Task.Delay(1000);
                var index = Items.Count;
                var count = index + 3 >= totalItems ? totalItems - index : 3;
                AddProducts(index, count);
            }
            catch
            {

            }
            finally
            {
                IsBusy = false;
            }
        }

        private void AddProducts(int index, int count)
        {
            for (int i = index; i < index + count; i++)
            {
                Items.Add(JSONCollection[i]);
            }
        }
        #endregion

        #region Interface Member

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }
}
