
using HushHunt.Maui.ViewModels;
using Plugin.Maui.Audio;
using HushHunt.Maui.Models;
using System.Diagnostics;


namespace HushHunt.Maui.Views;

public partial class GamePage : ContentPage
{


    private readonly HashSet<int> usedImageNumbers = new HashSet<int>();
    private readonly HashSet<(int, int)> usedPositions = new HashSet<(int, int)>();
    private List<(Image MainGridImage, Image FlexLayoutImage)> targetItems = new List<(Image, Image)>();
    private int levelCount = 1;
    private int HintCount = 5;
    private TimerManager _timerManager;
    private bool _isGameActive;



    public GamePage(IAudioManager audioManager)
    {
        InitializeComponent();
        
        _timerManager = new TimerManager(TimeSpan.FromSeconds(90));
        GenerateGame();


    }

    #region Game Logic

    private void GenerateGame()

    {
        Random random = new Random();

        ResetGame();

        SetHintCount();
              


        var gameElement = SetGameElement(levelCount);
        int targetCount = gameElement.targets;
        int barriersCount = gameElement.barriers;

        // Generate  unique random positions and images for the main game
        GenerateUniqueImages(random, targetCount, true);

        // Generate  unique random positions and images for the extra images
        GenerateUniqueImages(random, barriersCount, false);

        StatusLabel.Text = targetItems.Count <= 1 ? $"{targetItems.Count} item to seek" : $"{targetItems.Count} items to seek";


        btnHint.Text = (HintCount == 1) ? $"{HintCount} Hint" : $"{HintCount} Hints";
              


    }
    private async void EndGame()
    {
        _isGameActive = false;
        endGameLayout.IsVisible = true;
        rtgIntro.IsVisible = true;
        await timeOutLabel.ScaleTo(2, 300);
        await timeOutLabel.ScaleTo(1, 300);
        await timeOutLabel.ScaleTo(3, 300);
        await timeOutLabel.ScaleTo(0, 300);
        await Task.Delay(TimeSpan.FromSeconds(0.9));
        SoundManager.Instance.PlaySound("game_over.mp3");
        endGameLabel.Scale = 0;
        Restart_button.Scale = 0;
        endGameLabel.IsVisible = true;
        Restart_button.IsVisible = true;
        Restart_button.Scale = 0;
        await endGameLabel.ScaleTo(2, 300);
        await endGameLabel.ScaleTo(1, 300);
        await Restart_button.ScaleTo(2, 300);
        await Restart_button.ScaleTo(1, 300);



    }

    private void ResetGame()
    {

        //clear items on the grid
        var images = MainGrid.Children.OfType<Image>().ToList();

        var flexlayerImages = ClickableImagesLayout.Children.OfType<Image>().ToList();


        foreach (var image in images)
        {
            MainGrid.Children.Remove(image);
        }

        foreach (var image in flexlayerImages)
        {
            ClickableImagesLayout.Children.Remove(image);
        }



        LevelIntroFlexLayout.Children.Clear();
        usedImageNumbers.Clear();
        usedPositions.Clear();

        GenerateBackground();

        ClickableImagesLayout.BackgroundColor = Color.FromRgba(255, 255, 255, 0.7);

        UpdateTitleBasedOnLevel(levelCount);
        UpdateTitleView($"Level {levelCount}");


        rtgIntro.IsVisible = true;

        IntroLayout.IsVisible = true;

        lblLevel.Text = $"Level {levelCount}";

        SetHintUpdate(levelCount);

        _timerManager.Reset();
        _isGameActive = false;
    }
    #endregion

    #region Game Variable/Element Logic

    public void UpdateTitleBasedOnLevel(int levelCount)
        {

            this.Title = $"Level {levelCount}";


        }

    private void GenerateUniqueImages(Random random, int count, bool isClickable)
        {
            for (int i = 0; i < count; i++)
            {
                int randomRow, randomColumn, randomImageNumber;

                // Ensure unique positions
                do
                {
                    randomRow = random.Next(2, 19);
                    randomColumn = random.Next(1, 23);
                } while (usedPositions.Contains((randomRow, randomColumn)));

                usedPositions.Add((randomRow, randomColumn));

                // Ensure unique image numbers
                do
                {
                    randomImageNumber = random.Next(1, 481);
                } while (usedImageNumbers.Contains(randomImageNumber));

                usedImageNumbers.Add(randomImageNumber);

                // Create and configure the image
                var image = new Image
                {
                    Source = $"items{randomImageNumber}.png",
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    ZIndex = 50
                };

                // Assign row and column
                Grid.SetRow(image, randomRow);
                Grid.SetColumn(image, randomColumn);



                // Add hover effect using PointerGestureRecognizer
                var pointerGestureRecognizer = new PointerGestureRecognizer();
                pointerGestureRecognizer.PointerEntered += (s, e) => OnPointerEntered(image, e);
                pointerGestureRecognizer.PointerExited += (s, e) => OnPointerExited(image, e);
                image.GestureRecognizers.Add(pointerGestureRecognizer);


                // Add click event handler if it's a clickable image
                if (isClickable)
                {
                    var flexLayoutImage = new Image
                    {
                        Source = image.Source,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                        Margin = new Thickness(3, 0, 0, 0)
                    };
                    // Add image to the FlexLayout
                    ClickableImagesLayout.Children.Add(flexLayoutImage);

                    var introimage = new Image
                    {
                        Source = image.Source,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                        Margin = new Thickness(3, 0, 0, 0),
                        WidthRequest = 50
                    };
                    LevelIntroFlexLayout.Children.Add(introimage);



                    // Add click event handler for the grid images
                    image.GestureRecognizers.Add(new TapGestureRecognizer
                    {
                        Command = new Command(() => OnImageClicked(image, flexLayoutImage))
                    });
                    targetItems.Add((image, flexLayoutImage));

                }
                else
                {
                    image.GestureRecognizers.Add(new TapGestureRecognizer
                    {
                        Command = new Command(() => OnImageClickWrong(image))
                    });
                }


                // Add the image to the grid
                MainGrid.Children.Add(image);
            }
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
    private void GenerateBackground()

    {

        int backgroundIndex = ((levelCount - 1) % 11) + 1;

        var background = new Image
        {
            Source = $"background{backgroundIndex}.png",
            Aspect = Aspect.AspectFill,
            //HorizontalOptions = LayoutOptions.Center,
            //VerticalOptions = LayoutOptions.Center
            Opacity = SetBackgroundOpacity(levelCount)
        };

        Grid.SetRow(background, 0);
        Grid.SetColumn(background, 0);
        Grid.SetColumnSpan(background, 24);
        Grid.SetRowSpan(background, 20);



        MainGrid.Children.Add(background);


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

    private static (int targets, int barriers) SetGameElement(int levelCount)
        {


            int target = levelCount + 2;
            int barriers = SetBarrierCount(levelCount);

            return (target, barriers);
        }

    //target image logic
    private void OnImageClicked(Image mainGridImage, Image flexLayoutImage)
    {
        MainGrid.Children.Remove(mainGridImage);
        ClickableImagesLayout.Children.Remove(flexLayoutImage);
        targetItems.Remove((mainGridImage, flexLayoutImage));





        SoundManager.Instance.PlaySound("coin.mp3");

        if (targetItems.Count == 0)
        {
            SoundManager.Instance.PlaySound("level_up.mp3");
            if ((40 + levelCount * 2) < 374)
            {
                levelCount++;

            }
            else
                levelCount = 1;

            GenerateGame();
        }
        else
        {


            StatusLabel.Text = targetItems.Count <= 1 ? $"{targetItems.Count} item to seek" : $"{targetItems.Count} items to seek";

        }
    }

    private void OnImageClickWrong(Image image)
    {


        SoundManager.Instance.PlaySound("error.mp3");

    }

   //Restart Button logic
    private void btnRestart_Clicked(object sender, EventArgs e)
    {
        levelCount = 1;
        targetItems.Clear();
        GenerateGame();
        _timerManager.Stop();
        _timerManager.Reset();
        endGameLayout.IsVisible = false;
        SoundManager.Instance.PlaySound("level_up.mp3");
    }
    //Hint button logic
    private  void btnHint_Clicked(object sender, EventArgs e)
    {
        if (HintCount > 0)
        {
            Random random = new Random();
            
            int randomIndex = random.Next(targetItems.Count);
            var image = targetItems[randomIndex].MainGridImage;
            image.Scale = 2;
            HintCount--;
            btnHint.Text = (HintCount <= 1) ? $"{HintCount} Hint" : $"{HintCount} Hints";
            SoundManager.Instance.PlaySound("hint.mp3");
            
        }

        else
        {
            DisplayAlert("Oops", "You have used up all Hints! :(", "Ok");
        }

    }
    private void SetHintCount()
    {
        if ((levelCount - 1) % 5 == 0)
        {
            HintCount = 5;
        }
    }

    private void SetHintUpdate(int levelCount)
    {
        if ((levelCount - 1) % 5 == 0 && levelCount != 1 && HintCount < 5)
        {
            lblHintUpdate.Text = $"Hint +{5 - HintCount}";
            lblHintUpdate.IsVisible = true;
        }
        else
        {
            lblHintUpdate.IsVisible = false;
        }

    }
    //timer logic 
    private void UpdateTimerDisplay()
    {
        if (timerLabel == null || _timerManager == null)
        {
            Debug.WriteLine("timerLabel o is null.");
            return;
        }

        try
        {
            timerLabel.Text = _timerManager.RemainingTime.ToString(@"mm\:ss");
            var remainingTime = _timerManager.RemainingTime.TotalSeconds;

            if (timerLabel.Text == "00:10")
            {

                SoundManager.Instance.PlaySound("ticking.mp3");
            }
            else if (remainingTime < 11 && remainingTime > 0)
            {
                timerLabel.TextColor = Colors.Red;
                timerLabel.FontAttributes = FontAttributes.Bold;
                StartBlinkingAnimation();

            }
            else
            {
                timerLabel.TextColor = Colors.DarkSlateGray;
                timerLabel.FontAttributes = FontAttributes.None;
                StopBlinkingAnimation();
            }

            Debug.WriteLine($"Timer label updated: {_timerManager.RemainingTime}");

        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Exception updating timer label: {ex.Message}");
        }
    }

    private void StartTimer()
    {
        Debug.WriteLine("Starting timer...");
        _isGameActive = true;
        _timerManager.Stop();
        _timerManager.Reset();
        _timerManager.Start();

        Dispatcher.StartTimer(TimeSpan.FromSeconds(1), () =>
        {
            Debug.WriteLine("Timer tick");

            MainThread.BeginInvokeOnMainThread(() =>
            {
                _timerManager.Update();
                UpdateTimerDisplay();
            });

            if (_timerManager.RemainingTime == TimeSpan.Zero)
            {
                EndGame();
                return false;
            }

            return _isGameActive;
        });
    }


    #endregion

    #region Main Game UI Logic
    private void UpdateTitleView(string title)
        {
            if (Shell.Current is AppShell appShell)
            {
                appShell.UpdateTitle(title);
            }
        }
    //Image element ui logic
    private void OnPointerEntered(object sender, PointerEventArgs e)
    {
        if (sender is Image image)
        {
            image.Scale = 2;


            SoundManager.Instance.PlaySound("pop.mp3");

        }
    }

    private void OnPointerExited(object sender, PointerEventArgs e)
    {
        if (sender is Image image)
        {
            image.Scale = 1;
        }
    }

    //Level intro ui logic
    private void btnIntro_Clicked(object sender, EventArgs e)
    {
        rtgIntro.IsVisible = false;
        IntroLayout.IsVisible = false;
        StartTimer();
        SoundManager.Instance.PlaySound("yay.mp3");


    }

    private void btnIntro_PointerEntered(object sender, PointerEventArgs e)
    {

        SoundManager.Instance.PlaySound("swoosh.mp3");
        btnIntro.Scale = 1.2;
    }

    private void btnIntro_PointerExited(object sender, PointerEventArgs e)
    {
        btnIntro.Scale = 1;
    }

    //Time UI Logic

    private void StartBlinkingAnimation()
    {
        var animation = new Animation();


        var opacityAnimation = new Animation(v => timerLabel.Opacity = v, 1, 0.1, Easing.Linear);

        // Scaling animation (Scale)
        var scaleAnimation = new Animation(v => timerLabel.Scale = v, 1, 1.2, Easing.Linear);


        animation.Add(0, 1, opacityAnimation);
        animation.Add(0, 1, scaleAnimation);


        animation.Commit(timerLabel, "BlinkingWithScaling", 16, 500, Easing.Linear, (v, c) =>
        {
            timerLabel.Opacity = 1;
            timerLabel.Scale = 1;
        }, () => true);
    }

    private void StopBlinkingAnimation()
    {

        timerLabel.AbortAnimation("BlinkingWithScaling");
        timerLabel.Opacity = 1;
        timerLabel.Scale = 1;
    }

    //Restart button UI logic

    private async void PointerGestureRecognizer_PointerEntered(object sender, PointerEventArgs e)
    {

        await btnRestart.ScaleTo(1.2, 200);
        await btnRestart.ScaleTo(1, 200);

    }

    //Hint button UI logic
    private async void PointerGestureRecognizer_PointerEntered_1(object sender, PointerEventArgs e)
    {
        await btnHint.ScaleTo(1.2, 200);
        await btnHint.ScaleTo(1, 200);
    }

    #endregion

















}