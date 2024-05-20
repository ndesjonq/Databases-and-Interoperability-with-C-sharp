using System.Collections.Generic;

namespace DuckHunt
{
    class Score
    {
        private Dictionary<string, int> leaderboard;
        private List<int> scores;

        public List<int> Scores { get => scores; set => scores = value; }
        public Dictionary<string, int> Leaderboard { get => leaderboard; set => leaderboard = value; }

        public Score()
        {
            // imports the leaderboard and sets the scores to 0
        }

        public void UpdateLeaderboard()
        {
            // updates the leaderboard for example at the end of a game and saves it to txt or database with the function below
        }

        public void ImportLeaderboard()
        {
            // import the leaderboard from a txt file or a database
        }

        public void ExportLeaderboard()
        {
            // exports the leaderboard to a txt file or database
        }

        public void HighScoreCelebration()
        {
            // do a little animation when a new high score is done
        }
    }
}