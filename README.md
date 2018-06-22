・10mメッシュ標高のダウンロード

https://fgd.gsi.go.jp/download/menu.php
①上記URLにアクセスし、国土地理院基盤地図情報ダウンロードサービスにアクセス

②「ダウンロード」の「基盤地図情報　数値標高モデル」の「ファイル選択へ」をクリック

③「10mメッシュ」にチェックを入れ、「10B」にのみチェックを入れ他の「5A」「5B」「10A」のチェックを外す

④ダウンロードしたいファイルを選択して、「ダウンロードファイル確認へ」からダウンロード（会員登録が必要）


・10mメッシュ標高のインサート方法

①AltitudeInserterを起動

②「ファイル選択」をクリックし、標高データのXMLファイル（FG-GML-xxxx-yy-dem10b-zzzzzzzz.xml）を選択（複数ファイル選択可）

③「インサート開始」をクリックするとインサート開始



・インサートするテーブルの変更方法（プログラム内の書き換えるべき場所）

①「MainForm.cs」→「MainForm.Designer.cs」内「sqlConnection1」とコメントアウトされている中の「this.sqlConnection1.ConnectionString =」に「対象にするサーバ（Data Source）」、「対象とするDB（Initial Catalog）」を記述する

②「AltDataGetter.cs」内「InsertAltitudeData」メソッドの「bulkCopy.DestinationTableName=」に「対象とするテーブル」を記述する（mainform.Label_stateの文章も書き替えておくとわかりやすい）

③AltDataGetter.cs」内「AltitudeDataGetter」メソッドの「bulkCopy.DestinationTableName=」に「対象とするテーブル」を記述する（mainform.Label_stateの文章も書き替えておくとわかりやすい）
