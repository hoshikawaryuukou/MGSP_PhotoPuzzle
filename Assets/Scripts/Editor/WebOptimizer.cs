using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

public sealed class WebOptimizer
{
    [MenuItem("Tools/Optimize")]
    public static void Optimize()
    {
        var namedBuildTarget = NamedBuildTarget.WebGL;
        //var buildOptions = BuildOptions.CompressWithLz4HC;

        PlayerSettings.SetIl2CppCodeGeneration(namedBuildTarget, Il2CppCodeGeneration.OptimizeSize);
        PlayerSettings.SetManagedStrippingLevel(namedBuildTarget, ManagedStrippingLevel.High);          
        PlayerSettings.stripUnusedMeshComponents = true;
        PlayerSettings.WebGL.dataCaching = true;
        PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Brotli;
        PlayerSettings.WebGL.exceptionSupport = WebGLExceptionSupport.None;
        PlayerSettings.WebGL.debugSymbolMode = WebGLDebugSymbolMode.Off;
        PlayerSettings.WebGL.wasm2023 = true;
        UnityEditor.WebGL.UserBuildSettings.codeOptimization = UnityEditor.WebGL.WasmCodeOptimization.DiskSizeLTO;
    }
}