using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace vsWork.Utils
{
    /// <summary>
    /// 繰り返し処理専用のTimerラッパー
    /// Dispose後は実行中の処理が必ず無い状態となる
    /// 開始後は破棄のみ可能
    /// IDisposableのsnippestはほぼ、そのまま残してある
    /// https://qiita.com/imasaaki/items/dbb200ee20ada0c05aa9
    /// </summary>
    public class SafelyDisposableTimer : IDisposable
    {
        private bool disposedValue;
        // 定期的に実行したい処理
        private Action callbackAction;
        private Timer timer = new Timer();
        private object lockObject = new object();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="interlval">インターバル（ミリ秒）</param>
        /// <param name="callbackAction">定期的に実行したい処理</param>
        internal SafelyDisposableTimer(int interlval, Action callbackAction)
        {
            this.callbackAction = callbackAction;
            this.timer.Elapsed += this.TimerElapsedEventHandler;
            this.timer.Interval = interlval;
            // 繰り返し処理する
            this.timer.AutoReset = true;
        }

        /// <summary>
        /// タイマーを開始する
        /// </summary>
        internal void Start()
        {
            this.timer.Start();
        }

        /// <summary>
        /// 破棄
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: マネージド状態を破棄します (マネージド オブジェクト)
                    this.timer.Stop();
                    this.timer.Dispose();
                }

                // TODO: アンマネージド リソース (アンマネージド オブジェクト) を解放し、ファイナライザーをオーバーライドします
                // TODO: 大きなフィールドを null に設定します
                lock (this.lockObject)
                {
                    disposedValue = true;
                }
            }
        }

        // // TODO: 'Dispose(bool disposing)' にアンマネージド リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします
        // ~SafelyDisposableTimer()
        // {
        //     // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
        //     Dispose(disposing: false);
        // }

        /// <summary>
        /// 破棄
        /// </summary>
        public void Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private void TimerElapsedEventHandler(object sender, ElapsedEventArgs args)
        {
            lock (this.lockObject)
            {
                if (this.disposedValue == true)
                {
                    return;
                }

                // 処理を実行
                this.callbackAction();
            }
        }
    }
}
