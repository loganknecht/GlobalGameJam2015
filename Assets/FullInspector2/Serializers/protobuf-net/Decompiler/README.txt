The decompiler uses NRefactory to convert the protobuf-net precompiled DLL directly
into C# code. It then applies some post-processing steps that help ensure the generated
output will compile.

The C# decompiled output can be used in Unity's build process, allowing for an extremely
simple and easy to use protobuf-net for AOT platforms.

DecompilerSources contains the source code for the decompiler along with a prebuilt
NRefactory. DecompilerExecutables contains a prebuilt decompiler that Full Inspector
will automatically invoke. Unfourtantely, the decompiler requires .NET 4.0.

Please note that for actual decompiler execution, the DLL files in the Executables
directory have had their extensions renamed so that Unity will not import them (as they
require .NET 4.0). In order to deal with this, the FI decompiler automatically copies
the Executables folder to a temporary directory and renames the DLL files so that they
have the correct extension before executing the Decompiler.