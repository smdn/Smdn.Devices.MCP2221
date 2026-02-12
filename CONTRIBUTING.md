[Issue]:https://github.com/smdn/Smdn.Devices.MCP2221/issues/ "GitHub issues"
[Open an Issue]:https://github.com/smdn/Smdn.Devices.MCP2221/issues/new "GitHub issues"
[Pull Request]:https://github.com/smdn/Smdn.Devices.MCP2221/pulls/ "GitHub pull requests"
[General Bug Report]:https://github.com/smdn/Smdn.Devices.MCP2221/issues/new?template=01_bug-report.yml "GitHub issue template"
[Feature Request]:https://github.com/smdn/Smdn.Devices.MCP2221/issues/new?template=02_feature-request.yml "GitHub issue template"

# Contribution guidelines
Contributions are welcome! You can contribute to this project by submitting [Issue]s or [Pull Request]s.

Please follow the descriptions in the relevant sections below according to what you would like to contribute.

IssueやPull Requestを送る際は、可能なら英語が望ましいですが、日本語で構いません。　詳細は以下の該当する項目に従ってください。

## How to contribute

### Reporting issues / 不具合の報告
**Did you find a bug?** Please use the [General Bug Report] issue template to submit a bug report.

Please fill out the necessary items listed in the template as much as possible so that owner and collaborators can respond quickly and accurately.


### Feature requests and improvements / 機能追加・改善の提案
**Do you intend to add a new feature or improving existing feature?** Please use the [Feature Request] issue template.

If you have any feature requests including API suggestions, or an implementation improvements, please feel free to suggest them. If you can clarify what you want to achieve and what is lacking to achieve it, it will make it easier to proceed with the discussion.


### Contributing changes to the codes / コードに対する変更
**Did you write a patch that improves feature or fixes a bug?** Please create a [Pull Request].

If possible, it would be helpful if you could also write a test case that corresponds to the change, but it is not required.

Small improvements and modifications can also be made through pull requests.

On the other hand, if it involves major changes, such as across multiple files, please send it as a [Feature Request] in advance instead of a pull request.


### Contributing changes to English texts / 英文に対する変更
**Do you have any corrections for incorrect or strange English texts?** Please create a [Pull Request].

Corrections from English speakers are welcome!

Many English texts are written with the help of machine translation. We would like to correct any English mistakes in the text that users read, mainly in the documentation, but also in the code comments.

機械翻訳を使用しているため、不自然な英語となっている場合があります。　ドキュメントやコードのコメントを含め、特にユーザーの目に触れる部分での英文の誤りを見つけた場合は、プルリクエスト等で修正の提案をいただけると助かります。


### Sponsor this project / プロジェクトの支援
This project is open source and its artifacts are free. To continue and sustain development, please consider supporting through [GitHub sponsors](https://github.com/sponsors/smdn?frequency=one-time).


### Other Contributions / その他
If you would like to make other contributions, please [open an issue] and submit it.


### ❌Unacceptable changes / 受け入れられない変更
Changes such as the following examples may not be acceptable:

- ❌Performance improvements without benchmarks.
  - ✅Please include evidence in the pull request at least, or propose it as the [Feature Request].
- ❌Changes that add dependencies, such as the use of third-party packages.
  - ✅If you would like to make such a change, please create a [Feature Request] first.
- ❌Changes using *tricky* codes, like one-liners or shortcoding-oriented codes.<br/>❌Changes using the codes that are difficult to understand its intention, like too short naming of variables, etc.
  - ✅Try to write self-documenting codes. Code that is clear in its intentions, even if somewhat verbose, is preferable.


## Requests when writing and submitting in English
Some non-English speaking contributors, including the owner of this project, rely on the help of machine translations to read and write English. Therefore, it would be greatly appreciated if you could comply with the following requests.

- Please use short and simple sentences to avoid misreading and incorrect translation.
- Please consider using a list notation with short sentences, rather than long sentences with multiple clauses.
- Please use conjunctive adverbs and linking words to clarify relationships between sentences. (ex: firstly, also, however, therefore, otherwise, on the other hand, by the way)
- Please avoid the use of abbreviations except technical terms.
  - Avoid jargons and non-technical abbreviation terms, as they may not be understood by unfamiliar people. (ex: WKM, FYI, LGTM, AFAIK, IMO)
  - You may use technical terms that are clear in context, or abbreviations that are redundant when written without abbreviations. (ex: IDE, API, HTTP)
  - If you are using an abbreviation for the first time in a sentence, it would be helpful if you could write the term without abbreviation in parentheses like this; PR (Pull Request), IV (initial vector), VSCode (Visual Studio Code).


## Requests when making the changes to the code / コード変更時のお願い

### Code formats / コードフォーマット
For indentation, line breaks, spaces, casing conventions, etc. should follow the format in existing code and the format defined in `.editorconfig` files.

インデント・改行・空白・大文字小文字の規則などは、既存のコード、あるいは`.editorconfig`で定義されているフォーマットにならうようにしてください。

Some formatting deviations are acceptable. If necessary, fix them before and after the merge.

フォーマットは多少逸脱していてもかまいません。　必要ならマージ前後で修正します。

### Test codes / テストコード
If possible, please provide test code that covers the changes or add to existing tests.

可能であれば、変更箇所をカバーするテストコードを提示いただくか、既存のテストへの追加をお願いします。

### Copyright notices / 著作権表示
Do not modify or add any copyright notices.

著作権表示を書き換えたり追加したりしないでください。

After merging pull requests, the copyright notice will be updated, if necessary, as follows:

プルリクエストのマージ後、著作権表示は必要に応じて次のように更新されます。

```diff
 // SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
+// SPDX-FileCopyrightText: 202X smdn <smdn@smdn.jp> and contributors
 // SPDX-License-Identifier: MIT
```
