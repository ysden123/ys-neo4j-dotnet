using Neo4j.Driver;
using System.Runtime.InteropServices;

namespace YSNeo4jDotnet
{
    public class Program
    {
        public static void Main()
        {
            Console.WriteLine("Start");
            ClearDB();
            Console.WriteLine("After ClearDB");
            var result = ReadGreeting().GetAwaiter().GetResult();
            Console.WriteLine($"result: {result}");
            AddGreetings().GetAwaiter().GetResult();
            Console.WriteLine("After AddGreetings");

            TestTaskLoop().GetAwaiter().GetResult();
        }

        static async void ClearDB()
        {
            Console.WriteLine("==>ClearDB");
            Task.Delay(1000).Wait();
            await using var session = SessionManager.GetSession();
            await session.ExecuteWriteAsync(
                async tx =>
                {
                    await tx.RunAsync(
                        "MATCH(a:Greeting) DETACH DELETE(a)"
                        );
                }
                );
        }

        static async Task<string> ReadGreeting()
        {
            Console.WriteLine("==>ReadGreeting");
            await using var session = SessionManager.GetSession();
            var message = "It is a message";
            var greeting = await session.ExecuteWriteAsync(
            async tx =>
            {
                var result = await tx.RunAsync(
                    "CREATE (a:Greeting) " +
                    "SET a.message = $message " +
                    "RETURN a.message + ', from node ' + id(a)",
                    new { message });

                var record = await result.SingleAsync();
                return record[0].As<string>();
            });
            return greeting;
        }

        static async Task AddGreetings()
        {
            Console.WriteLine("==>AddGreetings");
            await using var session = SessionManager.GetSession();
            string[] messages = ["message 1", "message 2", "message 3", "message 4"];
            foreach (var message in messages)
            {
                Console.WriteLine($"Adding {message}");
                var message2 = message + " 2";
                await session.ExecuteWriteAsync(
                async tx =>
                {
                    {
                        var sql = "CREATE (a:Greeting) "
                        + "SET a.message = $message"
                        + ", a.message2 = $message2";
                        await tx.RunAsync(
                            sql, new { message, message2 }
                            );
                    }
                }
                );
            }
        }

        static async Task TestTaskLoop()
        {
            Console.WriteLine("==>TestTaskLoop");
            string[] messages = ["message 1", "message 2", "message 3", "message 4"];
            foreach (var message in messages)
            {
                await Task.Delay(500);
                await Task.Run(() =>
                {
                    Console.WriteLine($"{message}");
                });

            }
        }
    }
}