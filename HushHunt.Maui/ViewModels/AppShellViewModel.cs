using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HushHunt.Maui.ViewModels;
using HushHunt.Maui.Views;
using System.Threading.Tasks;

public partial class AppShellViewModel : BaseViewModel
{
    
    [RelayCommand]
    public async Task OpenInfoAsync()
    {
         await Shell.Current.GoToAsync(nameof(InfoPage));
    }
    [RelayCommand]
    public async Task BackToHomeAsync()
    {
        await Shell.Current.GoToAsync("//HomePage");
    }

}
