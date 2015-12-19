namespace CSWing
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger.RunAsync(args[0]).Wait();
        }
    }
}
