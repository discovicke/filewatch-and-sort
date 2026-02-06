# 🦆 Sortomatic 9000
![C#](https://img.shields.io/badge/C%23-.NET-239120?logo=csharp&logoColor=white)
![.NET](https://img.shields.io/badge/.NET-Desktop%20Application-512BD4?logo=dotnet&logoColor=white)
![Config](https://img.shields.io/badge/Config-XML-blue)
![Platform](https://img.shields.io/badge/Platform-Windows-lightgrey)
![Status](https://img.shields.io/badge/Status-Stable-brightgreen)

Sortomatic 9000 is a file sorting program that automatically monitors your folders and moves files to the right place based on rules in a settings file (XML format).
The program is perfect for keeping order in e.g. the Downloads folder without lifting a finger.

## Installation

### Step-by-step

1.	Download the program
   
    Click the green "Code" button, then select "Download ZIP".
    Save the file somewhere you can easily find it, e.g. on the Desktop.

2.	Open the program

   * Open the folder `Uppgift/dist`
   * Double-click the file `Uppgift.exe`.
   * The program starts directly and begins reading Inställningar.xml.
  	
3.	Let it work in the background

    When the program starts, it will automatically read Inställningar.xml and start monitoring the folders you specified there.
    You can minimize the program — it will continue working until you close it.

#### How it works

Sortomatic 9000 uses an XML file to know which folders should be monitored and where files with different extensions should be moved. You can add more Directories if you want to monitor more folders or move other types of files to other folders.
Here is an example of what Inställningar.xml can look like:

```
<?xml version="1.0" encoding="UTF-8" ?>
<Settings>
  <Log>log.txt</Log>

  <Directory>
    <Name>Pictures</Name>
    <Input>C:\Users\Donald\Downloads</Input>
    <Output>C:\Users\Donald\Pictures</Output>
    <Type>.jpg</Type>
    <Type>.png</Type>
  </Directory>

  <Directory>
    <Name>Documents</Name>
    <Input>C:\Users\Donald\Downloads</Input>
    <Output>C:\Users\Donald\Documents</Output>
    <Type>.txt</Type>
    <Type>.pdf</Type>
  </Directory>
</Settings>
```

---

## Tips and extras

* The program runs in the background and keeps track of new or changed files in real-time.
* It also sorts files that were added while the program was not running, perfect for cleaning up a neglected folder.
* Changes in Inställningar.xml are automatically loaded without needing to restart the program.
* The log file (log.txt) is continuously updated with date, time and what was moved where.
* It can easily be customized by editing the configuration file, no commands or flags needed.
