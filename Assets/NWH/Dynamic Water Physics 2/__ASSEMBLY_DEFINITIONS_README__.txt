Since all the NWH assets have been updated to use assembly definitions here is a disclaimer to avoid confusion when updating: 

This asset uses Assembly Definition (.asmdef) files. There are many benefits to assembly definitions but a downside is that the 
whole project needs to use them or they should not be used at all.

  * If the project already uses assembly definitions accessing a script that belongs to this asset can be done by adding an 
  reference to the assembly definition of the script that needs to reference the asset. E.g. to access AdvancedShipController 
  adding a NWH.DWP2 reference to MyProject.asmdef is required.
  
  * If the project does not use assembly definitions simply remove all the .asmdef files from the asset after import.

Using, for example, Lux Water (which does not fature assembly definitions) will therefore require an addition of .asmdef file inside the 
Lux Water directory and a reference inside NWH.DWP2 or removal of all .asmdef files from the asset if you do not wish to use assembly definitions. 
Some assets such as Crest already feature .asmdefs and adding Crest as a reference to NWH.DWP2 is the only step needed.

More about Assembly Definitions: https://docs.unity3d.com/Manual/ScriptCompilationAssemblyDefinitionFiles.html