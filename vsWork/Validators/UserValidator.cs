using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using vsWork.Data;

namespace vsWork.Validators
{
    /// <summary>
    /// バリデーションについてはベストプラクティスがまだ分かっていない
    /// BlazorでCustomValidationはDIできない(https://github.com/dotnet/aspnetcore/issues/23380)
    /// FluentValidationはDIできるが、複数のプロパティのバリデーションに依存したバリデーションを行おうとすると冗長になる気がする
    /// </summary>
    public class UserValidator : AbstractValidator<User>
    {
        /// <summary>
        /// ユーザリポジトリ
        /// </summary>
        private readonly IRepository<User, string> _userRepository;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="userRepository">ユーザリポジトリ</param>
        public UserValidator(IRepository<User, string> userRepository)
        {
            _userRepository = userRepository;

            RuleFor(x => x.Id).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("ユーザIDを入力してください。")
                .Length(3, 100).WithMessage("ユーザIDは3文字以上100文字以下にしてください。");

            RuleFor(x => x.Password).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("パスワードを入力してください。")
                .Length(3, 100).WithMessage("パスワードは3文字以上100文字以下にしてください。")
                .DependentRules(() =>
                {
                    RuleFor(x => new { x.Id, x.Password }).Must(x => MatchPassword(x.Id, x.Password)).WithMessage("ユーザID,またはパスワードが間違っています。");
                });
        }
        /// <summary>
        /// パスワード一致検証
        /// </summary>
        /// <param name="id">ユーザID</param>
        /// <param name="password">パスワード</param>
        /// <returns>false:正常 true:異常</returns>
        private bool MatchPassword(string id, string password)
        {
            // ユーザID未入力時は検証しない
            if (id.Length == 0)
            {
                return true;
            }
            else
            {
                User validUser = _userRepository.FindById(id);
                if (validUser == null)
                {
                    return false;
                }
                else if (validUser.Password != password)
                {
                    return false;
                }
                return true;
            }
        }
    }
}
