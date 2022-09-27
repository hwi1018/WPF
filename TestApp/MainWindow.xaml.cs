using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace TestApp
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Private Variable
        Random random = new Random();
        DispatcherTimer enemyTimer = new DispatcherTimer();
        DispatcherTimer targetTimer = new DispatcherTimer();
        bool IsHumanCaptured = false;

        int score = 0;
        #endregion

        #region Constructor
        public MainWindow()
        {
            InitializeComponent();

            enemyTimer.Tick += EnemyTimer_Tick;
            enemyTimer.Interval = TimeSpan.FromSeconds(2);

            targetTimer.Tick += TargetTimer_Tick;
            targetTimer.Interval = TimeSpan.FromSeconds(.1);
        }

        #endregion

        #region Private Func
        private void StartTheGame()
        {
            human.IsHitTestVisible = true;
            IsHumanCaptured = false;
            progressBar.Value = 0;
            score = 0;
            txtScore.Text = score.ToString();

            btnStart.Visibility = Visibility.Collapsed;
            playArea.Children.Clear();
            playArea.Children.Add(recTarget);
            playArea.Children.Add(human);
            
            enemyTimer.Start();
            targetTimer.Start();
        }

        private void EndTheGame()
        {
            if(!playArea.Children.Contains(txtGameOver))
            {
                score = 0;
                txtScore.Text = score.ToString();

                enemyTimer.Stop();
                targetTimer.Stop();
                IsHumanCaptured = false;
                btnStart.Visibility = Visibility.Visible;
                playArea.Children.Add(txtGameOver);
            }
        }

        private void AnimateEnemy(ContentControl enemy, double from, double to, string propertyToAnimate)
        {
            Storyboard storyboard = new Storyboard()
            {
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever
            };

            DoubleAnimation animation = new DoubleAnimation()
            {
                From=from,
                To=to,
                Duration = new Duration(TimeSpan.FromSeconds(random.Next(4,6)))
            };

            Storyboard.SetTarget(animation, enemy);
            Storyboard.SetTargetProperty(animation, new PropertyPath(propertyToAnimate));
            storyboard.Children.Add(animation);
            storyboard.Begin();
        }

        private void AddEnemy()
        {
            ContentControl enemy = new ContentControl();
            enemy.Template = Resources["EnemyTemplate"] as ControlTemplate;
            AnimateEnemy(enemy, 0, playArea.ActualWidth - 100, "(Canvas.Left)");
            AnimateEnemy(enemy, random.Next((int)playArea.ActualHeight - 100), random.Next((int)playArea.ActualHeight-100), "(Canvas.Top)");
            playArea.Children.Add(enemy);

            enemy.MouseEnter += Enemy_MouseEnter;
        }

     
        #endregion

        #region Event Func
        private void TargetTimer_Tick(object sender, EventArgs e)
        {
            progressBar.Value += 1;
            if(progressBar.Value >= progressBar.Maximum)
            {
                EndTheGame();
            }
        }

        private void EnemyTimer_Tick(object sender, EventArgs e)
        {
            AddEnemy();
        }

        private void Enemy_MouseEnter(object sender, MouseEventArgs e)
        {
            if (IsHumanCaptured)
            {
                EndTheGame();
            }
        }
        private void human_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(enemyTimer.IsEnabled)
            {
                IsHumanCaptured = true;
                human.IsHitTestVisible = false;
            }
        }
        private void recTarget_MouseEnter(object sender, MouseEventArgs e)
        {
            if(targetTimer.IsEnabled && IsHumanCaptured)
            {
                score += 50;
                txtScore.Text = score.ToString();

                progressBar.Value = 0;
                Canvas.SetLeft(recTarget, random.Next(100, (int)playArea.ActualWidth - 100));
                Canvas.SetTop(recTarget, random.Next(100, (int)playArea.ActualHeight - 100));
                Canvas.SetLeft(human, random.Next(100, (int)playArea.ActualWidth - 100));
                Canvas.SetTop(human, random.Next(100, (int)playArea.ActualHeight - 100));
                IsHumanCaptured = false;
                human.IsHitTestVisible = true;
            }
        }
        private void playArea_MouseMove(object sender, MouseEventArgs e)
        {
            if(IsHumanCaptured)
            {
                Point pointerPosition = e.GetPosition(null);
                Point relativePosition = gridMain.TransformToVisual(playArea).Transform(pointerPosition);
                if((Math.Abs(relativePosition.X - Canvas.GetLeft(human)) > human.ActualWidth*3) ||
                   (Math.Abs(relativePosition.Y - Canvas.GetTop(human)) > human.ActualHeight*3))
                {
                    IsHumanCaptured = false;
                    human.IsHitTestVisible = true;
                }
                else
                {
                    Canvas.SetLeft(human, relativePosition.X - human.ActualWidth / 2);
                    Canvas.SetTop(human, relativePosition.Y - human.ActualHeight / 2);
                }
            }
        }

        private void playArea_MouseLeave(object sender, MouseEventArgs e)
        {
            if (IsHumanCaptured)
            {
                EndTheGame();
            }
        }
        #endregion

        #region Main Event
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            StartTheGame();
        }

       
        #endregion
    }
}
