using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class TreeSystem : MonoBehaviour
{

    List<Folder> tempfolders = new List<Folder>();
    private Dictionary<string, Folder> folderByPath = new Dictionary<string, Folder>();

    void Start()
    {
        var home = new List<string>()
            {
                "home/sports/futbol/world cup/Brazil|America",
                "home/sports/football",
                "home/music/metal/iron maiden",
            };

        var music = new List<string>()
            {
                //"home/music/rap|rock|pop",
                "home/sports|music/misc|favorites",
            };

        var folders3 = GetFoldersStrings(home);
        var folders2 = GetFoldersStringsPowerSet(music);
        ShowFolders(folders2);
        Debug.Log(" ");
        ShowFolders(folders3);
    }


    #region single
    static List<Folder> GetFoldersStrings(List<string> strings)
    {
        var folders = new List<Folder>();
        //strings.Sort(StringComparer.InvariantCultureIgnoreCase);
        var folderByPath = new Dictionary<string, Folder>();
        foreach (var str in strings)
        {
            string buildPath = null;
            var splitedStrings = SplitString(str);
            foreach (var splitStr in splitedStrings)
            {
                if (splitStr.EndsWith("/"))
                {
                    buildPath += splitStr;
                    CreateFolder(folders, folderByPath, buildPath);
                }
                else
                {
                    var tempString = buildPath + splitStr;
                    CreateFolder(folders, folderByPath, tempString);
                }
            }
        }
        return folders;
    }

    private static Folder CreateFolder(List<Folder> rootFolders, Dictionary<string, Folder> folderByPath, string folderPath)
    {
        if (!folderByPath.TryGetValue(folderPath, out var folder))
        {

            var folderPathWithoutEndSlash = folderPath.TrimEnd('/', '|');
            var lastSlashPosition = folderPathWithoutEndSlash.LastIndexOf("/");
            List<Folder> folders;
            string folderName;
            if (lastSlashPosition < 0) // it's a first level folder
            {
                folderName = folderPathWithoutEndSlash;
                folders = rootFolders;
            }
            else
            {
                var parentFolderPath = folderPath.Substring(0, lastSlashPosition + 1);
                folders = folderByPath[parentFolderPath].Folders;
                folderName = folderPathWithoutEndSlash.Substring(lastSlashPosition + 1);
            }
            folder = new Folder
            {
                Name = folderName
            };
            folders.Add(folder);
            folderByPath.Add(folderPath, folder);
        }
        return folder;
    }

    #endregion


    #region PowerSet
    List<Folder> GetFoldersStringsPowerSet(List<string> strings)
    {

        foreach (var str in strings)
        {
            string buildPath = null;
            var splitedStrings = SplitString(str);
            foreach (var splitStr in splitedStrings)
            {
                if (splitStr.EndsWith("/"))
                {
                    buildPath += splitStr;
                    CreateFolderPowerSet(tempfolders, folderByPath, buildPath);
                }
                else
                {
                    var tempString = buildPath + splitStr;
                    CreateFolderPowerSet(tempfolders, folderByPath, tempString);
                }
            }
        }

        PowerSetFolders(tempfolders);

        CreateRemainsfolder(tempfolders);



        return tempfolders;
    }


    void CreateRemainsfolder(List<Folder> folders)
    {

        List<Folder> tempList = new List<Folder>();
        foreach (var subfolder in folders)
        {
            if (folders.Count <= 1)
            {
                CreateRemainsfolder(subfolder.Folders);
            }
            else
            {
                if (subfolder.Folders.Count > 0)
                {
                    tempList = subfolder.Folders;
                }
            }
        }
        if (tempList.Count > 0)
        {
            foreach (var subfolder in folders)
            {
                if (!subfolder.Folders.Count.Equals(tempList.Count))
                {

                    subfolder.Folders = tempList;
                }
            }
        }
    }


    private void PowerSetFolders(List<Folder> folders)
    {
        PowerSetFolder(folders);
    }

    private void PowerSetFolder(List<Folder> folders)
    {
        foreach (var subfolder in folders)
        {
            PowerSetFolder(subfolder.Folders);
            if (subfolder.Folders.Count > 1)
            {
                List<string> stringPowersetList = new List<string>();

                foreach (var i in subfolder.Folders)
                {
                    stringPowersetList.Add(i.Name);

                }

                foreach (var item in GetPowerset(stringPowersetList))
                {
                    var buildPath = subfolder.PathString + item;
                    CreateFolderPowerSet(folders, folderByPath, buildPath);
                }
            }
        }
    }


    static List<string> GetPowerset(List<string> list)
    {
        var templist = new List<string>();
        for (int i = 0; i < (1 << list.Count); i++)
        {
            var sublist = new List<string>();
            string temp = null;
            for (int j = 0; j < list.Count; j++)
            {
                if ((i & (1 << j)) != 0)
                {
                    sublist.Add(list[j]);

                }

            }
            if (sublist.Count > 1)
            {
                foreach (var item in sublist)
                {
                    if (temp == null)
                        temp = item;
                    else
                        temp += "-" + item;
                }
                if (temp != null)
                {
                    templist.Add(temp);
                    temp = null;
                }
            }

        }

        return templist;

    }

    private static Folder CreateFolderPowerSet(List<Folder> rootFolders, Dictionary<string, Folder> folderByPath, string folderPath)
    {
        if (!folderByPath.TryGetValue(folderPath, out var folder))
        {

            var folderPathWithoutEndSlash = folderPath.TrimEnd('/', '|');
            var lastSlashPosition = folderPathWithoutEndSlash.LastIndexOf("/");
            List<Folder> folders;
            string folderName;
            if (lastSlashPosition < 0) // it's a first level folder
            {
                folderName = folderPathWithoutEndSlash;
                folders = rootFolders;
            }
            else
            {
                var parentFolderPath = folderPath.Substring(0, lastSlashPosition + 1);
                folders = folderByPath[parentFolderPath].Folders;
                folderName = folderPathWithoutEndSlash.Substring(lastSlashPosition + 1);
            }
            folder = new Folder
            {
                Name = folderName,
                PathString = folderPath
            };
            folders.Add(folder);
            folderByPath.Add(folderPath, folder);
        }
        return folder;
    }

    #endregion


    #region utils
    private static void ShowFolders(List<Folder> folders)
    {
        foreach (var folder in folders)
        {
            ShowFolder(folder, 0);
        }
    }

    private static void ShowFolder(Folder folder, int indentation)
    {
        string folderIndentation = new string('-', indentation);
        Debug.Log($"{folderIndentation}-{folder.Name}");
        foreach (var subfolder in folder.Folders)
        {
            ShowFolder(subfolder, indentation + 1);
        }
    }


    private static string[] SplitString(string str)
    {
        var value = Regex.Split(str, @"(?<=[/|])");

        return value;
    }

    #endregion
}


public class Folder
{
    public string Name { get; set; }
    public string PathString { get; set; }
    public List<Folder> Folders { get; set; } = new List<Folder>();
}
