class Cell
{
    public Cell() { }
    public Cell(int posx, int posy, char icon, ConsoleColor color = ConsoleColor.Red)
    {
        posX = posx;
        posY = posy;
        Icon = icon;
        Color = color;
    }

    public void Draw()
    {
        Console.SetCursorPosition(posX, posY);
        Console.ForegroundColor = Color;
        Console.Write(Icon);
    }

    public void Clear()
    {
        Console.SetCursorPosition(posX, posY);
        Console.Write(' ');
    }

    public char Icon;
    public int posX;
    public int posY;
    public ConsoleColor Color { get; private set; } = ConsoleColor.Gray;
}

static class Eat
{
    public static void Place(int WIDTH, int HEIGHT)
    {
        Clear();
        posX = rnd.Next(0, WIDTH);
        posY = rnd.Next(0, HEIGHT);
        Draw();
    }

    public static void Clear()
    {
        Console.SetCursorPosition(posX, posY); Console.Write(" ");
    }

    public static void Draw()
    {
        Console.SetCursorPosition(posX, posY); Console.Write("@");
    }

    static private Random rnd = new Random();
    static public int posX = 0;
    static public int posY = 0;
}

class Snake
{
    public Snake(int posX, int posY, int bodyLen = 3)
    {
        Head = new Cell(posX, posY, '*', ConsoleColor.Blue);
        

        for (int i = bodyLen; i >= 0; i--)
            Body.Enqueue(new Cell(Head.posX - i - 1, posY, BodyShape, bodyColor));
        Draw();
        Eat.Place(this.Head.posX + 3, this.Head.posY + 3); //3 - просто число из головы, мне просто так захотелось
    }

    public void Move(Direction direction)
    {
        Clear();

        Body.Enqueue(new Cell(Head.posX, Head.posY, BodyShape, bodyColor));
        Body.Dequeue();

        Head = direction switch
        {
            Direction.Left => new Cell(Head.posX + 1, Head.posY, HeadShape, ConsoleColor.Blue),
            Direction.Right => new Cell(Head.posX - 1, Head.posY, HeadShape, ConsoleColor.Blue),
            Direction.Up => new Cell(Head.posX, Head.posY - 1, HeadShape, ConsoleColor.Blue),
            Direction.Down => new Cell(Head.posX, Head.posY + 1, HeadShape, ConsoleColor.Blue),
            _ => Head
        };

        if (Head.posX == Eat.posX && Head.posY == Eat.posY)
        {
            Body.Enqueue(new Cell(this.Head.posX, this.Head.posY, '&', bodyColor));
            Eat.Place(this.Head.posX + 20, this.Head.posY + 20); //20 - просто число из головы, мне просто так захотелось
            SnakeGame.SnakeSpeed -= 10;
        }

        Draw();
    }

    public Direction ReadInput(Direction CurrentDirection)
    {
        if (!Console.KeyAvailable)
            return CurrentDirection;

        CurrentDirection = Console.ReadKey(true).Key switch
        {
            ConsoleKey.W when CurrentDirection != Direction.Down => CurrentDirection = Direction.Up,
            ConsoleKey.A when CurrentDirection != Direction.Left => CurrentDirection = Direction.Right,
            ConsoleKey.S when CurrentDirection != Direction.Up => CurrentDirection = Direction.Down,
            ConsoleKey.D when CurrentDirection != Direction.Right => CurrentDirection = Direction.Left,
            _ => CurrentDirection
        };
        return CurrentDirection;
    }

    public void Draw()
    {
        try
        {
            Head.Draw();
            foreach (Cell shape in Body)
            {
                shape.Draw();
            }
            Eat.Draw();
        }
        catch
        {
            Console.Clear();
            Console.SetCursorPosition(Console.WindowWidth / 2, Console.WindowHeight / 2);
            Console.Write($"Вы разбились!");
            Console.SetCursorPosition(Console.WindowWidth / 2, Console.WindowHeight / 2 + 1);
            Console.Write($"Длина змейки: {Body.Count}");
            Thread.Sleep(1500);
        }
    }
    private void Clear()
    {
        Head.Clear();
        foreach (Cell shape in Body)
        {
            shape.Clear();
        }
    }
    private Cell Head;
    private Queue<Cell> Body = new Queue<Cell>();
    private char BodyShape = '#';
    private char HeadShape = '+';
    private ConsoleColor bodyColor = ConsoleColor.Green;
}

enum Direction
{
    Up,
    Down,
    Left,
    Right
}

static class SnakeGame
{
    public static void Main()
    {
        Console.CursorVisible = false;
        Snake snake = new Snake(20, 10, 10);
        Direction CurrentDirection = Direction.Left;
        while (true)
        {
            try
            {
                Thread.Sleep(SnakeSpeed);
                CurrentDirection = snake.ReadInput(CurrentDirection);
                snake.Move(CurrentDirection);
            }
            catch
            {
                Console.Clear();
                snake = new Snake(20, 10, 5);

            }
        }
    }

    public static int SnakeSpeed = 300;
}