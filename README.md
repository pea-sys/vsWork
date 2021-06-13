# vsWork
勤怠システム開発に使用するリポジトリです  
ノウハウがないため長期で取り組む予定です  
Blazorの前提知識となりそうなASP.NETも経験がないため、  
事前設計は行わずにハンズオンで開発を進めて継続改善する方針    
どこまで開発モチベーションが持つかは不明  

## 動機
* .NETFrameworkの知識が4.0で止まっているため、.NET周辺の最新技術のキャッチアップ
* Webサービスの開発経験がないため、ノウハウの習得
* PostgreSQLも知識が9.2で止まっているため、最新の構文やチューニング等のキャッチアップ

## 構成
### アプリケーション
* Blazor Server(.NET Core 5.0)
* Dapper
* NpgSQL
* FluentValidation
* Blazored.Modal
* Blazored.Toast
* Blazorize  
* Blazorise.DataGrid  
* Fluxor
* BlazorPagination
※ Postgresのコネクションはユーザーシークレットで管理

### データベース
* PostgreSQL 13.2  
パスワード管理にPGCRYPTOを利用
```psql
psql --username=postgres --command="CREATE EXTENSION PGCRYPTO;"
```

■　テーブル構成  


|テーブル名|説明|備考|
|----|----|----|
|attendance_tbl|勤怠打刻テーブル|ユーザ毎の打刻履歴の記録|
|session_tbl|セッションテーブル|認証状況の監視|
|user_tbl|ユーザ情報テーブル|ユーザ登録情報の管理|
|user_state_tbl|ユーザ状態管理テーブル|勤務状況の管理.トリガー更新|
|organization_tbl|組織テーブル|組織登録情報の管理|


■ ToDo
* 所定の勤務時間の登録
* 勤務統計情報。月毎の残業時間や勤務時間。有休取得日数などの管理
* 休日登録。会社独自の記念日とか登録可能にする
* 申請ワークフロー。休暇届けなどに使用。

■ メモ  
### 現状の構想  
1. 顧客の組織を登録する
2. 顧客の組織に所属する管理者を登録する
3. 管理者は所属するユーザ情報を任意に登録・更新・削除ができる





