using Unvoxel;
using Unvoxel.GameLoop;

class Program
{
    static void Main(string[] args)
    {
        Game game = new TestGame(800, 600, "Unvoxel Engine • Test Program");
        game.Run();
    }
}