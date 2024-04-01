using MonkeyFinder.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MonkeyFinder.ViewModel;
using MonkeyFinder.Services;
using System.Diagnostics;

using CommunityToolkit.Mvvm.Input;

namespace MonkeyFinder.ViewModel
{
    public partial class MonkeysViewModel : BaseViewModel
    {
        public ObservableCollection<Monkey> Monkeys { get; } = new();

        //public Command GetMonkeysCommand { get; }

        MonkeyService monkeyService;

        public MonkeysViewModel(MonkeyService monkeyService)
        {
            Title = "Monkey Finder";
            this.monkeyService = monkeyService;

            // Do not need this as using the RelayCommandAttribute from the community toolkit
            //GetMonkeysCommand = new Command(async () => await GetMonkeysAsync());
            
            
        }

        [RelayCommand]
        //[RelayCommandAttribute]
        async Task GetMonkeysAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                var monkeys = await monkeyService.GetMonkeys();

                if (Monkeys.Count != 0)
                {
                    Monkeys.Clear();
                }

                foreach (var monkey in monkeys)
                {
                    Monkeys.Add(monkey);
                }
            }
            catch (Exception ex) 
            {
                Debug.WriteLine(ex);
                await Shell.Current.DisplayAlert ("Error getting monkeys", ex.Message, "OK" );
            }
            finally 
            {
                IsBusy = false;
            }
        }
    }
}
