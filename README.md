# Blackfeather in C-Sharp
# Installing
## Nuget
### https://www.nuget.org/packages/Blackfeather/
```bash
   Install-Package Blackfeather 
```
## Source
```bash
  git clone https://github.com/TimothyMeadows/Blackfeather
```
# Blackfeather.Data.ManagedMemory
Ths is a helper class for storing data in memory in a managed collection. It supports serialization allowing you to import, and, export serializable types.

## ManagedMemorySpace
Model that data in memory is read, and, written too.
```cs
   public struct ManagedMemorySpace
   {
        public long Created;
        public long Accessed;
        public long Updated;
        public string Pointer;
        public string Name;
        public object Value;
    }
```
## T Read<T>(string pointer, string name)
Read entry from memory that matches the pointer, and, name.
```cs
   var memory = new ManagedMemory();
   var caw = memory.Read<string>("birds", "raven");
   Console.WriteLine(caw);
```
## T[] ReadAll(string pointer)
Read all entries from memory that matches the pointer.
```cs
   var memory = new ManagedMemory();
   var caw = memory.ReadAll<string>("birds");
   Console.WriteLine(caw);
```
## void Write(string pointer, string name, T value)
Write entry into memory. Remove any previous entry that matches the pointer, and, name.
```cs
   var memory = new ManagedMemory();
   memory.Write("birds", "raven", "caw!");
```
## void WriteAll(MangedMemorySpace[] spaces)
Write all entres into memory. Remove any previous entries that matches the pointer, and, name.
```cs
   var memory = new ManagedMemory();
   var spaces = new List<ManagedMemorySpace>();
   
   spaces.Add(new ManagedMemorySpace()
   {
      Created = 0,
      Updated = 0,
      Accessed = 0,
      Pointer = "birds",
      Name = "raven",
      Value = "caw!"
   });
   memory.WriteAll(spaces);
```
