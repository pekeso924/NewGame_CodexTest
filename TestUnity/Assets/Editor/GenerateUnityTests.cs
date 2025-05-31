#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO; // System.IO名前空間を使用

public static class GenerateUnityTests
{
    /// <summary>
    /// CLI から `-executeMethod GenerateUnityTests.EditModeSamples`
    /// </summary>
    public static void EditModeSamples()
    {
        const string testsPath = "Assets/Tests";
        const string assemblyName = "EditModeTests"; // アセンブリ名（例: EditModeTests や Tests.Editor など）

        // ── 1) フォルダーを確実に作成 ──
        if (!Directory.Exists(testsPath)) // System.IO.Directory.Exists を使用
        {
            Directory.CreateDirectory(testsPath); // System.IO.Directory.CreateDirectory を使用
            AssetDatabase.Refresh(); // Unityエディタにフォルダ作成を認識させる
        }
        // 作成した（または既存の）Testsフォルダを選択状態にしておく（必須ではないが一応）
        Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>(testsPath);

        // ── 2) Test Assembly Definition File (.asmdef) を手動で生成 ──
        string asmdefFilePath = Path.Combine(testsPath, assemblyName + ".asmdef");
        if (!File.Exists(asmdefFilePath))
        {
            // .asmdef ファイルの内容を定義
            // ログで手動修正していた内容を反映
            var asmdefContent = new AsmdefDefinition
            {
                name = assemblyName,
                rootNamespace = "", // 必要に応じて設定
                references = new string[0], // 他のアセンブリへの参照があれば追加
                includePlatforms = new[] { "Editor" }, // EditModeテスト用
                excludePlatforms = new string[0],
                allowUnsafeCode = false,
                overrideReferences = false,
                precompiledReferences = new string[0],
                autoReferenced = true, // 通常、テストアセンブリはtrueで良い
                defineConstraints = new[] { "UNITY_INCLUDE_TESTS" }, // テストコードとしてコンパイルされるために必要
                versionDefines = new VersionDefine[0],
                noEngineReferences = false,
                optionalUnityReferences = new[] { "TestAssemblies" } // Unity Test Framework を使う場合に必要
            };

            string jsonContent = JsonUtility.ToJson(asmdefContent, true); // 整形されたJSONで出力
            File.WriteAllText(asmdefFilePath, jsonContent);
            AssetDatabase.Refresh(); // Unityエディタに .asmdef ファイル作成を認識させる
            Debug.Log($"[GenerateUnityTests] Test Assembly Definition を生成しました: {asmdefFilePath}");
        }

        // ── 3) サンプルテストスクリプトも手動で生成 ──
        //    ログではC# Test Scriptメニューがうまく機能していなかったので、直接ファイルを生成します。
        string sampleTestScriptPath = Path.Combine(testsPath, "NewTestScript.cs"); // 生成するスクリプト名
        if (!File.Exists(sampleTestScriptPath))
        {
            string scriptContent = @"using NUnit.Framework;
using UnityEngine;
// using UnityEditor; // EditModeテストでエディタAPIを使う場合は必要

public class NewTestScript
{
    [Test]
    public void SampleTestPasses()
    {
        // ここにテストロジックを記述します
        Assert.IsTrue(true);
    }

    // PlayModeテストの場合は IEnumerator を使う UnityTest も記述できます
    // [UnityTest]
    // public System.Collections.IEnumerator SampleUnityTest()
    // {
    //     // テストロジック
    //     yield return null;
    //     Assert.IsTrue(true);
    // }
}";
            File.WriteAllText(sampleTestScriptPath, scriptContent);
            AssetDatabase.Refresh(); // Unityエディタにスクリプトファイル作成を認識させる
            Debug.Log($"[GenerateUnityTests] サンプルテストスクリプトを生成しました: {sampleTestScriptPath}");
        }

        // ── 4) 完了メッセージ ──
        AssetDatabase.Refresh(); // 念のため最後にリフレッシュ
        Debug.Log($"[GenerateUnityTests] サンプルテスト環境の生成が完了しました (パス: {testsPath})");
    }

    // .asmdefファイルの構造を表す補助クラス (JsonUtility用)
    [System.Serializable]
    private class AsmdefDefinition
    {
        public string name;
        public string rootNamespace;
        public string[] references;
        public string[] includePlatforms;
        public string[] excludePlatforms;
        public bool allowUnsafeCode;
        public bool overrideReferences;
        public string[] precompiledReferences;
        public bool autoReferenced;
        public string[] defineConstraints;
        public VersionDefine[] versionDefines;
        public bool noEngineReferences;
        public string[] optionalUnityReferences;
    }

    [System.Serializable]
    private class VersionDefine // VersionDefineの構造
    {
        public string name;
        public string expression;
        public string define;
    }
}
#endif