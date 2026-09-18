using Npgsql;

const string connStr =
    "Host=aws-1-eu-west-1.pooler.supabase.com;Port=5432;Database=postgres;" +
    "Username=postgres.wclxndipmmxoemgrycda;Password=DigiFikile@123;" +
    "SslMode=Require;TrustServerCertificate=true;Pooling=true;Maximum Pool Size=100;" +
    "SearchPath=lms;";

await using var conn = new NpgsqlConnection(connStr);
await conn.OpenAsync();
Console.WriteLine("CONNECTED");

// List tables in lms schema with pg_stat
await using (var cmd = new NpgsqlCommand(
    "SELECT table_name FROM information_schema.tables WHERE table_schema = 'lms' ORDER BY table_name", conn))
await using (var reader = await cmd.ExecuteReaderAsync())
{
    while (await reader.ReadAsync())
        Console.WriteLine("TABLE " + reader.GetString(0));
}

// List columns of all lms tables
await using (var cmd2 = new NpgsqlCommand(
    "SELECT table_name, column_name, data_type, is_nullable FROM information_schema.columns " +
    "WHERE table_schema = 'lms' ORDER BY table_name, ordinal_position", conn))
await using (var reader2 = await cmd2.ExecuteReaderAsync())
{
    while (await reader2.ReadAsync())
        Console.WriteLine($"COL {reader2.GetString(0)}.{reader2.GetString(1)} {reader2.GetString(2)} nullable={reader2.GetString(3)}");
}

// List foreign keys
await using (var cmd3 = new NpgsqlCommand(
    "SELECT tc.table_name, kcu.column_name, ccu.table_name, ccu.column_name FROM information_schema.table_constraints tc " +
    "JOIN information_schema.key_column_usage kcu ON tc.constraint_name = kcu.constraint_name AND tc.table_schema = kcu.table_schema " +
    "JOIN information_schema.constraint_column_usage ccu ON ccu.constraint_name = tc.constraint_name AND ccu.table_schema = tc.table_schema " +
    "WHERE tc.constraint_type = 'FOREIGN KEY' AND tc.table_schema = 'lms' ORDER BY tc.table_name", conn))
await using (var reader3 = await cmd3.ExecuteReaderAsync())
{
    while (await reader3.ReadAsync())
        Console.WriteLine($"FK {reader3.GetString(0)}.{reader3.GetString(1)} -> {reader3.GetString(2)}.{reader3.GetString(3)}");
}

// List indexes
await using (var cmd4 = new NpgsqlCommand(
    "SELECT tablename, indexname FROM pg_indexes WHERE schemaname = 'lms' ORDER BY tablename, indexname", conn))
await using (var reader4 = await cmd4.ExecuteReaderAsync())
{
    while (await reader4.ReadAsync())
        Console.WriteLine($"IDX {reader4.GetString(0)}.{reader4.GetString(1)}");
}

Console.WriteLine("DONE");