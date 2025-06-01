using System.Collections.Generic;

namespace Assets.GracesScripts
{
    public static class GameState
    {
        public const string NewGame = "NewGame";
        public const string BattleLost = "BattleLost";
        public const string BattleWon = "BattleWon";
        public const string LoadingSave = "LoadingSave";
        public const string RegularSceneChange = "RegularSceneChange";
        public const string QuittingToTitle = "QuittingToTitle";
        public const string BattleRunAwaySuccess = "BattleRunAwaySuccess";
        public const string StartedBattle = "StartedBattle";

        public static List<string> GameStates => new() { NewGame, BattleLost, BattleWon, LoadingSave, RegularSceneChange, QuittingToTitle };
    }
}
