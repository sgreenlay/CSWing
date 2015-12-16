namespace CSWing
{
    class Program
    {
        static void Main(string[] args)
        {
            ChockABlock.RunAsync(args[0]).Wait();
        }
    }
}
