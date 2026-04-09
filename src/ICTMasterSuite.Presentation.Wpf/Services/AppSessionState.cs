using CommunityToolkit.Mvvm.ComponentModel;

namespace ICTMasterSuite.Presentation.Wpf.Services;

public enum AuthenticationState
{
    Guest = 0,
    Authenticated = 1
}

public enum ConnectivityState
{
    Offline = 0,
    Online = 1
}

public sealed partial class AppSessionState : ObservableObject
{
    [ObservableProperty]
    private AuthenticationState authentication = AuthenticationState.Guest;

    [ObservableProperty]
    private ConnectivityState connectivity = ConnectivityState.Offline;

    public bool IsGuest => Authentication == AuthenticationState.Guest;

    public bool IsAuthenticated => Authentication == AuthenticationState.Authenticated;

    public bool IsOffline => Connectivity == ConnectivityState.Offline;

    public bool IsOnline => Connectivity == ConnectivityState.Online;

    public void SetAuthentication(AuthenticationState state)
    {
        Authentication = state;
        OnPropertyChanged(nameof(IsGuest));
        OnPropertyChanged(nameof(IsAuthenticated));
    }

    public void SetConnectivity(ConnectivityState state)
    {
        Connectivity = state;
        OnPropertyChanged(nameof(IsOffline));
        OnPropertyChanged(nameof(IsOnline));
    }
}
