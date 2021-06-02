using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using vsWork.Data;

namespace vsWork.Validators
{
    public class SignUpUserValidator : AbstractValidator<SignUpUser>
    {
        /// <summary>
        /// ユーザリポジトリ
        /// </summary>
        private readonly IRepository<User, string> _userRepository;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="userRepository">ユーザリポジトリ</param>
        public SignUpUserValidator(IRepository<User, string> userRepository)
        {
            _userRepository = userRepository;

            RuleFor(x => x.UserId).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("ユーザIDを入力してください。")
                .Length(3, 100).WithMessage("ユーザIDは3文字以上100文字以下にしてください。")
                .DependentRules(() =>
                {
                    RuleFor(x => x.UserId).Must(x => IsExistUser(x)).WithMessage("登録済みのユーザIDです");
                });

            RuleFor(x => x.UserName).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("ユーザ名称を入力してください。")
                .Length(3, 100).WithMessage("ユーザ名称は3文字以上100文字以下にしてください。");


            RuleFor(x => x.Password).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("パスワードを入力してください。")
                .Length(3, 100).WithMessage("パスワードは3文字以上100文字以下にしてください。")
                .DependentRules(() =>
                {
                    RuleFor(x => new { PasswordConfirm = x.PasswordConfirm, x.Password}).Must(x => MatchPassword(x.Password, x.PasswordConfirm)).WithMessage("パスワードと確認要パスワードが異なっています。");
                });
        }
        /// <summary>
        /// ユーザの存在チェック
        /// </summary>
        /// <param name="id"></param>
        /// <returns>false:正常 true:異常</returns>
        private bool IsExistUser(string id)
        {
            User validUser = _userRepository.FindById(id);
            return (validUser == null);

        }
        /// <summary>
        /// パスワード一致検証
        /// </summary>
        /// <param name="password">パスワード</param>
        /// <param name="passwordConfirm">パスワード(確認要)</param>
        /// <returns>false:正常 true:異常</returns>
        private bool MatchPassword(string password, string passwordConfirm)
        {
                return (password == passwordConfirm);
        }
    }
}
