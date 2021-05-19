# vsWork
勤怠システム開発に使用するリポジトリです  
ノウハウがないため長期で取り組む予定です  
どこまで開発モチベーションが持つかは不明

## 動機
* .NETFrameworkの知識が4.0で止まっているため、.NET周辺の最新技術のキャッチアップ
* Webサービスの開発経験がないため、ノウハウの習得
* PostgreSQLも知識が9.2で止まっているため、最新の構文やチューニング等のキャッチアップ

## 構成
* Blazor Server APL(.NET Core 5.0)
* Dapper
* FluentValidation

* PostgreSQL 13.2(Extention:PGCRYPTO)

```psql
psql --username=postgres --command="CREATE EXTENSION PGCRYPTO;"
```



psql --username=postgres --command="CREATE EXTENSION PGCRYPTO;"