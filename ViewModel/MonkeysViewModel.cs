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
using MonkeyFinder.View;

namespace MonkeyFinder.ViewModel
{
    public partial class MonkeysViewModel : BaseViewModel
    {
        public ObservableCollection<Monkey> Monkeys { get; } = new();

        //public Command GetMonkeysCommand { get; }

        MonkeyService monkeyService;

        IConnectivity connectivity;
        IGeolocation geolocation;

        public MonkeysViewModel(MonkeyService monkeyService, IConnectivity connectivity, IGeolocation geolocation)
        {
            Title = "Monkey Finder";
            this.monkeyService = monkeyService;

            this.connectivity = connectivity;
            this.geolocation = geolocation;

            // Do not need this as using the RelayCommandAttribute from the community toolkit
            //GetMonkeysCommand = new Command(async () => await GetMonkeysAsync());
            
            
        }

        [RelayCommand]
        async Task GetClosestMonkeyAsync()
        {
            if (IsBusy || Monkeys.Count == 0) return;

            try
            {
                var location = await geolocation.GetLastKnownLocationAsync();
                if (location == null)
                {
                    location = await geolocation.GetLocationAsync(
                        new GeolocationRequest
                        {
                            DesiredAccuracy = GeolocationAccuracy.Medium,
                            Timeout = TimeSpan.FromSeconds(30),
                        });
                }

                if (location is null) { return; }

                var first = Monkeys.OrderBy(m => location.CalculateDistance(m.Latitude, m.Longitude, DistanceUnits.Miles)).FirstOrDefault();

                if (first is null) return;

                await Shell.Current.DisplayAlert("Closest Monkey", $"{first.Name} in {first.Location}", "OK");

            }
            catch (Exception ex) 
            {
                Debug.WriteLine(ex);
                await Shell.Current.DisplayAlert("Error!", "Unable to get closest monkey: {ex.Message}", "OK");
            }

        }

        [RelayCommand]
        async Task GoToDetailsAsync (Monkey monkey)
        {
            if (monkey is null) return;

            //await Shell.Current.GoToAsync($"{nameof(DetailsPage)}?id={monkey.Name}");
            await Shell.Current.GoToAsync($"{nameof(DetailsPage)}", true, 
                new Dictionary<string, object>
                {
                    {"Monkey", monkey}
                });
        }


        [RelayCommand]
        //[RelayCommandAttribute]
        async Task GetMonkeysAsync()
        {
            if (IsBusy) return;

            try
            {
                if (connectivity.NetworkAccess != NetworkAccess.Internet)
                {
                    await Shell.Current.DisplayAlert("No internet", "Check your internet and try again!", "OK");
                    return;
                }

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
