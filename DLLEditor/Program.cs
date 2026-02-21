using Mono.Cecil;
using Mono.Cecil.Cil;
using ADV;
using System.Reflection;
using ModdingCore;
using UnityEngine;
using ModUtils;

internal class Program
{
    static String inputDirectory = "\"C:\\Program Files (x86)\\Steam\\steamapps\\common\\Holder of Place\\HolderOfPlace_Data\\Managed";
    static String inputPath = "C:\\Users\\Michael\\Desktop\\HolderOfPlace\\Assembly-CSharp.dll";
    static String modPath = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Holder of Place\\HolderOfPlace_Data\\Managed\\ModBootstrap.dll";

    static String outputPath = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Holder of Place\\HolderOfPlace_Data\\Managed\\Assembly-CSharp.dll";
    static void Main(string[] args)
    {
        var assemblyPath = Path.GetFullPath(inputPath);


        var resolver = new DefaultAssemblyResolver();
        resolver.AddSearchDirectory(inputDirectory);

        AssemblyDefinition _assembly = AssemblyDefinition.ReadAssembly(assemblyPath, new ReaderParameters { AssemblyResolver = resolver });
        AssemblyDefinition _mod = AssemblyDefinition.ReadAssembly( modPath, new ReaderParameters { AssemblyResolver = resolver });

        //Spare Change [Unused]
        var _menuSettings = FindMethod(_assembly, nameof(UIButton_MenuSettings), nameof(UIButton_MenuSettings.MouseDownEffect));
        ILProcessor processor = _menuSettings.Body.GetILProcessor();
        var instruction = processor.Body.Instructions[0];
        processor.InsertBefore(instruction, processor.Create(OpCodes.Call, GetMethodReference<BootstrapMain>(_assembly, nameof(BootstrapMain.OpenUnityExplorer), Array.Empty<Type>())));

        //MAIN CODE BLOCK!!!
        var _titleScreen = FindMethod(_assembly, nameof(TitleScreenControl), "Start");
        processor = _titleScreen.Body.GetILProcessor();
        instruction = processor.Body.Instructions[0];
        processor.InsertBefore(instruction, processor.Create(OpCodes.Call, GetMethodReference<BootstrapMain>(_assembly, nameof(BootstrapMain.Start), Array.Empty<Type>())));

        //Event Insertion: Card Generated
        var _cardGenerated = FindMethod(_assembly, nameof(RecruitPanel), nameof(RecruitPanel.Generate), 5);
        processor = _cardGenerated.Body.GetILProcessor();
        instruction = processor.Body.Instructions.FirstOrDefault(i =>
        {
            
            //Console.WriteLine(methodRef);
            //Console.WriteLine(i.Offset + ": " + i.OpCode);
            Console.WriteLine(i.Operand?.ToString() ?? "null");
            return (i.Operand?.ToString() == "!!0 UnityEngine.Object::Instantiate<UnityEngine.GameObject>(!!0,UnityEngine.Transform)");
        });
        if (instruction == null)
        {
            Console.WriteLine("Could not find instantiate()");
            return;
        }
        int index = processor.Body.Instructions.ToList().IndexOf(instruction) + 2;
        processor.InsertAfter(index, processor.Create(OpCodes.Call, GetMethodReference<ModEvents>(_assembly, nameof(ModEvents.InvokeCardGenerated), new Type[] { typeof(Card) })));
        processor.InsertAfter(index, processor.Create(OpCodes.Ldloc_0));

        var _buttonUpdate = FindMethod(_assembly, nameof(UIButton), nameof(UIButton.Update), 0);
        processor = _buttonUpdate.Body.GetILProcessor();
        for(int i=0; i<processor.Body.Instructions.Count; i++)
        {
            var ins = processor.Body.Instructions[i];
            //System.Console.WriteLine(ins.Operand?.ToString() ?? "null");
            if (ins.Operand?.ToString() == "System.Void ADV.UIButton::MouseDownEffect()")
            {
                System.Console.WriteLine("Adding extra check to mousedown");
                var postIns = processor.Body.Instructions[i+1];
                var endOfCheck = processor.Body.Instructions[i - 2];
                System.Console.WriteLine(endOfCheck);
                processor.InsertAfter(endOfCheck, processor.Create(OpCodes.Brfalse_S, postIns));
                processor.InsertAfter(endOfCheck, processor.Create(OpCodes.Call, GetMethodReference<BootstrapMain>(_assembly, nameof(BootstrapMain.IsInputAllowed), Array.Empty<Type>())));
                System.Console.WriteLine(endOfCheck);
                i += 2;
            }
            if (ins.Operand?.ToString() == "System.Void ADV.UIButton::MouseUpEffect()")
            {
                System.Console.WriteLine("Adding extra check to mouseup");
                var postIns = processor.Body.Instructions[i + 1];
                var endOfCheck = processor.Body.Instructions[i - 2];
                System.Console.WriteLine(endOfCheck);
                processor.InsertBefore(endOfCheck, processor.Create(OpCodes.Brfalse, postIns));
                processor.InsertBefore(endOfCheck, processor.Create(OpCodes.Call, GetMethodReference<BootstrapMain>(_assembly, nameof(BootstrapMain.IsInputAllowed), Array.Empty<Type>())));
                System.Console.WriteLine(endOfCheck);

                i += 2;
            }
        }


        Console.WriteLine("Outputting!");
        _assembly.Write(outputPath);
        Console.WriteLine("Done!");
    }

    static PropertyDefinition FindProperty(AssemblyDefinition _assembly, string type, string property)
    {
        var _type = _assembly.MainModule.Types.First(t => t.Name == type);
        var _property = _type.Properties.FirstOrDefault(f => f.Name == property);
        Console.WriteLine($"Found Method: {type}.{property}");
        return _property;
    }

    static MethodReference GetMethodReference<T>(AssemblyDefinition _assembly, string methodName, Type[] args)
    {
        MethodInfo info = typeof(T).GetMethod(methodName, args);
        MethodReference reference = _assembly.MainModule.ImportReference(info);
        return reference;
    }

    static MethodDefinition FindMethod(AssemblyDefinition _assembly, string type, string method, int parameters = 0)
    {
        Console.WriteLine($"Searching for Method: {type}.{method}");
        var _type = _assembly.MainModule.Types.FirstOrDefault(t => t.Name == type);
        var _method = _type.Methods.FirstOrDefault(_m => _m.Name == method && _m.Parameters.Count == parameters);
        Console.WriteLine($"Found Method: {type}.{method}");
        return _method;
    }

    static MethodDefinition FindStaticMethod(AssemblyDefinition _assembly, string type, string method, int parameters = 0)
    {
        var _type = _assembly.MainModule.Types.FirstOrDefault(t => t.Name == type);
        var _method = _type.Methods.FirstOrDefault(_m => _m.Name == method && _m.IsStatic && _m.Parameters.Count <= parameters);
        Console.WriteLine($"Found Method: {type}.{method}");
        return _method;
    }
}