using System;

namespace ONCGM.VR.VREnums {
    #region Game & Settings
    /// <summary>
    /// Defines the type of leaning input to be used.
    /// </summary>
    public enum InputDirection {
        Forward,
        Left,
        Right,
        Backward,
        Centered,
        InvalidDirection
    }
    
    /// <summary>
    /// Extends the enum to return translated strings for UI use.
    /// </summary>
    public static class InputDirectionExtension {
        public static string ToString(this InputDirection self) {
            // ReSharper disable once ConvertSwitchStatementToSwitchExpression
            switch(self) {
                case InputDirection.Forward:
                    return "Frente";
                case InputDirection.Left:
                    return "Esquerda";
                case InputDirection.Right:
                    return "Direita";
                case InputDirection.Backward:
                    return "Atras";
                case InputDirection.Centered:
                    return "Centralizado";
                case InputDirection.InvalidDirection:
                    return "Inválido";
                default:
                    return "Inválido";
            }
        }
    }
    
    /// <summary>
    /// Defines the spawn lanes to be used.  Catch Game Enum.
    /// </summary>
    public enum SpawnDirection {
        Up,
        Down,
        Left,
        Right,
        UpAndDown,
        LeftAndRight,
        UpAndLeft,
        UpAndRight,
        DownAndLeft,
        DownAndRight,
        All
    }
    
    /// <summary>
    /// Extends the enum to return translated strings for UI use.  Catch Game Enum.
    /// </summary>
    public static class SpawnDirectionCatchGameExtension {
        public static string ToString(this SpawnDirection self) {
            // ReSharper disable once ConvertSwitchStatementToSwitchExpression
            switch(self) {
                case SpawnDirection.Up:
                    return "Cima";
                case SpawnDirection.Down:
                    return "Baixo";
                case SpawnDirection.Left:
                    return "Esquerda";
                case SpawnDirection.Right:
                    return "Direita";
                case SpawnDirection.All:
                    return "Todas";
                case SpawnDirection.UpAndDown:
                    return "Cima e Baixo";
                case SpawnDirection.LeftAndRight:
                    return "Esquerda e Direita";
                case SpawnDirection.UpAndLeft:
                    return "Cima e Esquerda";
                case SpawnDirection.UpAndRight:
                    return "Cima e Direita";
                case SpawnDirection.DownAndLeft:
                    return "Baixo e Esquerda";
                case SpawnDirection.DownAndRight:
                    return "Baixo e Direita";
                default:
                    return "Inválido";
            }
        }
    }
    
    /// <summary>
    /// Defines the objects to be used. Catch Game Enum.
    /// </summary>
    public enum SpawnObjectsCatchGame {
        Butterfly,
        Helicopter,
        All
    }
    
    /// <summary>
    /// Extends the enum to return translated strings for UI use. Catch Game Enum.
    /// </summary>
    public static class SpawnObjectsCatchGameExtension {
        public static string ToString(this SpawnObjectsCatchGame self) {
            // ReSharper disable once ConvertSwitchStatementToSwitchExpression
            switch(self) {
                case SpawnObjectsCatchGame.Butterfly:
                    return "Borboleta";
                case SpawnObjectsCatchGame.Helicopter:
                    return "Helicóptero";
                case SpawnObjectsCatchGame.All:
                    return "Todos";
                default:
                    return "Inválido";
            }
        }
    }
    
    /// <summary>
    /// Defines the game scenes by build index.
    /// </summary>
    public enum GameScene {
        Calibration,
        MainMenu,
        GameSetup,
        CatchGame,
        ColorsGame,
        FlyingGame,
        InvalidScene
    }
    
    /// <summary>
    /// Defines the game and minigames difficulty.
    /// </summary>
    public enum GameDifficulty {
        VeryEasy,
        Easy,
        Normal,
        Hard,
        Expert,
        Impossible
    }
    
    /// <summary>
    /// Extends the enum to return translated strings for UI use.
    /// </summary>
    public static class GameDifficultyExtension {
        public static string ToString(this GameDifficulty self) {
            switch(self) {
                case GameDifficulty.VeryEasy:
                    return "Muito Fácil";
                case GameDifficulty.Easy:
                    return "Fácil";
                case GameDifficulty.Normal:
                    return "Normal";
                case GameDifficulty.Hard:
                    return "Difícil";
                case GameDifficulty.Expert:
                    return "Perito";
                case GameDifficulty.Impossible:
                    return "Impossível";
                default:
                    return "Não Encontrado";
            }
        }
    }

    /// <summary>
    /// Used to select a clip to be played in the UI audio handler.
    /// </summary>
    public enum UiAudioClips {
        Click,
        ClickOut,
        StartGame,
        EndGame,
        Success,
        Miss,
        SaveSuccessful,
        None
    }
    
    #endregion
    
    #region Minigames
    
    /// <summary>
    /// Defines the minigames that the game will contain.
    /// </summary>
    public enum Minigames {
        CatchGame,
        ColorsGame,
        FlyingGame,
        CatchAndColors,
        CatchAndFlying,
        ColorsAndFlying,
        All,
        Invalid
    }
    
    /// <summary>
    /// Extends the enum to return translated strings for UI use.
    /// </summary>
    public static class MinigamesExtension {
        public static string ToString(this Minigames self) {
            switch(self) {
                case Minigames.CatchGame:
                    return "Pegar objetos";
                case Minigames.ColorsGame:
                    return "Cores";
                case Minigames.FlyingGame:
                    return "Voo";
                case Minigames.CatchAndColors:
                    return "Objetos e Cores";
                case Minigames.CatchAndFlying:
                    return "Objetos e Voo";
                case Minigames.ColorsAndFlying:
                    return "Cores e Voo";
                case Minigames.All:
                    return "Todos";
                case Minigames.Invalid:
                    return "Inválido";
                default:
                    return "Não Encontrado";
            }
        }
    }
    
    #endregion
}