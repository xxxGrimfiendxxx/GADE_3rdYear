using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
public class ConsoleApp1 : MonoBehaviour
{
    // Define the edge table size
    const int rows = 256;
    const int columns = 16;

    // Define the edge table array
    static readonly int[,] edgeTable = new int[rows, columns];

    // Start is called before the first frame update
    void Start()
    {
        // Initialize the edge table with sample data
        InitializeEdgeTable();

        // Write the edge table to a file
        WriteEdgeTableToFile("EdgeTable.txt");

        Debug.Log("Edge table has been written to EdgeTable.txt.");
    }

    void InitializeEdgeTable()
    {
        // Initialize with sample data or extend with your actual data
        System.Random rand = new System.Random();
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                // Replace this with actual data or extend accordingly
                edgeTable[i, j] = rand.Next(0x0000, 0xFFFF);
            }
        }
    }

    void WriteEdgeTableToFile(string fileName)
    {
        string path = Path.Combine(Application.dataPath, fileName);
        using (StreamWriter writer = new StreamWriter(path))
        {
            for (int i = 0; i < rows; i++)
            {
                writer.Write( " { ");
                for (int j = 0; j < columns; j++)
                {
                    writer.Write($"0x{edgeTable[i, j]:X4}, ");
                }
                writer.WriteLine();
            }
        }
    }
}