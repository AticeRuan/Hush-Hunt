
using Microsoft.Maui.Animations;

namespace HushHunt.Maui.Views;

public partial class GamePage : ContentPage
{
    private readonly HashSet<int> usedImageNumbers = new HashSet<int>();
    private readonly HashSet<(int, int)> usedPositions = new HashSet<(int, int)>();
    private List<(Image MainGridImage, Image FlexLayoutImage)> targetItems = new List<(Image, Image)>();
    private int levelCount = 1;
    

    public GamePage()
	{
		InitializeComponent();      

        GenerateGame();
  
    }

    private void GenerateGame()

    {
        Random random = new Random();
        var statusLabel = ClickableImagesLayout.Children.OfType<Label>().FirstOrDefault();
        var levelLabel=MainGrid.Children.OfType<Label>().FirstOrDefault();
        var restartButton= MainGrid.Children.OfType<Button>().FirstOrDefault();

        ResetGame();

        GenerateBackground();



        var gameElement = SetGameElement(levelCount);
        int targetCount = gameElement.targets;
        int barriersCount = gameElement.barriers;
      

        // Generate 3 unique random positions and images for the main game
        GenerateUniqueImages(random, targetCount, true);

        // Generate 20 unique random positions and images for the extra images
        GenerateUniqueImages(random, barriersCount, false);

        if (statusLabel != null)
        {
            ClickableImagesLayout.Children.Add(statusLabel);
            statusLabel.Text = $"{targetItems.Count} items to seek";
        }
        if (levelLabel != null)
        {
            MainGrid.Children.Add(levelLabel);
            LevelLabel.Text = $"Level {levelCount}";
        }
        if (restartButton != null)
        {
            MainGrid.Children.Add(restartButton);
        }
      

    }

    private void ResetGame()
    {
        // Clear existing children
        var flexLayout = MainGrid.Children.OfType<FlexLayout>().FirstOrDefault();
        
        MainGrid.Children.Clear();
        ClickableImagesLayout.Children.Clear();
        usedImageNumbers.Clear();
        usedPositions.Clear();

     

        // Re-add FlexLayout to ensure it's on top
        if (flexLayout != null) MainGrid.Children.Add(flexLayout);
 

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
                };
                // Add image to the FlexLayout
                ClickableImagesLayout.Children.Add(flexLayoutImage);




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
        };

        Grid.SetRow(background, 0);
        Grid.SetColumn(background, 0);
        Grid.SetColumnSpan(background, 24);
        Grid.SetRowSpan(background, 20);




        MainGrid.Children.Add(background);



    }


   
    private void OnPointerEntered(object sender, PointerEventArgs e)
    {
        if (sender is Image image)
        {
            image.Scale = 1.5;
          
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
            if (levelCount < 374)
            {
                levelCount++;

            }
            else
                levelCount = 1;
                
            GenerateGame();
        }
        else
        {
           
            var statusLabel = ClickableImagesLayout.Children.OfType<Label>().FirstOrDefault();
            if (statusLabel != null)
            {
                statusLabel.Text = targetItems.Count==1?$"{targetItems.Count} item to seek": $"{targetItems.Count} items to seek";
            }
        }
    }

    private void btnRestart_Clicked(object sender, EventArgs e)
    {
        levelCount = 1;
        targetItems.Clear();
        GenerateGame();
    }
}