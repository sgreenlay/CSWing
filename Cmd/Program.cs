namespace CSWing
{
    class Program
    {
        static void Main(string[] args)
        {
            Helpers.Logger.RunAsync(args[0]).Wait();
        }
    }
}
