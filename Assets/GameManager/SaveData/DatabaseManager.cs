
using Mono.Data.Sqlite;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    private string dbPath;

    private void Start()
    {
        dbPath = "URI=file:" + Application.persistentDataPath + "/GameData.db";

        InitializeDatabase();

    }
    private void InitializeDatabase()
    {
        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();


            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                CREATE TABLE IF NOT EXISTS PlayerPosition(
                    ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    PosX REAL,
                    PosY REAL,
                    PosZ REAL
                )";
                command.ExecuteNonQuery();
            }

        }
    }

    public void SavePlayerPosition(Vector3 position)
    {
        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();

            using(var command = connection.CreateCommand())
            {
                command.CommandText = "INSERT INTO PlayerPosition (PosX, PosY, PosZ) VALUES(@x, @y, @z)";
                command.Parameters.Add(new SqliteParameter("@x", position.x));
                command.Parameters.Add(new SqliteParameter("@y", position.y));
                command.Parameters.Add(new SqliteParameter("@z", position.z));

                command.ExecuteNonQuery();
            }
        }
    }

    public Vector3 LoadPlayerPosition()
    {
        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT PosX, PosY, PosZ FROM PlayerPosition ORDER BY ID DESC LIMIT 1";

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        float x = reader.GetFloat(0);
                        float y = reader.GetFloat(1);
                        float z = reader.GetFloat(2);

                        return new Vector3(x, y, z);
                    }
                }
            }
        }
        return Vector3.zero;
    }

}
