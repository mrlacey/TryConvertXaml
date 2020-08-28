# TryConvertXaml

This is a simple project that shows how to use 
[RapidXaml.AutoFix](https://www.nuget.org/packages/RapidXaml.AutoFix/).

The program uses `XamlConverter` and takes a `.xaml` file and an 
implementation of `ICustomAnalyzer`.

The program:
- writes our the original XAML contents.
- parses the document identifying changes to make based on the analyzer(s).
- the file is the modified based on what the analyzer(s) report.
- the modified file contents are then shown on screen.

feedback appreciated.
