using Neo4j.Driver;

namespace YSNeo4jDotnet
{
    internal class SessionManager
    {
        public static IAsyncSession GetSession()
        {
            return GraphDatabase.Driver("bolt://localhost:7687", AuthTokens.Basic("neo4j", "neo4j1234")).AsyncSession();
        }
    }
}
