using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System.Text;
using static ProxyDBX.SQLParser.QueryRewriter;

namespace ProxyDBX.SQLParser;
public class SqlOptimizer
{
    public static string OptimizeQuery(string sql)
    {
        //try
        //{
        //    var inputStream = new AntlrInputStream(sql);
        //    var lexer = new SQLiteLexer(inputStream);
        //    var tokenStream = new CommonTokenStream(lexer);
        //    tokenStream.Fill();
        //    foreach (var token in tokenStream.GetTokens())
        //    {
        //        Console.WriteLine($"Token: {token.Text}");
        //    }
        //    var parser = new SQLiteParser(tokenStream);



        //    var tree = parser.select_stmt();

        //    var optimizer = new QueryRewriter();
        //    return optimizer.Visit(tree);
        //}
        //catch (Exception e)
        //{

        //    throw;
        //
        AntlrInputStream inputStream = new AntlrInputStream(sql);
        SQLiteLexer lexer = new SQLiteLexer(inputStream);
        CommonTokenStream tokenStream = new CommonTokenStream(lexer);
        SQLiteParser parser = new SQLiteParser(tokenStream);

        ParseTreeWalker walker = new ParseTreeWalker();
        SqlQueryRewriter rewriter = new SqlQueryRewriter();
        walker.Walk(rewriter, parser.parse());

        // Print modified query
        Console.WriteLine("Rewritten Query: " + rewriter.GetModifiedQuery());
        return "";
    }
}

public class QueryRewriter : SQLiteParserBaseVisitor<dynamic>
{
    //public override string VisitSelect_core(SQLiteParser.Select_coreContext context)
    //{
    //    Console.WriteLine("Visiting Select_core...");

    //    var resultColumns = context.result_column();
    //    bool hasSelectAll = resultColumns.Any(col => col.GetText() == "*");

    //    if (hasSelectAll)
    //    {
    //        var tableName = context.table_or_subquery(0).GetText();
    //        string newQuery = $"SELECT id, name FROM {tableName}"; // Replace `SELECT *`

    //        Console.WriteLine($"Modified Query: {newQuery}");
    //        return newQuery;
    //    }

    //    return base.VisitSelect_core(context);
    //}

    public class SqlQueryRewriter : SQLiteParserBaseListener
    {
        private StringBuilder modifiedQuery = new StringBuilder();
        private bool modified = false;

        public string GetModifiedQuery() => modified ? modifiedQuery.ToString() : "No changes made.";

        public override void EnterSql_stmt(SQLiteParser.Sql_stmtContext context)
        {
            var selectStmt = context.select_stmt();
            if (selectStmt != null)
            {
                var selectCore = selectStmt.select_core(0);
                if (selectCore != null && selectCore.result_column().Length > 0)
                {
                    if (selectCore.result_column(0).GetText() == "*")
                    {
                        string tableName = selectCore.table_or_subquery(0).GetText();
                        modifiedQuery.Append($"SELECT id, name FROM {tableName};");
                        modified = true;
                    }
                }
            }
        }
    }
}

