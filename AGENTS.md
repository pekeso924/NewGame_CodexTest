# AGENTS.md – 開発者ガイド / Contributor Guide

---

## 目次

1. 開発の進め方
2. コードの書き方
3. Unity プロジェクトの取り扱い

   1. `.meta` ファイル――**絶対に手を触れないで！**
   2. `Library` フォルダ
   3. 変更の反映と Unity の認識
   4. アセンブリ定義 (`.asmdef`) ―― **原則、作ってはいけない**
   5. Git 操作の注意点
   6. **CI で毎回起こる「意図しない差分」エラーを 0 にする手順**
4. テストについて

   1. **テスト結果 XML の安全な取得**
5. パッケージの取り扱い

   1. **Packages ディレクトリ最重要ファイルの絶対厳守ルール**

## 1. 開発の進め方

* **テスト駆動開発 (TDD)** を徹底してください。まずテストを実装し、要件をすべて満たすまで開発を止めないでください。
* オフライン環境です。外部インターネットに依存しない解決策のみを採用してください。
* テストを意図的に甘く書き換えて合格を装う行為は **厳禁** です。
* **時間的制約を除く客観的、論理的、理論値的に不可能** と判断した場合のみ作業をやめ、以下をレポートを.mdで作成してください。

  1. 未実装機能
  2. 「時間的制約を除く客観的、論理的、理論値的に不可能」と判断した根拠
     根拠の提示なしに「不可能」と言い切らないこと。
* あなたはヘッドレス環境しか利用できません。`-nographics` が随所で必要になります。
* 個人開発です。

---

## 2. コードの書き方

* お任せします。

---

## 3. Unity プロジェクトの取り扱い

### 3.1 `.meta` ファイル――**絶対に手動で編集、作成しないで！**

| NG 集                                 | なぜダメなのか                                      |
| ------------------------------------ | -------------------------------------------- |
| **`.meta` を手で新規作成**                  | GUID が重複し、別アセットが上書きされる危険があります。               |
| **`.meta` の GUID を書き換え**             | 依存関係が壊れ、参照切れやプレハブ破損の原因になります。                 |
| **差分を戻そうと `git checkout -- *.meta`** | Unity が自動で付与した正当な GUID まで巻き戻し、プロジェクト全体が壊れます。 |

**覚えてください ―― "`.meta` を手で編集・新規作成・削除するのは、自爆スイッチを押すのと同じ"**。
AI / 人間を問わず、本リポジトリで **.meta を手動編集・作成した時点でタスクは即失格** です。
**ただし、Unity が自動生成した `.meta` を *コミットする行為は必ず行ってください*。**

> #### ✅ 正しい運用 4 箇条
>
> 1. **新しいアセットを追加したら、必ず Unity を起動**（またはバッチモードで一度実行）し、インポートと `.meta` 生成を完了させる。
> 2. **`.meta` は絶対に手で編集・新規作成・削除しない**。破損したら削除して **Unity に再生成させる**。
> 3. **（必要があって）アセンブリ定義ファイル (.asmdef) を作成する場合は、参照が正しいか常に確認**する。
> 4. **★今回の最重要ポイント**
>
>    * **Unity が自動で生成したファイル（`.meta`、`Packages/packages-lock.json`、`ProjectSettings/*.asset` 等）は、基本的に *すべてまとめてコミット* して構いません。**
>    * **`git add .` や `git add -A` を遠慮なく多用し、生成物を一括でステージする方が実際には事故が起こりにくく、結果的に安全です。**
>    * チーム開発で GUID 衝突リスクがある場合でも、"最初に全部入れてしまう → 以降は差分が出ない" 戦略がシンプルかつ有効です。

### 3.2 `Library` フォルダ

* `Library` 削除は **最後の手段**。再インポートコストと破損リスクが高い。
* 先に `.asmdef` 構文・パッケージ整合性・Unity 再起動・対象アセット再インポートを試行してください。

### 3.3 変更の反映と Unity の認識

* CLI でファイル操作後は Unity が変更を検知する余裕を持たせる（短い sleep かバッチ起動＆終了）。
* **可能なら GUI で Unity を一度起動して確認するのが最も安全**。

  * GUI が利用できない環境では **`-batchmode -nographics`** で起動し、**BatchBoot** アセットのログを確認してください。
* **BatchBoot** アセットの自動スクリプト（`Assets/Editor`）により、CLI から Unity を起動した際はインポート・コンパイル・`.meta` 生成が完了するまで待機し、その後自動で終了します。
* **CLI 実行時に `-executeMethod` や `-quit` オプションは付けないでください。**

  * **`-quit` を付けない方針で固定**します。
  * **ただし、どうしても `-executeMethod` を使用する必要がある場合は、必ず最後に `-forceBatchBoot` フラグを追加してください。**
    
    ```bash
    # 例：カスタムメソッドを実行しつつBatchBootも動作させる場合
    Unity -batchmode -nographics -projectPath TestUnity -executeMethod MyClass.MyMethod -forceBatchBoot
    ```
    
    * `-forceBatchBoot` フラグにより、`-executeMethod` が指定されていてもBatchBootスクリプトが確実に実行され、必要なアセットのインポートと`.meta`ファイルの生成が行われます。
    * このフラグを付け忘れると、BatchBootが動作せず、プロジェクトの整合性が保たれない可能性があります。

### 3.4 アセンブリ定義 (`.asmdef`) ―― **原則、作ってはいけない**

* **基本方針**

  * テスト・スクリプトは `Assets/Tests/Editor` (EditMode) や `Assets/Tests/PlayMode` (PlayMode) といった **既定のフォルダー階層に置くだけ**で Unity Test Runner が自動検出します。
  * **テスト専用の asmdef を作成しないでください。**（asmdef を増やすと依存関係が複雑になり、ビルド時間と管理コストが跳ね上がります）
* **例外的に必要なケース**（DLL 分割、Define 制御など）がある場合のみ、次の条件をすべて満たすなら作成を許可します。

  * テスト用アセンブリには `optionalUnityReferences = ["TestAssemblies"]` と `defineConstraints = ["UNITY_INCLUDE_TESTS"]` を設定し、**Player ビルドに混入しないよう十分注意**する。
  * `references` には **最小限の必要アセンブリだけ**を明示する。

### 3.5 Git 操作の注意点

* Unity 起動中に `git checkout`, `git rm` を行わない。エディタを閉じてから操作する。
* **`.meta` を含む Unity 自動生成ファイルは、`git add -A`（または `git add .`）で *まるごとステージ* してコミットしてしまう方が安全です。**
* **テスト結果 XML や一時生成物は `.gitignore` で確実に除外**し、リポジトリの肥大化を防いでください。

### 3.6 CI で毎回起こる「意図しない差分」エラーを 0 にする手順

1. **Unity を一度バッチモードで起動し、全自動生成物を吐かせる**

   ```bash
   /opt/unity/Editor/Unity -batchmode -nographics -projectPath TestUnity
   ```

   \*`-quit` を付けず、**`-executeMethod` も指定しない**でください。
   バッチ起動時は **BatchBoot** アセットが自動的に発火します。`-executeMethod` を付けると BatchBoot が発火しない場合があります。
   
   **※ どうしても `-executeMethod` を使用する必要がある場合は、必ず `-forceBatchBoot` フラグを最後に追加してください：**
   
   ```bash
   # カスタムメソッドを実行する場合の例
   /opt/unity/Editor/Unity -batchmode -nographics -projectPath TestUnity -executeMethod MyClass.MyMethod -forceBatchBoot
   ```

2. 生成された差分を **そのままコミット**

   ```bash
   git add -A   # まとめて全部ステージ → コミット
   git commit -m "chore: Unity auto-generated files"
   ```

   * 代表例:

     * `Assets/**/**.meta`
     * `Packages/packages-lock.json`
     * `ProjectSettings/*.asset`

3. **lockfile（packages-lock.json）を手動で巻き戻さない**

   * manifest を触ったら lockfile も必ず同じコミットで更新。

4. **生成系フォルダ／テストコード／テスト結果 XML は `.gitignore` で除外**

   ```gitignore
   # Unity sample tests (auto-generated)
   /Assets/Tests/Editor/
   # テスト結果
   /**/results-*.xml
   ```

5. CI の最終ステップで `git diff --exit-code` を走らせ、**差分ゼロを保証** ― もし赤くなったら上記手順を踏み直すだけで解決できます。

> **よくあるハマりどころ**
>
> * `.meta` を入れ忘れて GUID が毎ビルドで変わる
> * lockfile を「汚れ」と勘違いして `git checkout --` する
> * ProjectSettings の自動追記を放置
>
> これらが **"管理されていないファイル"** を生み、CI が失敗します。
> **Unity 自動生成物は一度に全部コミット（`git add -A`）** を徹底し、常に **`git diff` が空** の状態でプッシュしてください。

---

## 4. テストについて

* 可能な限り **Unity Test Framework** を用いた自動テストを実行してください。
* オフラインキャッシュ済みパッケージのみ利用可能です。
* 失敗するテストを放置しない。**テストが緑になるまで merge 不可**。
* **asmdef を作らず** `Assets/Tests/Editor` 等の既定フォルダーにテストを配置する運用を推奨します。
* `playModeTestRunnerEnabled` を 1 に設定済み。

### 4.1 **テスト結果 XML の安全な取得**

| ✅ やること                                                           | ❌ やってはいけないこと                                                             |
| ---------------------------------------------------------------- | ------------------------------------------------------------------------ |
| **絶対パス** を渡す。例:<br>`-testResults "$(pwd)/TestUnity/results.xml"` | `-testResults TestUnity/results.xml` のように *projectPath* を重ね書きし、パスを二重にする。 |
| **`cd <projectPath>` してから** `-testResults results.xml` を渡す。      | ~~CWD を意識せず相対パスを渡す~~（上記のいずれかを必ず守れば問題なし）                                  |
| 実行時と同じロジックでファイルを探し、**存在しなければ即失敗扱い**。                             | 「XML が無い＝成功」と誤判定する。                                                      |
| コンパイルエラーで終了した場合は XML が出力されない。これも失敗として扱う。                         | ―                                                                        |

> #### 背景
>
> Unity は `-projectPath` が与えられると **相対パスを自動結合** して出力先を決定します。
> 相対で `TestUnity/results.xml` を渡すと実際には `TestUnity/TestUnity/results.xml` が生成され、
> `results.xml` だけを渡すと `projectPath` 直下に出力されます。
> この食い違いが「XML が見つからない」原因になります。
> **絶対パスを渡すか、`cd` して単純なファイル名のみ渡す** のどちらかを徹底してください。

#### 🎓 **先輩からの体験談と実践的解決策**

##### XML が生成される場所

Unity に `-testResults` オプションを渡すと、指定したパスに XML が出力されます。`-projectPath` を同時に指定した場合、相対パスはすべて *projectPath を基準* として解釈されます。そのため実行ディレクトリに依存せず、次のようになります。

* 例: `-projectPath TestUnity -testResults "$(pwd)/TestUnity/results.xml"`

  * 絶対パスを指定しているため、確実に `TestUnity/results.xml` に出力される
* 例: (プロジェクト直下で) `cd TestUnity && /opt/unity/Editor/Unity -batchmode -nographics -runTests -testResults results.xml`

  * カレントディレクトリが `projectPath` になるため `TestUnity/results.xml` が生成される

`-testResults TestUnity/results.xml` のように相対パスを直接渡すと `TestUnity/TestUnity/results.xml` へ二重に解釈されてしまうので注意してください。

##### 取得を安定させるポイント

1. **必ず絶対パスを渡すか、`cd <projectPath>` してから実行する**

   * AGENTS.md で明示されている通り、どちらかを徹底することでパスの食い違いを防げます。
2. **コンパイルエラー時は XML が出力されない**

   * スクリプトにエラーがあるとテストランナーが起動できず、XML も生成されません。先に `Editor.log` や標準出力を確認し、エラーを解消してください。
3. **テスト実行後に指定パスを確認する**

   * `ls` でファイルの有無を確認し、無い場合は上記のパス指定ミスやコンパイルエラーを疑います。

以上を守れば、CI 環境でも安定して `results.xml` を取得できます。

---

## 5. パッケージの取り扱い

### 5.1 **Packages ディレクトリ最重要ファイルの絶対厳守ルール**

`Packages/manifest.json` と `Packages/packages-lock.json` は **プロジェクトの生命線** です。

| **やってはいけない操作**                                | **理由**                                 |
| --------------------------------------------- | -------------------------------------- |
| `git checkout -- Packages/manifest.json`      | Unity が自動追加した必須パッケージを消し飛ばし、ビルド不能になります。 |
| `git checkout -- Packages/packages-lock.json` | 依存解決が壊れ、テストフレームワークが消滅します。              |

#### Unity による自動変更への正しい対処

1. `git diff` で変更内容を精査
2. 必須パッケージが追加された場合は **そのままコミット**
3. 不要な差分のみパッチモードで部分戻し
4. 不明点があれば自己判断せず要相談

> **違反した場合** — プロジェクト崩壊の責任を負う覚悟がありますか？
> `.meta` 手動編集と同様、本ガイド違反は **即タスク失敗** と見なされます。

---

*この AGENTS.md は、Codex 1 エージェントおよび人間開発者が安全かつ効率的に作業するための **単一の信頼できる情報源 (SSOT)** です。内容を理解し、遵守してください。*
