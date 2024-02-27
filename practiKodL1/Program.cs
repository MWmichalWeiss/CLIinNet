using System;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using System.Diagnostics.SymbolStore;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Generic;
using System.Diagnostics;

List<string> files = new List<string>();
var rootCommand = new RootCommand("root command for fils bundler CLI");
var bundleCommand = new Command("bundle", "bundle cod fils to a single file");
var create_rspCommand = new Command("create_rsp", "");
var languageOption = new Option<string>("--lang", "all laguage to choise");
var outputFiles = new Option<FileInfo>("--output", "output all the files in one file");
var addNote = new Option<bool>("--note", "to add onte with details");
var sortFiles = new Option<bool>("--sort", "sort the file to copy");
var remove_empty_lines = new Option<bool>("--r-e-l", "remove empty lines from file");
var authorOption = new Option<string>("--author", "to wirte name of auther in bundle file");

bundleCommand.AddOption(languageOption);
bundleCommand.AddOption(outputFiles);
bundleCommand.AddOption(addNote);
bundleCommand.AddOption(sortFiles);
bundleCommand.AddOption(remove_empty_lines);
bundleCommand.AddOption(authorOption);
bundleCommand.SetHandler((language, output, note,sort,rel,aouthor) =>
{


    bool f = false;
    string content = "";
    var l = language.ToString();
    var languageList = l.Split(",").ToList();
    Console.WriteLine("language: " + l);
    if (l == "all")
    {
        files.AddRange(Directory.GetFiles(".", "*"));
    }
    else
    {
        try {
            foreach (var lang in languageList)
            {
                Console.Write("lang ," + lang);
                Console.WriteLine();
                files.AddRange(Directory.GetFiles(".", "*." + lang));
            }
        }catch(Exception ex) { Console.WriteLine("ERROR: the language is not exsit in this folder"); }
        
    }


    //יצירת רשימה של זוגות(שם שפה, סוג קובץ)
    if (sort)
    {
        // Sort files by file type
        files.Sort((file1, file2) =>
        {
            var fileType1 = Path.GetExtension(file1);
            var fileType2 = Path.GetExtension(file2);
            return string.Compare(fileType1, fileType2);
        });

    }



    foreach (var file in files)
    {
        if (rel)
        {
            // קריאת הקובץ
            string content2 = File.ReadAllText(file);

            // מיון הקבצים
            string[] lines = content2.Split('\n');
            var filteredLines = lines.Where(line => line.Trim().Length > 0);

            // החלפת השורות הממוינות בקובץ
            File.WriteAllText(file, string.Join("\n", filteredLines));
            content2 = File.ReadAllText(file);
            //Console.WriteLine("content: " + content2);
        }
        // יוצר אובייקט של סוג `StreamReader`
        using (var reader = new StreamReader(file))
        {
            // קורא את התוכן של הקובץ
            content += reader.ReadToEnd();
            //Console.WriteLine("content: " + content);
            if (output != null)
            {
                string filePath = output.FullName;
                Console.WriteLine("the path: " + filePath);
                //Create or overwrite the file
                try
                {
                    using (StreamWriter writer = new StreamWriter(filePath, append: true))
                    {
                        //note-----------------
                        if (note)
                        {
                            string currentDirectoryNote = Environment.CurrentDirectory;
                            writer.Write("מקור: " + currentDirectoryNote + "\n name of file is: " + file + "\n");
                        }
                        if (aouthor != null && !f)
                        {
                            f = true;
                            string nameAouthor = aouthor.ToString();
                            Console.WriteLine("שם המחבר: " + nameAouthor);
                            writer.Write("name of aouther: " + nameAouthor);
                        }
                        writer.Write(content);

                    }
                }
                catch (Exception ex) { Console.WriteLine("ERROR: the directory invalid"); }
            }
        }
    }
    //מספר הקבצים שנקלטו
    Console.WriteLine("count: " + files.Count());

    //הצגה של כל הקבצים שנקלטו
    Console.Write("files: ");
    foreach (var item in files)
    {
        Console.WriteLine(item + " ,");
    }
    Console.WriteLine("hello worlde***************************");


}, languageOption, outputFiles, addNote,sortFiles,remove_empty_lines,authorOption);

create_rspCommand.SetHandler(() =>
{
    string command = Environment.CurrentDirectory+">practikodL1 bundle";
    Console.WriteLine("if you wont to chois languages write the languages with ',' between language and if you wont all languages write all");
    command += " --lang " + Console.ReadLine();
    Console.WriteLine("do you want to do output to your files?");
    if (Console.ReadLine() == "yes")
    {
        Console.WriteLine("write or only name and type to file or directory and name and type to file!");
        command += " --output " + Console.ReadLine();
        Console.WriteLine("do you want to put note on your output file?");
        if (Console.ReadLine() == "yes")
        {
            command += " --note";
        }
        Console.WriteLine("do you want sort by file type?");
        if (Console.ReadLine() == "yes")
        {
            command += " --sort";
        }
        Console.WriteLine("do you want to write the name of author on the output file?");
        if (Console.ReadLine() == "yes")
        {
            Console.WriteLine("whiche name?");
            command += " --author " + Console.ReadLine();
        }
    }
    Console.WriteLine("do you want to remove empty lines from your orginal file?");
    if (Console.ReadLine() == "yes")
    {
        command += " --r-e-l";
    };
    Console.WriteLine(command);
    // יצירת קובץ התגובה
    using (var responseFile2 = new StreamWriter("response.rsp"))
    {
        responseFile2.WriteLine(command);
    }
    // הדפסת הודעת הצלחה
    Console.WriteLine("הקובץ response.rsp נוצר בהצלחה");

    // הפעלת הפקודה עם קובץ התגובה
    Process.Start("dotnet", "@response.rsp");
});



rootCommand.AddCommand(bundleCommand);
rootCommand.AddCommand(create_rspCommand);





rootCommand.InvokeAsync(args);

