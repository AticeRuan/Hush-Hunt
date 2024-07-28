
namespace HushHunt.Maui.Views;

public partial class GamePage : ContentPage
{
    private readonly HashSet<int> usedImageNumbers = new HashSet<int>();
    private readonly HashSet<(int, int)> usedPositions = new HashSet<(int, int)>();
    private List<(Image MainGridImage, Image FlexLayoutImage)> targetItems = new List<(Image, Image)>();
    private int levelCount = 1;
    private int HintCount = 5;

    //shell background colour
    private static readonly string[] hexValues = new string[]
{
    "#8fbcf9",
    "#8b4190",
    "#ec8a81",
    "#b657b0",
    "#532a69",
    "#a74899",
    "#833891",
    "#32224d",
    "#5caa76",
    "#d47476",
    "#8cd1a0"
};


    public GamePage()
	{
		InitializeComponent();
       
        GenerateGame();
  
    }

    private void GenerateGame()

    {
        Random random = new Random();

        ResetGame();

        SetHintCount();

        GenerateBackground();


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
            
       

        ClickableImagesLayout.BackgroundColor = Color.FromRgba(255, 255, 255, 0.7);

        UpdateTitleBasedOnLevel(levelCount);
        UpdateTitleView($"Level {levelCount}");

        
        rtgIntro.IsVisible = true;
 
        IntroLayout.IsVisible = true;
        
        lblLevel.Text = $"Level {levelCount}";

        SetHintUpdate(levelCount);
    }

    public void UpdateTitleBasedOnLevel(int levelCount)
    {
       
        this.Title = $"Level {levelCount}";

       
    }

    private void UpdateTitleView(string title)
    {
        if (Shell.Current is AppShell appShell)
        {
            appShell.UpdateTitle(title);
        }
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
                    WidthRequest=50
                };
                LevelIntroFlexLayout.Children.Add(introimage);



                // Add click event handler for the grid images
                image.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = new Command(() => OnImageClicked(image, flexLayoutImage))
                });
                targetItems.Add((image, flexLayoutImage));

            }

            // Add the image to the grid
            MainGrid.Children.Add(image);
        }
    }
    private static int SetBarrierCount(int levelCount)
    {
    
        if (levelCount < 6)
        {
            return  20 + levelCount * 2;
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

    private static (int targets,int barriers)  SetGameElement(int levelCount)
        {
       

            int target = levelCount + 2;
            int barriers = SetBarrierCount(levelCount);

            return (target, barriers);
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

    private double SetBackgroundOpacity (int levelCount)
    {
        if (levelCount <= 5)
        {
            return 0.9;
        }
        else if (levelCount <= 10)
        {
            return 0.95;
        }
        else
        {
            return 1;
        }
    }

   
    private void OnPointerEntered(object sender, PointerEventArgs e)
    {
        if (sender is Image image)
        {
            image.Scale = 2;
          
        }
    }

    private void OnPointerExited(object sender, PointerEventArgs e)
    {
        if (sender is Image image)
        {
            image.Scale = 1;
        }
    }

    private void OnImageClicked(Image mainGridImage, Image flexLayoutImage)
    {
        MainGrid.Children.Remove(mainGridImage);
        ClickableImagesLayout.Children.Remove(flexLayoutImage);
        targetItems.Remove((mainGridImage, flexLayoutImage));

        if (targetItems.Count == 0)
        {
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


            StatusLabel.Text = targetItems.Count<=1?$"{targetItems.Count} item to seek": $"{targetItems.Count} items to seek";
            
        }
    }

    private void btnRestart_Clicked(object sender, EventArgs e)
    {
        levelCount = 1;
        targetItems.Clear();
        GenerateGame();
    }

    private void btnHint_Clicked(object sender, EventArgs e)
    {
        if (HintCount > 0) 
        { 
        Random random = new Random();
        int randomIndex = random.Next(targetItems.Count);
        var image = targetItems[randomIndex].MainGridImage;
        image.Scale = 2;
        HintCount--;
        btnHint.Text = (HintCount <= 1) ? $"{HintCount} Hint" : $"{HintCount} Hints";
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
        if ((levelCount - 1) % 5 == 0 && levelCount != 1&&HintCount<5)
        {
            lblHintUpdate.Text = $"Hint +{5 - HintCount}";
            lblHintUpdate.IsVisible = true;
        }
        else 
        {
            lblHintUpdate.IsVisible = false;
        }
     
    }

    private void btnIntro_Clicked(object sender, EventArgs e)
    {
        rtgIntro.IsVisible = false;
        IntroLayout.IsVisible = false;


    }

}