using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using HushHunt.Maui.Models;

namespace HushHunt.Maui.ViewModels
{
    public class GameViewModel : BaseViewModel
    {
        private readonly Random _random = new Random();
        private GameModel _gameModel;

        public ObservableCollection<Image> MainGridImages { get; set; } = new ObservableCollection<Image>();
        public ObservableCollection<Image> FlexLayoutImages { get; set; } = new ObservableCollection<Image>();
        public ObservableCollection<Image> LevelIntroFlexImages { get; set; } = new ObservableCollection<Image>();

        public string StatusLabel { get; set; }
        public string BtnHintText { get; set; }
        public string LevelLabelText { get; set; }
        public string HintUpdateText { get; set; }
        public bool IsHintUpdateVisible { get; set; }
        public bool IsIntroVisible { get; set; } = true;

        public ICommand StartGameCommand { get; set; }
        public ICommand RestartGameCommand { get; set; }
        public ICommand HintCommand { get; set; }
        public ICommand IntroCommand { get; set; }


        public event EventHandler<string> TitleUpdated;
        public event EventHandler<(string title, string message, string cancel)> AlertRequested;

        public GameViewModel()
        {
            _gameModel = new GameModel();
            StartGameCommand = new Command(GenerateGame);
            RestartGameCommand = new Command(RestartGame);
            HintCommand = new Command(Hint);
            IntroCommand = new Command(HideIntro);
        }




        private void GenerateGame()
        {
            ResetGame();
            SetHintCount();
            GenerateBackground();
            var gameElement = SetGameElement(_gameModel.LevelCount);
            GenerateUniqueImages(gameElement.targets, true);
            GenerateUniqueImages(gameElement.barriers, false);
            StatusLabel = _gameModel.TargetItems.Count <= 1 ? $"{_gameModel.TargetItems.Count} item to seek" : $"{_gameModel.TargetItems.Count} items to seek";
            BtnHintText = (_gameModel.HintCount == 1) ? $"{_gameModel.HintCount} Hint" : $"{_gameModel.HintCount} Hints";
        }

        private void ResetGame()
        {
            MainGridImages.Clear();
            FlexLayoutImages.Clear();
            LevelIntroFlexImages.Clear();
            _gameModel.UsedImageNumbers.Clear();
            _gameModel.UsedPositions.Clear();
            UpdateTitleBasedOnLevel(_gameModel.LevelCount);
            LevelLabelText = $"Level {_gameModel.LevelCount}";
            SetHintUpdate(_gameModel.LevelCount);
        }

        private void GenerateBackground()
        {
            int backgroundIndex = ((_gameModel.LevelCount - 1) % 11) + 1;
            var background = new Image
            {
                Source = $"background{backgroundIndex}.png",
                Aspect = Aspect.AspectFill,
                Opacity = SetBackgroundOpacity(_gameModel.LevelCount)
            };
            MainGridImages.Add(background);
        }

        private double SetBackgroundOpacity(int levelCount)
        {
            if (levelCount <= 5 && levelCount > 1)
            {
                return 0.9;
            }
            else if (levelCount <= 10 && levelCount > 5)
            {
                return 0.95;
            }
            else
            {
                return 1;
            }
        }

        private void GenerateUniqueImages(int count, bool isClickable)
        {
            for (int i = 0; i < count; i++)
            {
                int randomRow, randomColumn, randomImageNumber;
                do
                {
                    randomRow = _random.Next(2, 19);
                    randomColumn = _random.Next(1, 23);
                } while (_gameModel.UsedPositions.Contains((randomRow, randomColumn)));

                _gameModel.UsedPositions.Add((randomRow, randomColumn));
                do
                {
                    randomImageNumber = _random.Next(1, 481);
                } while (_gameModel.UsedImageNumbers.Contains(randomImageNumber));

                _gameModel.UsedImageNumbers.Add(randomImageNumber);

                var image = new Image
                {
                    Source = $"items{randomImageNumber}.png",
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    ZIndex = 50
                };

                Grid.SetRow(image, randomRow);
                Grid.SetColumn(image, randomColumn);
                MainGridImages.Add(image);
            }
        }

        private void UpdateTitleBasedOnLevel(int levelCount)
        {
            TitleUpdated?.Invoke(this, $"Level {levelCount}");
        }

        private static int SetBarrierCount(int levelCount)
        {
            if (levelCount < 6)
            {
                return 20 + levelCount * 2;
            }
            else if (levelCount >= 6 && levelCount < 10)
            {
                return 30 + levelCount * 2;
            }
            else
            {
                return 40 + levelCount * 2;
            }
        }

        private static (int targets, int barriers) SetGameElement(int levelCount)
        {
            int target = levelCount + 2;
            int barriers = SetBarrierCount(levelCount);
            return (target, barriers);
        }

        private void SetHintCount()
        {
            if ((_gameModel.LevelCount - 1) % 5 == 0)
            {
                _gameModel.HintCount = 5;
            }
        }

        private void SetHintUpdate(int levelCount)
        {
            if ((levelCount - 1) % 5 == 0 && levelCount != 1 && _gameModel.HintCount < 5)
            {
                HintUpdateText = $"Hint +{5 - _gameModel.HintCount}";
                IsHintUpdateVisible = true;
            }
            else
            {
                IsHintUpdateVisible = false;
            }
            OnPropertyChanged(nameof(HintUpdateText));
            OnPropertyChanged(nameof(IsHintUpdateVisible));
        }

        private void RestartGame()
        {
            _gameModel.LevelCount = 1;
            _gameModel.TargetItems.Clear();
            GenerateGame();
        }

        private void Hint()
        {
            if (_gameModel.HintCount > 0)
            {
                int randomIndex = _random.Next(_gameModel.TargetItems.Count);
                var image = _gameModel.TargetItems[randomIndex].MainGridImage;
                image.Scale = 2;
                _gameModel.HintCount--;
                BtnHintText = (_gameModel.HintCount <= 1) ? $"{_gameModel.HintCount} Hint" : $"{_gameModel.HintCount} Hints";
                OnPropertyChanged(nameof(BtnHintText));
            }
            else
            {
                AlertRequested?.Invoke(this, ("Oops", "You have used up all Hints! :(", "Ok"));
            }
        }

        private void HideIntro()
        {
            IsIntroVisible = false;
            OnPropertyChanged(nameof(IsIntroVisible));
        }
    }
}
