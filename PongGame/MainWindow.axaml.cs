// MainWindow.axaml.cs : handles the logic of the game, including ball movement, collision detection, and UI updates.
// Pong Game:
        // 1.Game for 2 players.
        // 2.It has 2 paddles, with the left operator using keys 'W' , 'S' to move the paddle up and down respectively.
        //Similarly the right paddle operator uses 'KEY UP' and 'KEY DOWN' key to move the paddle. The goal of the game is to bounce the ball off the paddle.
        //3. The game has Pause/Resume button to pause and Resume the game. Also a Restart button to restart the game.
        //4.After the game has ended the score is displayed for the winner. The score gets added with each round.
//  Sources: Basics of Avalonia 11 : https://www.youtube.com/watch?v=TA0VSIrCFw4 ,VectorArt channel 
//  Event handling  : Stackoverflow
// Button Functionality : StackoverFlow, ChatGPT 3.5
// Canvas : ChatGPT 3.5

using Avalonia; // Avalonia is used for creating the user interface, handling input events, and managing the layout of UI elements.
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
using System;
using System.Threading.Tasks;

namespace PongGame
{
    public partial class MainWindow : Window
    {
        // Game variables
        private double ballX; // x position of the pong ball
        private double ballY; // y position of the pong ball
        private double ballSpeedX = 5; // Initial x-vector speed of the pong ball
        private double ballSpeedY = 2; // Initial y-vector speed of the pong ball


        private double paddleLeftY = 100; // Initial position of the left paddle
        private double paddleRightY = 100; // Initial position of the right paddle
        private double paddleSpeed = 20; // Initial speed of the paddle

        // UI elements
        private TextBlock restartTextBlock = null; // This fetches the game
        private TextBlock scoreLeftTextBlock; // Updates the score of the left paddle
        private TextBlock scoreRightTextBlock; // Updates the score of the right paddle
        private TextBlock pauseTextBlock; // Updates status of Game Paused
        // Saves position of the paddle when paused
        private double savedPaddleLeftY; 
        private double savedPaddleRightY;
        private TextBlock winnerTextBlock = null; // Displays winner Text on UI
        // Game state
        private bool isGameOverDisplayVisible = false;  // This boolean variable makes the game text visible only when required
        private string winner = string.Empty; // This variable stores the winner of a game
        private bool isGamePaused = false;  // This boolean variable is used to control the pause condition of the game
        private int scoreLeft = 0; // This variable calculates the wins of the left paddle controller
        private int scoreRight = 0; // This variable calculates the wins of the right paddle controller

        private bool isGameRunning = true; 
        private bool isroundended= false; // Checks the gaame end condition

        // Constructor
        public MainWindow()
        {
            System.Diagnostics.Debug.WriteLine("Initializing PongGame...");

            // Initialize UI components
            InitializeComponent();


            System.Diagnostics.Debug.WriteLine($" Bounds.Height: {Bounds.Height}, Bounds.Width: {Bounds.Width}");
            System.Diagnostics.Debug.WriteLine($" paddleLeftY : {paddleLeftY}, ballY : {Bounds.Width}");

            // Set up the game loop timer
            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(16),
            };
            timer.Tick += (sender, args) => Update(); //the timer will tick every 16 milliseconds: Update function is called every 16 milliseconds
            timer.Start();

            // Register input event handlers
            KeyDown += OnKeyDown!;
            KeyUp += OnKeyUp!;
            this.Loaded += OnLoaded;
        }

        // Event handler for the Loaded event
        private void OnLoaded(object? sender, RoutedEventArgs e)
        {
            this.Loaded -= OnLoaded;



            // Find UI elements by name
            scoreLeftTextBlock = this.FindControl<TextBlock>("scoreLeftText");
            scoreRightTextBlock = this.FindControl<TextBlock>("scoreRightText");
            pauseTextBlock = this.FindControl<TextBlock>("pauseText");
            restartTextBlock = this.FindControl<TextBlock>("restartText");
            winnerTextBlock = this.FindControl<TextBlock>("winnerText");

            // Set initial ball position closer to the center
            ballX = Bounds.Width / 2;
            ballY = Bounds.Height / 2;

            // Center the paddles vertically
            paddleLeftY = (Bounds.Height - paddleLeft.Height) / 2;
            paddleRightY = (Bounds.Height - paddleRight.Height) / 2;

            // Ensure paddle positions are within bounds
            paddleLeftY = Math.Clamp(paddleLeftY, 0, Height - paddleLeft.Height);
            paddleRightY = Math.Clamp(paddleRightY, 0, Height - paddleRight.Height);
        }


        // Main game update loop : responsible for updating the game state on each frame.
        private async Task Update()
        {
            System.Diagnostics.Debug.WriteLine("Updating PongGame...");
            // Check if the game is running
            if (!isGameRunning)
            {
                return;
            }
            // Check game state and update positions
            if (internalBall != null && Bounds.Height > 0 && !isGamePaused && !isroundended)
            {
                ballX += ballSpeedX;
                ballY += ballSpeedY;

                //System.Diagnostics.Debug.WriteLine($"ballX: {ballX}, ballY: {ballY}, ballSpeedX: {ballSpeedX}, ballSpeedY: {ballSpeedY}");


                // Check ball collisions with walls and paddles
                if (ballY - internalBall.Height / 2 <= 0)
                {
                    System.Diagnostics.Debug.WriteLine("Bouncing off top");
                    ballSpeedY = Math.Abs(ballSpeedY);
                    ballY = internalBall.Height / 2;
                }
                else if (ballY + internalBall.Height / 2 >= Bounds.Height)
                {
                    System.Diagnostics.Debug.WriteLine("Bouncing off bottom");
                    ballSpeedY = -Math.Abs(ballSpeedY);
                    ballY = Bounds.Height - internalBall.Height / 2;
                }

                if (paddleLeft != null && ballX - internalBall.Width / 2 <= paddleLeft.Width && ballY >= paddleLeftY && ballY <= paddleLeftY + paddleLeft.Height)
                {
                    System.Diagnostics.Debug.WriteLine("Bouncing off left paddle");
                    ballSpeedX = Math.Abs(ballSpeedX);
                    ballX = paddleLeft.Width + internalBall.Width / 2;
                    System.Diagnostics.Debug.WriteLine($" ballSpeedX: {ballSpeedX}, ballX: {ballX}");
                }

                if (paddleRight != null && ballX + internalBall.Width / 2 >= Bounds.Width - paddleRight.Width && ballY >= paddleRightY && ballY <= paddleRightY + paddleRight.Height)
                {
                    System.Diagnostics.Debug.WriteLine("Bouncing off right paddle");
                    ballSpeedX = -Math.Abs(ballSpeedX);
                    ballX = Bounds.Width - paddleRight.Width - internalBall.Width / 2;
                    System.Diagnostics.Debug.WriteLine($" ballSpeedX: {ballSpeedX}, ballX: {ballX}");
                }

                // Check for game over condition
                //The condition checks whether the ball has gone out of bounds on the left side (ballX - internalBall.Width / 2 <= 0) or
                //the right side (ballX + internalBall.Width / 2 >= Bounds.Width).
                //If this condition is true, it means the ball has crossed either the left or right boundary of the game window.
                //The winner is then determined based on which side the ball went out. If the left side boundary is crossed, the winner is the "Right Paddle";
                //if the right side boundary is crossed, the winner is the "Left Paddle". The Game Pauses after the round has ended.
      
                if (ballX - internalBall.Width / 2 <= 0 || ballX + internalBall.Width / 2 >= Bounds.Width)
                {
                    winner = ballX - internalBall.Width / 2 <= 0 ? "Right Paddle" : "Left Paddle";
                    System.Diagnostics.Debug.WriteLine($"Game Over! {winner} Wins!");
                    isroundended = true;
                    await ResetGame(winner);
                    return;
                }

                // Update paddle positions and UI elements
                paddleLeftY = Math.Clamp(paddleLeftY, 0, Bounds.Height - paddleLeft.Height);
                paddleRightY = Math.Clamp(paddleRightY, 0, Bounds.Height - paddleRight.Height);

                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    // Update UI elements
                    Canvas.SetLeft(internalBall, ballX);
                    Canvas.SetTop(internalBall, ballY);

                    // Check for null before updating UI elements
                    if (paddleLeft != null)
                    {
                        Canvas.SetLeft(paddleLeft, 0);
                        Canvas.SetTop(paddleLeft, paddleLeftY);
                    }

                    if (paddleRight != null)
                    {
                        Canvas.SetLeft(paddleRight, Bounds.Width - paddleRight.Width);
                        Canvas.SetTop(paddleRight, paddleRightY);
                    }


                    //method in Avalonia is used to request a redraw or refresh of the visual content of a control or window
                    internalBall.InvalidateVisual();
                    this.InvalidateVisual();


                    // Update game information in UI
                    if (winnerTextBlock != null)
                    {
                        winnerTextBlock.IsVisible = isGameOverDisplayVisible;

                        if (isGameOverDisplayVisible)
                        {
                            winnerTextBlock.Text = ""; // Ensure the winnerTextBlock is initially blank
                            winnerTextBlock.Text = $"Game Over: {winner} Wins!";

                            System.Diagnostics.Debug.WriteLine($"Game Over: {winner} Wins!");
                        }
                        else
                        {
                            // Display "Game Restarted" text if the game was restarted
                            winnerTextBlock.Text = winner == "Game Restarted" ? "Game Restarted" : "";
                        }
                    }
                    if (scoreLeftTextBlock != null && scoreRightTextBlock != null)
                    {
                        scoreLeftTextBlock.Text = $"Left Score: {scoreLeft}";
                        scoreRightTextBlock.Text = $"Right Score: {scoreRight}";
                    }

                    // Display pause state in the UI
                    if (pauseTextBlock != null)
                    {
                        pauseTextBlock.Text = isGamePaused ? "Paused" : "";

                        // Set the position of the pauseTextBlock
                        double pauseTextLeft = (Bounds.Width - pauseTextBlock.Width) / 2;
                        double pauseTextTop = (Bounds.Height - pauseTextBlock.Height) / 2;

                        Canvas.SetLeft(pauseTextBlock, pauseTextLeft);
                        Canvas.SetTop(pauseTextBlock, pauseTextTop);
                    }
                });
            }
        }



        /// Method to reset the game state: ResetGame method is called, passing the winner's name as an argument.
        // This method increments the corresponding score, resets the ball and paddle positions, and displays a brief "Game Over" message
    
        private async Task ResetGame(string winner)
        {
            if (isroundended)
            {
                // Increment the score based on the winner
                if (winner == "Left Paddle")
                    scoreLeft++;
                else if (winner == "Right Paddle")
                    scoreRight++;

                // Update UI elements without resetting the game state
                await UpdateUI(winner);

                // Reset the ball and paddle positions
                ballX = Bounds.Width / 2;
                ballY = Bounds.Height / 2;

                paddleLeftY = (Bounds.Height - paddleLeft.Height) / 2;
                paddleRightY = (Bounds.Height - paddleRight.Height) / 2;

                // Ensure paddle positions are within bounds
                paddleLeftY = Math.Clamp(paddleLeftY, 0, Height - paddleLeft.Height);
                paddleRightY = Math.Clamp(paddleRightY, 0, Height - paddleRight.Height);

                // Delay before continuing the game (2 seconds)
                await Task.Delay(2000);

            }

        }



        // Displays UI information

        private async Task UpdateUI(string winner)
        {
            // Display game over information
            if (restartTextBlock != null)
            {
                isGameOverDisplayVisible = true;
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    restartTextBlock.IsVisible = true;

                    // Display "Game Restarted" text if the game was restarted
                    restartTextBlock.Text = winner == "Game Restarted" ? "Game Restarted" : "";
                });

              

                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    restartTextBlock.IsVisible = false;
                    restartTextBlock.Text = "";
                    isGameOverDisplayVisible = false;
                });
            }

            // Display winner information in winnerTextBlock
            if (winnerTextBlock != null && winner != "Game Restarted")
            {
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    winnerTextBlock.IsVisible = true;
                    winnerTextBlock.Text = $"Game Over: {winner} Wins!";
                });
            }

            // Display final scores 
            if (scoreLeftTextBlock != null && scoreRightTextBlock != null)
            {
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    scoreLeftTextBlock.Text = $"Left Score: {scoreLeft}";
                    scoreRightTextBlock.Text = $"Right Score: {scoreRight}";
                });
            }

            // Delay before continuing the game (2 seconds)
            await Task.Delay(2000);
            //isGamePaused = true;
            await Task.Delay(2000);
        }



        // Event handler for the pause/resume button click event
        private void PauseResume_Click(object sender, RoutedEventArgs e)
        {
            isGamePaused = !isGamePaused; 

            if (isGamePaused)
            {
                // Save paddle positions before pausing
                savedPaddleLeftY = paddleLeftY;
                savedPaddleRightY = paddleRightY;
            }
            else
            {
                // Restore paddle positions when resuming
                paddleLeftY = savedPaddleLeftY;
                paddleRightY = savedPaddleRightY;
            }

            //System.Diagnostics.Debug.WriteLine($"Game {(isGamePaused ? "Paused" : "Resumed")}");
        }



        // Event handler for the Restart functionality
        private async void RestartGame_Click(object sender, RoutedEventArgs e)
        {
            isroundended = true;
            isGamePaused =false;
            if (!isGamePaused || (!isGameOverDisplayVisible && isroundended))
            {
                // Display restart message 
                restartTextBlock.Text = "Game Restarted!";
                restartTextBlock.IsVisible = true;
                restartTextBlock.InvalidateVisual();

                // Delay for 2 seconds
                //await Task.Delay(2000);

                // Restart the game after the restart button is clicked
                await ResetGame("Game Restarted");
                isroundended = false;
            }
            // Hide the restart message
            restartTextBlock.IsVisible = false;

        }


        // Event handler for key press events
        private async void OnKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.P)
                {
                    PauseResume_Click(sender, e);
                }
                else if (isGamePaused)
                {
                    // If the game is paused, reset paddle speeds to avoid unintended movement during pause
                    paddleLeftY = 0;
                    paddleRightY = 0;
                }
                else
                {
                    // Update paddle positions based on key presses
                    switch (e.Key)
                    {
                        case Key.W:
                            paddleLeftY -= paddleSpeed;
                            break;
                        case Key.S:
                            paddleLeftY += paddleSpeed;
                            break;
                        case Key.Up:
                            paddleRightY -= paddleSpeed;
                            break;
                        case Key.Down:
                            paddleRightY += paddleSpeed;
                            break;
                        
                    }
                    // Ensure paddle positions are within bounds
                    paddleLeftY = Math.Clamp(paddleLeftY, 0, Height - paddleLeft.Height);
                    paddleRightY = Math.Clamp(paddleRightY, 0, Height - paddleRight.Height);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception in OnKeyDown: {ex.Message}");
            }
        }





        // Event handler for key release events
        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                
                switch (e.Key)
                {
                    case Key.W:
                    case Key.S:
                        break;
                    case Key.Up:
                    case Key.Down:
                        break;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception in OnKeyUp: {ex.Message}");
            }
        }

        // Initialization method for UI components
        private void InitializeComponent()
        {
            try
            {
                // Load XAML layout
                AvaloniaXamlLoader.Load(this);
                // Find UI elements by name
                internalBall = this.FindControl<Ellipse>("internalBall");
                paddleLeft = this.FindControl<Rectangle>("paddleLeft");
                paddleRight = this.FindControl<Rectangle>("paddleRight");
                scoreLeftTextBlock = this.FindControl<TextBlock>("scoreLeftText");
                scoreRightTextBlock = this.FindControl<TextBlock>("scoreRightText");
                pauseTextBlock = this.FindControl<TextBlock>("pauseText");
                restartTextBlock = this.FindControl<TextBlock>("restartText");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception in InitializeComponent: {ex.Message}");
            }
        }
    }
}
