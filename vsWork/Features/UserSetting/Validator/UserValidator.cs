﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Fluxor;
using vsWork.Data;
using vsWork.Features.UserSetting.Store;
using vsWork.Features.Shared.Store;

namespace vsWork.Features.UserSetting.Validator
{
    public class UserValidator : AbstractValidator<User>
    {
        /// <summary>
        /// ユーザリポジトリ
        /// </summary>
        private readonly IRepository<User, string> _userRepository;
        private readonly IState<SettingState<User>> _userSettingState;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="userRepository">ユーザリポジトリ</param>
        /// <param name="UserSettingState">ユーザー設定状態</param>
        public UserValidator(IRepository<User, string> userRepository, IState<SettingState<User>> userSettingState)
        {
            _userRepository = userRepository;
            _userSettingState = userSettingState;

            if (_userSettingState.Value.Mode == SettingMode.Add)
            {
                RuleFor(x => x.UserId).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("ユーザIDを入力してください。")
                .Length(3, 100).WithMessage("ユーザIDは3文字以上100文字以下にしてください。")
                .DependentRules(() =>
                {
                    RuleFor(x => x.UserId).Must(x => IsExistId(x)).WithMessage("登録済みのIDです");
                });

                RuleFor(x => x.UserName).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("ユーザ名称を入力してください。")
                .Length(3, 100).WithMessage("ユーザ名称は3文字以上100文字以下にしてください。");

                RuleFor(x => x.Password).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("パスワードを入力してください。")
                .Length(3, 100).WithMessage("パスワードは3文字以上100文字以下にしてください。");
            }
            else if (userSettingState.Value.Mode ==SettingMode.Update)
            {
                RuleFor(x => x.UserName).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("ユーザ名称を入力してください。")
                .Length(3, 100).WithMessage("ユーザ名称は3文字以上100文字以下にしてください。");

                RuleFor(x => x.Password).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("パスワードを入力してください。")
                .Length(3, 100).WithMessage("パスワードは3文字以上100文字以下にしてください。");
            }
            else
            {
                RuleFor(x => new { Id=x.UserId, x.Password }).Must(x => MatchPassword(x.Id, x.Password)).WithMessage("ユーザID,またはパスワードが間違っています。");
            }
        }
        /// <summary>
        /// ユーザの存在チェック
        /// </summary>
        /// <param name="id"></param>
        /// <returns>false:正常 true:異常</returns>
        private bool IsExistId(string id)
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
