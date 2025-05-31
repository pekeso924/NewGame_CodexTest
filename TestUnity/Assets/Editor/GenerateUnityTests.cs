#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// CLI から  
/// <br/>`-executeMethod GenerateUnityTests.EditModeSamples_NoAsmdef`  
/// を呼び出すと、<c>Assets/Tests/Editor</c> に
///
/// * <c>SampleTest.cs</c>（NUnit サンプルテスト）
///
/// を生成し、`AssetDatabase.Refresh()` します。  
/// 既存ファイルがある場合は上書きせずスキップする安全設計です。
/// <para>
/// asmdef を生成しないため、テストは Assembly-CSharp-Editor.dll に
/// 直接コンパイルされます。  
/// リリースビルドへ混入しないよう、CI 等で  
/// 「Assets/Tests/** を除外」する運用を推奨します。
/// </para>
/// </summary>
public static class GenerateUnityTests
{
    public static void EditModeSamples_NoAsmdef()
    {
        const string testsPath = "Assets/Tests/Editor";  // ★Editor サブフォルダー

        // ── 1) 『Assets/Tests/Editor』フォルダーを必ず用意 ─────────
        if (!AssetDatabase.IsValidFolder(testsPath))
        {
            // 必要に応じて中間フォルダーも作成
            if (!AssetDatabase.IsValidFolder("Assets/Tests"))
                AssetDatabase.CreateFolder("Assets", "Tests");

            AssetDatabase.CreateFolder("Assets/Tests", "Editor");
            Debug.Log("[GenerateUnityTests] Created Assets/Tests/Editor folder");
        }

        // ── 2) サンプルテストスクリプトを生成（なければ）───────────
        var testScriptPath = Path.Combine(testsPath, "SampleTest.cs");
        if (!File.Exists(testScriptPath))
        {
            var testScript = @"using NUnit.Framework;

/// <summary>
/// サンプル用の単純な EditMode テスト
/// </summary>
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

        // ── 3) Unity にインポートを認識させる ────────────────────
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport |
                              ImportAssetOptions.ForceUpdate);

        Debug.Log("[GenerateUnityTests] テストセットアップ完了 (asmdef 無し)");
    }
}
#endif
