#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

public static class GenerateUnityTests
{
    /// <summary>
    /// バッチ専用。CLI から  
    /// <br/>`-executeMethod GenerateUnityTests.EditModeSamples`
    /// を呼び出すと、<c>Assets/Tests</c> 以下に
    /// 
    /// * <c>Tests.asmdef</c> （EditMode 用アセンブリ定義）
    /// * <c>SampleTest.cs</c> （NUnit サンプルテスト）
    /// 
    /// を生成して `AssetDatabase.Refresh()` します。  
    /// 既存ファイルがある場合は **上書きせずスキップ** するため安全です。
    /// </summary>
    public static void EditModeSamples()
    {
        const string testsPath = "Assets/Tests";

        // ── 1) 『Assets/Tests』フォルダーを必ず用意 ─────────────
        if (!AssetDatabase.IsValidFolder(testsPath))
        {
            // 既に Assets/Tests 以外の「Tests 1」「Tests 2」…が存在しても
            // ここで強制的に本来のパスを生成して固定化する
            AssetDatabase.CreateFolder("Assets", "Tests");
            Debug.Log("[GenerateUnityTests] Created Assets/Tests folder");
        }

        // ── 2) asmdef を生成（なければ）──────────────────────
        var asmdefPath = Path.Combine(testsPath, "Tests.asmdef");
        if (!File.Exists(asmdefPath))
        {
            var asmdefJson = @"{
    ""name"": ""GeneratedEditModeTests"",
    ""optionalUnityReferences"": [""TestAssemblies""],
    ""includePlatforms"": [""Editor""],
    ""defineConstraints"": [""UNITY_INCLUDE_TESTS""]
}";
            File.WriteAllText(asmdefPath, asmdefJson);
            Debug.Log("[GenerateUnityTests] Created Tests.asmdef");
        }

        // ── 3) サンプルテストスクリプトを生成（なければ）────────
        var testScriptPath = Path.Combine(testsPath, "SampleTest.cs");
        if (!File.Exists(testScriptPath))
        {
            var testScript = @"using NUnit.Framework;

public class SampleTest
{
    [Test]
    public void Passes()
    {
        Assert.IsTrue(true);
    }
}";
            File.WriteAllText(testScriptPath, testScript);
            Debug.Log("[GenerateUnityTests] Created SampleTest.cs");
        }

        // ── 4) Unity にインポートを認識させる ─────────────────
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport |
                              ImportAssetOptions.ForceUpdate);

        Debug.Log("[GenerateUnityTests] テストセットアップ完了");
    }
}
#endif