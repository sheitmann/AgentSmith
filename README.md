AgentSmith
==========

Agent Smith is C# code style validation plugin for ReSharper (Visual Studio plugin). 

The original home was google code and you can find the archived project under https://code.google.com/archive/p/agentsmithplugin/ .

I started to work on AgentSmith when ReSharper 7 Beta was released, to update it and keep it working on the latest ReSharper versions.

Current and future version can be obtained from https://resharper-plugins.jetbrains.com/packages/ReSharper.AgentSmith/ or via the extension manager of ReSharper.

If you find any bugs or would like to see improvements, please open an issue on the github page.
I will try my best to fix them or add improvements but they will take some time because this is just a hobby project.

## Features

### Naming Conventions

Plugin has interface that allows you to specify what naming rules should be applied to certain declarations.

Naming rules include:

- Obligatory prefixes
- Obligatory suffixes
- Prefixes that should be avoided
- Suffixes that should be avoided
- Casing rules: Camel, Pascal, Uppercase
- Regular Expression rules
- Declaration to which a naming rule applies is identified by one or more matching rules. In a Matching rule following can be specified:
  - Declaration type.
  - Visibility
  - Attribute declaration is marked with
  - Base class of interface (for classes and interfaces)
  - For a naming rule also non matching rules can be specified in similar way

For each kind of rule plugin highlights incorrect declaration and suggests quickfixes(except regular expressions).

### XML comment validation

Agent Smith can check that certain declaration have xml comments. And have flexible configuration interface for specifying what members shall have comments.

### Spell check

Agent Smith performs spell checking of 

* XML comments
* C# string literals
* C# identifiers. Identifiers are split by camel humps and checked against dictionary.
* Resource files (it can automatically choose appropriate dictinary depending on resource file extension) and suggests quick fixes (Word suggestions, Replace with

Open Office dictionaries can be imported so a few languages are virtually supported.

### Smart paste

If you are smart pasting a text into XML comment Agent Smith will insert `///` characters at line breaks and optionally escape XML reserved characters. If you are smart pasting into a string literal Agent Smith will escape string characters correspondingly.

### Comment reflowing

Automatically realigns words in comment to fit configured line width.
