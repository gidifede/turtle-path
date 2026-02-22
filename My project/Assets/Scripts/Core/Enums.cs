namespace TurtlePath.Core
{
    public enum TileType
    {
        Straight,
        Curve,
        T
    }

    public enum CellType
    {
        Normal,
        Rock,
        Hole,
        Nest,
        Sea
    }

    public enum GameState
    {
        MainMenu,
        LevelSelect,
        Editing,
        Completed,
        Animating,
        Result
    }

    public enum CollectibleType
    {
        Shell,
        BabyTurtle
    }
}
