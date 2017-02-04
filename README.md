[![Build status](https://ci.appveyor.com/api/projects/status/0ws893ovb4pv6v3f?svg=true)](https://ci.appveyor.com/project/BenBrougher/abstractdata)
# AbstractData
AbstractData is a C# library and scripting language built to move data from one place to another.

Project Status: Under Development - Interpreter and basic string data is working

## What sort of data formats are compatible?
- SQL Server Databases  
- Excel Files  
- CSV Files  
- SQLite Databases  
- TODO: Access Databases  
- TODO: PostgreSql Databases  
- TODO: MariaDB Databases  
- TODO: Oracle Databases  
- TODO: MongoDB Datablases ?  
- TODO: Firebase ?
- TODO: JSON Files ?  

## Build with Visual Studio
This solution has been built for Visual Studio Community 2015 and can be edited and built in that environment.

## Build without Visual Studio
Without visual studio, download nuget bianaries from nuget.org and add nuget to your PATH. Then dowload Visual Studio 2015 Build Tools. After it has been installed then run the build.cmd file.

## Library Usage
Currently unusable. As things come together there will be a way to include the library in projects from source and also from NuGet.

## CLI Installation
Same answer as above. When things are working there will be a way to run scripts from the command line and an installer to add the CLI to the path.

## CLI Usage
Below is an example of how to use the interpreter

<pre><code>>> #This is a comment. Lines that start with '#' will be ignored.
>> 
>> # Database references define the files that will be used later
>> SQLiteDB sqlite1 = "C:\--path to database--"
>> ExcelFile excel1 = "C:\--path to excel file--"
>> CSVFile csv1 = "C:\--path to csv file--"
>>
>> # Table references tell where the data will move to. In this data moves from Sheet1 in the excel file to the CSV file.
>> tableReference(excel1.Sheet1 => csv1)
>> # Data references tells which columns or fields to move where. In this case letter columns to ordinal columns in the csv file
>> A => 0
>> B => 1
>> C => 2
>> D => 3
>> # The move command executes the references and moves the data
>> move()
>>
>> tableReference(excel1.Sheet1 => sqlite1.Table1)
>> A => Field1
>> B => Field2
>> C => Field3
>> D => Field4
>> move()
>>
>> # exit will close the interpreter
>> exit
</code></pre>
