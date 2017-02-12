# Example AbstractData Script
# This is an ideal showcase of what the script can become.
# Not all of the features in this are implemented

Global {parameterIn} = @paramInName
Global {staticVar} = "Static Variable"

SQLServerDB sqlDb = "<<ConnectionString>>"
ExcelFile excelFile = "C:\com\file1.xlsx"

tableReference(sqlDb>Table1 => excelFile>Sheet1)
Local {localVar} = Table1.Field1 + " Test"
Field2 => A
{localVar} => B
move()