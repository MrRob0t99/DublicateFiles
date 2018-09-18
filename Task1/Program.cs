using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace SearchSameFiles
{
    class Program


    {
        private static readonly List<FileInfo> _files = new List<FileInfo>();
        private static readonly List<FileInfo> _addedFiles = new List<FileInfo>();

        static void Main(string[] args)
        {
            var stopwatch = new Stopwatch();
            Console.WriteLine("Enter the path to the directory");
            var directory = Console.ReadLine();
            if (!Directory.Exists(directory))
            {
                Console.WriteLine("Directory not found");
            }

            else
            {
                stopwatch.Start();
                var directoryInfo = new DirectoryInfo(directory);
                GetAllFileFromDirectory(directoryInfo);
                var filesGroup = _files.GroupBy(f => f.Length);
                foreach (var g in filesGroup)
                {
                    if (g.Select(p => p).Skip(1).Any())
                    {
                        var list = GetName(g.Select(p => p).ToList());
                        foreach (var item in list)
                        {
                            Console.WriteLine("Same _files:");
                            foreach (var elem in item)
                                Console.WriteLine(elem.FullName);
                        }
                    }
                }
                stopwatch.Stop();
                Console.WriteLine(stopwatch.Elapsed);
                Console.ReadKey();
            }
        }

        static List<List<FileInfo>> GetName(List<FileInfo> fileInfos)
        {
            var fileList = new List<List<FileInfo>>();
            for (int i = 0; i < fileInfos.Count; i++)
            {
                for (int j = 0; j < fileInfos.Count; j++)
                {
                    if (i != j && !_addedFiles.Contains(fileInfos[i]) && IsEqualFiles(fileInfos[i], fileInfos[j]))
                    {
                        var list = fileList.FirstOrDefault(l => l.Contains(fileInfos[i]) || l.Contains(fileInfos[j]));
                        if (list != null)
                        {
                            list.Add(fileInfos[i]);
                        }
                        else
                        {
                            fileList.Add(new List<FileInfo>()
                            {
                                fileInfos[i],
                                fileInfos[j]
                            });
                        }
                        _addedFiles.Add(fileInfos[i]);
                        _addedFiles.Add(fileInfos[j]);
                    }
                }
            }
            return fileList;
        }

        static void GetAllFileFromDirectory(DirectoryInfo directoryInfo)
        {
            Console.WriteLine(directoryInfo.FullName);
            _files.AddRange(directoryInfo.GetFiles());
            if (directoryInfo.GetDirectories().Any())
                foreach (var dir in directoryInfo.GetDirectories())
                {
                    GetAllFileFromDirectory(dir);
                }
        }

        static bool IsEqualFiles(FileInfo first, FileInfo second)
        {
            var firstHash = MD5.Create().ComputeHash(first.OpenRead());
            var secondHash = MD5.Create().ComputeHash(second.OpenRead());
            for (int i = 0; i < firstHash.Length; i++)
                if (firstHash[i] != secondHash[i])
                    return false;
            return true;
        }
    }
}
