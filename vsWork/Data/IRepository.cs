using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace vsWork.Data
{
    public interface IRepository<T1, T2> where T1 : IEntity
    {

        /// <summary>
        /// テーブルを作成します
        /// </summary>
        void CreateTable();

        /// <summary>
        /// テーブルを削除します
        /// </summary>
        void DropTable();

        /// <summary>
        /// レコードを追加します
        /// </summary>
        /// <param name="item"></param>
        void Add(T1 item);

        /// <summary>
        /// レコードを削除します
        /// </summary>
        /// <param name="id"></param>
        void Remove(T2 id);

        /// <summary>
        /// レコードを更新します
        /// </summary>
        /// <param name="item"></param>
        bool Update(T1 item);

        /// <summary>
        /// 任意のレコードを取得します
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T1 FindById(T2 id);

        /// <summary>
        /// 有効なレコードを全取得します
        /// </summary>
        /// <returns></returns>
        IEnumerable<T1> FindAll();
    }
}
