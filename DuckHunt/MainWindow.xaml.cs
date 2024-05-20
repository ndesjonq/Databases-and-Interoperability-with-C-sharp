using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Diagnostics;
using System.Threading;

namespace DuckHunt
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<Entity> entities;

        public Hunter hunter;
        public string hunter_username;
        public Dictionary<string, Key> hunter_movement_keys = new() { { "up", Key.Up }, { "down", Key.Down }, { "left", Key.Left }, { "right", Key.Right } };

        public Duck duck;
        public string duck_username;
        public Dictionary<string, Key> duck_movement_keys = new() { { "up", Key.Z }, { "down", Key.S }, { "left", Key.Q }, { "right", Key.D } };

        public static List<Bullet> bullets = new() { };

        public bool shooting;
        public bool game_started;
        public int frame;
        public int shooting_speed = 15;

        public DatabaseManager game_database;

        public string player_playing;
        readonly DispatcherTimer game_timer = new(DispatcherPriority.Render);

        public Dictionary<string, int> score = new() { { "hunter", 0 }, { "duck", 0} };

        public MediaPlayer quack = new MediaPlayer();
        public bool quacking;

        public static Stopwatch clock = new();
        public static int game_duration = 60;
        public static int seconds_remaining;

        public static int low_time_multiplier = 2;

        public MainWindow()
        {
            game_database = new("duckhunt_database3");

            quack.Open(new Uri(AppDomain.CurrentDomain.BaseDirectory + "quack.wav"));

            game_timer.Tick += GameTimerTick;
            game_timer.Interval = TimeSpan.FromMilliseconds(1000 / 120);

            InitializeComponent();
        }

        public void StartGame()
        {
            quacking = false;
            game_started = true;

            score["duck"] = 0;
            score["hunter"] = 0;

            play_menu.Visibility = Visibility.Collapsed;
            game_panel.Visibility = Visibility.Visible;

            hunter_username_textbox.Text = "";
            hunter_password_textbox.Text = "";
            duck_username_textbox.Text = "";
            duck_password_textbox.Text = "";

            hunter = new Hunter(hunter_sprite, 4, 540, 252, hunter_movement_keys);
            duck = new Duck(duck_sprite, 10, 100, 79, duck_movement_keys);

            entities = new List<Entity> { hunter, duck };

            clock.Start();

            game_timer.Start();
        }

        public void EndGame()
        {
            hunter.Sprite.Visibility = Visibility.Collapsed;
            duck.Sprite.Visibility = Visibility.Collapsed;
            score_label.Visibility = Visibility.Collapsed;
            clock_label.Visibility = Visibility.Collapsed;

            winner_label.Content = $"The {(score["hunter"] > score["duck"] ? "hunter" : "duck")} wins !";
            winner_label.Visibility = Visibility.Visible;

            winner_scores_label.Content = $"Hunter : {score["hunter"]} | Duck : {score["duck"]}";
            winner_scores_label.Visibility = Visibility.Visible;
            winning_screen_background.Visibility = Visibility.Visible;

            winning_screen_back_button.Visibility = Visibility.Visible;

            bullets = new() { };

            game_database.AddScore("hunter", hunter_username, score["hunter"], date: Convert.ToString(DateTime.Now));
            game_database.AddScore("duck", duck_username, score["duck"], Convert.ToString(DateTime.Now));

            clock.Reset();
            game_timer.Stop();

            game_started = false;
        }

        public void GameTimerTick(object sender, EventArgs e)
        {
            seconds_remaining = game_duration - (int)(clock.ElapsedMilliseconds / 1000);

            if (seconds_remaining < 0)
            {
                EndGame();
            }

            score_label.Content = $"Hunter : {score["hunter"]} | Duck : {score["duck"]}";

            if (seconds_remaining < 20)
            {
                clock_label.Foreground = Brushes.Red;
            }

            if ((string)clock_label.Content != $"{game_duration - (int)(clock.ElapsedMilliseconds / 1000)}")
            {
                score["duck"] += 50 * low_time_multiplier;
            }

            clock_label.Content = $"{game_duration - (int)(clock.ElapsedMilliseconds /1000)}";

            frame++;

            if (shooting && frame > shooting_speed)
            {
                frame = 0;
                hunter.Shoot(canvas);
            }

            Move();
        }

        public void Move()
        {
            hunter.Rotate(canvas);

            hunter.Move(canvas);
            duck.Move(canvas);

            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                bullets[i].UpdatePosition(this, duck);

                if (bullets[i].Has_hit != "")
                {
                    if (bullets[i].Has_hit == "duck")
                    {
                        score["hunter"] += 100 * low_time_multiplier;
                        score["duck"] = Math.Max(score["duck"] - 50 * low_time_multiplier, 0);
                    }

                    canvas.Children.Remove(bullets[i].Sprite);

                    bullets.RemoveAt(i);
                }
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (game_started)
            {
                foreach (Entity entity in entities)
                {
                    if (e.Key == entity.Movement_keys["up"])
                    {
                        entity.Up = true;
                    }
                    else if (e.Key == entity.Movement_keys["down"])
                    {
                        entity.Down = true;
                    }

                    if (e.Key == entity.Movement_keys["left"])
                    {
                        entity.Left = true;
                    }
                    else if (e.Key == entity.Movement_keys["right"])
                    {
                        entity.Right = true;
                    }
                }

                if (e.Key == Key.Space && !quacking)
                {
                    quack.Position = TimeSpan.Zero;
                    quack.Play();
                    quacking = true;
                    score["duck"] += 20 * low_time_multiplier;
                    score["hunter"] = Math.Max(score["hunter"] - 10 * low_time_multiplier, 0);
                }
            }
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (game_started)
            {
                foreach (Entity entity in entities)
                {
                    if (e.Key == entity.Movement_keys["up"])
                    {
                        entity.Up = false;
                    }
                    else if (e.Key == entity.Movement_keys["down"])
                    {
                        entity.Down = false;
                    }

                    if (e.Key == entity.Movement_keys["left"])
                    {
                        entity.Left = false;
                    }
                    else if (e.Key == entity.Movement_keys["right"])
                    {
                        entity.Right = false;
                    }
                }

                if (e.Key == Key.Space)
                {
                    quacking = false;
                }
            }
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (game_started)
            {
                shooting = true;
            }
        }

        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (game_started)
            {
                shooting = false;
            }
        }

        private void play_button_Click(object sender, RoutedEventArgs e)
        {
            hunter_username_textbox.Text = "";
            hunter_password_textbox.Text = "";
            duck_username_textbox.Text = "";
            duck_password_textbox.Text = "";

            main_menu.Visibility = Visibility.Collapsed;
            play_menu.Visibility = Visibility.Visible;
        }

        private void leaderboards_button_Click(object sender, RoutedEventArgs e)
        {
            (hunters_leaderboard_textblock.Text, ducks_leaderboard_textblock.Text) = game_database.GetLeaderBoards();

            main_menu.Visibility = Visibility.Collapsed;
            leaderboards_menu.Visibility = Visibility.Visible;
        }

        private void list_players_button_Click(object sender, RoutedEventArgs e)
        {
            list_players_textblock.Text = game_database.GetAllPlayerIds(true);

            main_menu.Visibility = Visibility.Collapsed;
            list_players_menu.Visibility = Visibility.Visible;
        }

        private void back_button_Click(object sender, RoutedEventArgs e)
        {
            main_menu.Visibility = Visibility.Visible;

            play_menu.Visibility = Visibility.Collapsed;
            leaderboards_menu.Visibility = Visibility.Collapsed;
            list_players_menu.Visibility = Visibility.Collapsed;
            create_player_menu.Visibility = Visibility.Collapsed;

            game_panel.Visibility = Visibility.Collapsed;
        }

        private void start_game_button_Click(object sender, RoutedEventArgs e)
        {
            hunter_username = hunter_username_textbox.Text;
            string hunter_password = hunter_password_textbox.Text;

            duck_username = duck_username_textbox.Text;
            string duck_password = duck_password_textbox.Text;

            if (hunter_username != "" && hunter_password != "" && duck_username != "" & duck_password != "")
            {
                if (hunter_username != duck_username)
                {
                    bool hunter_user_exists = game_database.CheckIfPlayerExists(hunter_username);
                    bool duck_user_exists = game_database.CheckIfPlayerExists(duck_username);

                    if (hunter_user_exists && duck_user_exists)
                    {
                        bool hunter_pword_correct = game_database.CheckIfPasswordIsCorrect(hunter_username, hunter_password);
                        bool duck_pword_correct = game_database.CheckIfPasswordIsCorrect(duck_username, duck_password);

                        if (hunter_pword_correct && duck_pword_correct)
                        {
                            error_label.Content = "";

                            StartGame();
                        }
                        else if (hunter_pword_correct)
                        {
                            error_label.Content = $"The password of the user {hunter_username} is incorrect.";
                        }
                        else if (!duck_pword_correct)
                        {
                            error_label.Content = $"The password of the user {duck_username} is incorrect.";
                        }
                        else
                        {
                            error_label.Content = $"The password of the two users are incorrect.";
                        }
                    }
                    else if (!hunter_user_exists)
                    {
                        error_label.Content = $"The user {hunter_username} does not exist";
                    }
                    else if (!duck_user_exists)
                    {
                        error_label.Content = $"The user {duck_password} does not exist";
                    }
                    else
                    {
                        error_label.Content = $"The users {hunter_username} and {duck_password} does not exist";
                    }
                }
                else
                {
                    error_label.Content = "Please enter two different users.";
                }
            }
            else
            {
                error_label.Content = "Please fill in all the details of the two users.";
            }
        }

        private void create_player_button_Click(object sender, RoutedEventArgs e)
        {
            create_user_password_textbox.Text = "";
            create_user_username_textbox.Text = "";
            error_label2.Content = "";

            main_menu.Visibility = Visibility.Collapsed;
            create_player_menu.Visibility = Visibility.Visible;
        }

        private void register_button_Click(object sender, RoutedEventArgs e)
        {
            string username = create_user_username_textbox.Text;
            string password = create_user_password_textbox.Text;

            if (username != "" && password != "")
            {
                if (!game_database.CheckIfPlayerExists(username))
                {
                    game_database.AddPlayer(username, password);

                    create_user_password_textbox.Text = "";
                    create_user_username_textbox.Text = "";

                    error_label2.Content = "User successfully created !";
                }
                else
                {
                    error_label2.Content = $"Username {username} already exists.";
                }
            } 
            else if (username == "")
            {
                error_label2.Content = "Please enter a username.";
            }
            else
            {
                error_label2.Content = "Please enter a password";
            }
        }

        private void winning_screen_back_button_Click(object sender, RoutedEventArgs e)
        {
            winner_label.Visibility = Visibility.Collapsed;
            winner_scores_label.Visibility = Visibility.Collapsed;
            winning_screen_background.Visibility = Visibility.Collapsed;

            hunter.Sprite.Visibility = Visibility.Visible;
            duck.Sprite.Visibility = Visibility.Visible;
            score_label.Visibility = Visibility.Visible;
            clock_label.Visibility = Visibility.Visible;

            winning_screen_back_button.Visibility = Visibility.Collapsed;

            back_button_Click(null, null);
        }
    }
}