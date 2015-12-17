namespace CSWing
{
    class Program
    {
        static void Main(string[] args)
        {
            SellSide.RunAsync(args[0]).Wait();
        }
    }
}
