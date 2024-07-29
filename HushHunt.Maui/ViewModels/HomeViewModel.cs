using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HushHunt.Maui.Views;
using System.Windows.Input;
namespace HushHunt.Maui.ViewModels
{
    public class HomeViewModel:BaseViewModel
    {
        public ICommand StartButtonCommand { get; set; }

        public HomeViewModel()
        {
            StartButtonCommand = new Command(OnNavigateToGamePage);
        }

        private async void OnNavigateToGamePage()
        {
            await Shell.Current.GoToAsync(nameof(GamePage));
        }
    }
}
