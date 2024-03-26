using Microsoft.Maui.ApplicationModel.DataTransfer;
using Microsoft.Maui.Devices.Sensors;

namespace fsn_maui_FindMe;

public partial class MainPage : ContentPage
{
    readonly string _baseUrl = "https://bing.com/maps/default.aspx?cp=";

    public string UserName { get; set; } = "";

    private async Task ShareLocation()
    {
        UserName = UsernameEntry.Text;

        var locationRequest = new GeolocationRequest(GeolocationAccuracy.Best);

        var location = await Geolocation.GetLocationAsync(locationRequest);

        // note: app crashes when multiple share request are done.
        // fix: disable FindMe Button or Command (when using mvvm) while awaiting previous request
        await Share.RequestAsync(new ShareTextRequest
        {
            Subject = "Find me!",
            Title = "Find me!",
            Text = $"{UserName} is sharing their location with you",
            Uri = $"{_baseUrl}{location?.Latitude}~{location?.Longitude}"
        });
    }


    public MainPage()
    {
        InitializeComponent();
    }

    private async void OnFindMeClicked(object sender, EventArgs e)
    {
        var permissions = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

        if (permissions == PermissionStatus.Granted)
        {
            await ShareLocation();
        }
        else
        {
            await App.Current.MainPage.DisplayAlert("Permissions Error", "You have not granted the app permission to access your location.", "OK");
    

            var requested = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

            if (requested == PermissionStatus.Granted)
            {
                await ShareLocation();
            }
            else
            {
                if (DeviceInfo.Platform == DevicePlatform.iOS ||
                    DeviceInfo.Platform == DevicePlatform.MacCatalyst)
                {
                    await App.Current.MainPage.DisplayAlert("Location Required", "Location is required to share it. Please enable location for this app in Settings.", "OK");
               }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Location Required", "Location is required to share it. We'll ask again next time.", "OK");
                }
            }
        }
    }

}
